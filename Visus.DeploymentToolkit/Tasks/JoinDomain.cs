// <copyright file="JoinDomain.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Compliance;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Joins the machine to a domain.
    /// </summary>
    public sealed class JoinDomain : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JoinDomain(IState state,
                IDomainService domainService,
                ILogger<JoinDomain> logger)
                : base(state, logger) {
            this._domainService = domainService
                ?? throw new ArgumentNullException(nameof(domainService));
            this.Name = Resources.JoinDomain;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the domain to join.
        /// </summary>
        [FromState(WellKnownStates.DeploymentShareDomain)]
        [Required]
        public string? Domain {
            get => this._domain;
            set {
                this._domain = value;
                this.Options |= JoinOptions.JoinDomain;
            }
        }

        /// <summary>
        /// Gets or sets whether the computer should be joined as dual-boot
        /// machine, which means that the task creates a keytab for use with the
        /// Linux installation.
        /// </summary>
        public bool IsDualBoot { get; set; } = false;

        /// <summary>
        /// Gets or sets path where the keytab file should be created.
        /// </summary>
        /// <remarks>
        /// <para>This property has only an effect if <see cref="IsDualBoot"/>
        /// is <see langword="true"/>.</para>
        /// <para>If the path is not absolute, it is relative to the working
        /// directory of the agent.</para>
        /// </remarks>
        [Required]
        public string Keytab { get; set; } = "krb5.keytab";

        /// <summary>
        /// Gets or sets the length of the machine password generated.
        /// </summary>
        /// <remarks>
        /// This property has only an effect if <see cref="IsDualBoot"/> is
        /// <see langword="true"/>.
        /// </remarks>
        [Range(1, 512)]
        public int MachinePasswordLength { get; set; } = 64;

        /// <summary>
        /// Gets or sets the options for the join operation.
        /// </summary>
        /// <remarks>
        /// <para>Make sure that <see cref="JoinOptions.JoinDomain"/> is set to
        /// join a domain rather than a workgroup.</para>
        /// </remarks>
        [DefaultValue(JoinOptions.JoinDomain)]
        JoinOptions Options { get; set; } = JoinOptions.JoinDomain;

        /// <summary>
        /// Gets or sets the full DN of the organisational unit (OU) of the
        /// machine account to be created.
        /// </summary>
        public string? OrganisationalUnit { get; set; }

        /// <summary>
        /// Gets or sets the password of <see cref="User"/> if a user account
        /// is specified.
        /// </summary>
        [FromState(WellKnownStates.DeploymentSharePassword)]
        [SensitiveData]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the workgroup to join.
        /// </summary>
        /// <remarks>
        /// This property is an alias for <see cref="Domain"/>. The semantic is
        /// controlled by the <see cref="Options"/> property. Using this
        /// property will erase this flag whereas setting the
        /// <see cref="Domain"/> will set it.
        /// </remarks>
        [Required]
        public string? Workgroup {
            get => this._domain;
            set {
                this._domain = value;
                this.Options &= ~JoinOptions.JoinDomain;
            }
        }

        /// <summary>
        /// Gets or sets the name of the user account to use for joining the
        /// machine to the domain. This can be omitted if
        /// <see cref="JoinOptions.Unsecure"/> is used to join the machine with
        /// an existing machine account.
        /// </summary>
        [FromState(WellKnownStates.DeploymentShareUser)]
        public string? User { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            if (this.IsDomain) {
                var domain = await this.JoinDomainAsync(cancellationToken);

                if (this.IsDualBoot) {
                    this._logger.LogInformation("Re-joining pre-created "
                        + "machine account to obtain a shared keytab.");

                    // Preserve the account that can connect to the DC.
                    var user = this.User;
                    var password = this.Password;

                    // Adjust the options to do an unsecure join.
                    this.Options |= JoinOptions.JoinIfJoined;
                    this.Options |= JoinOptions.Unsecure;
                    this.Options |= JoinOptions.MachinePasswordPassed;

                    // No user is required as we pass the machine password.
                    this.User = null;

                    // Generate the machine password.
                    var chars = (from i in Enumerable.Range(0, 1 << 8)
                                 let c = (char) i
                                 where !char.IsControl(c)
                                 select c).ToArray();
                    this.Password = RandomNumberGenerator.GetString(chars,
                        this.MachinePasswordLength);

                    // Join again on the same DC, but with different options.
                    this.JoinResolvedDomain(domain);

                    // Now that we know the machine password in AD, we can
                    // create a keytab for use with Linux.
                    this._logger.LogInformation("Creating keytab.");
                    var keytab = await this._domainService.CreateKeyTableAsync(
                        Environment.MachineName,
                        this.Password,
                        domain.DomainController,
                        user,
                        password,
                        IDomainService.DefaultEncryptionTypes);

                    this._logger.LogInformation("Saving keytab to {Path}.",
                        this.Keytab);
                    using (var f = File.Open(this.Keytab, FileMode.Create,
                        FileAccess.ReadWrite, FileShare.None))
                    using (var w = new BinaryWriter(f)) {
                        keytab.Write(w);
                    }
                }

            } else {
                this.JoinWorkgroup();
            }
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets whether the task should join a domain rather than a workgroup.
        /// </summary>
        private bool IsDomain => ((this.Options & JoinOptions.JoinDomain) != 0);
        #endregion

        #region Private methods
        /// <summary>
        /// Join the configured <see cref="Domain"/>.
        /// </summary>
        private async Task<DomainInfo> JoinDomainAsync(
                CancellationToken cancellationToken) {
            Debug.Assert(this.IsDomain);
            Debug.Assert(!string.IsNullOrWhiteSpace(this.Domain));

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogInformation("Discovering domain {Domain}.",
                this.Domain!);
            var retval = await this._domainService.DiscoverAsync(this.Domain!);

            cancellationToken.ThrowIfCancellationRequested();
            this.JoinResolvedDomain(retval);

            return retval;
        }

        /// <summary>
        /// Join the given <paramref name="domain"/> that has already been
        /// resolved.
        /// </summary>
        private void JoinResolvedDomain(DomainInfo domain) {
            Debug.Assert(domain != null);
            this._logger.LogInformation("Joining domain {Domain} using "
                + "{DomainController}.", domain.Name, domain.DomainController);
            try {
                this._domainService.JoinDomain(domain.NameWithDomainController,
                    this.User,
                    this.Password,
                    this.Options,
                    this.OrganisationalUnit);
            } catch (Exception ex) {
                if (string.IsNullOrEmpty(this.OrganisationalUnit)) {
                    throw;
                }

                this._logger.LogWarning(ex, "Failed to join domain {Domain}. "
                    + "This might be caused by the machine already existing in "
                    + "another location than {OrganisationalUnit}.",
                    domain.Name, this.OrganisationalUnit);
                this.OrganisationalUnit = null;
                this.JoinResolvedDomain(domain);
            }
        }

        /// <summary>
        /// Join the configured <see cref="Workgroup"/>.
        /// </summary>
        private void JoinWorkgroup() {
            Debug.Assert(!this.IsDomain);
            Debug.Assert(!string.IsNullOrWhiteSpace(this.Workgroup));
            this._logger.LogInformation("Joining workgroup {Workgroup}.",
                this.Workgroup);
            this._domainService.JoinDomain(this.Workgroup, null, null,
                this.Options, null);
        }
        #endregion

        #region Private fields
        private string? _domain;
        private readonly IDomainService _domainService;
        #endregion
    }
}
