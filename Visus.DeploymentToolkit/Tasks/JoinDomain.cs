// <copyright file="JoinDomain.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
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
        /// Gets or sets the name of the domain controller to use for the join.
        /// If this property i <see langword="null"/>, a random DC will be used.
        /// </summary>
        public string? DomainController { get; set; }

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

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogInformation("Discovering domain {Domain}.",
                this.Domain!);
            var domain = await this._domainService.DiscoverAsync(this.Domain!);

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogInformation("Joining {Domain}.", domain.Name);
            this._domainService.JoinDomain(this.Domain!,
                this.User,
                this.Password,
                this.Options,
                this.OrganisationalUnit);
        }
        #endregion

        #region Private fields
        private string? _domain;
        private readonly IDomainService _domainService;
        #endregion
    }
}
