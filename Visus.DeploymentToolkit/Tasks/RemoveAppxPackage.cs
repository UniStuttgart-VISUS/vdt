// <copyright file="RemoveAppxPackage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Adds a component to a windows installation using DISM.
    /// </summary>
    public sealed class RemoveAppxPackage : ImageServicingTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="imageServicing">A new image servicing implementation
        /// to perform the work.</param>
        /// <param name="logger">The logger used to report results of the
        /// operation.</param>
        public RemoveAppxPackage(IState state,
                IImageServicing imageServicing,
                ILogger<ApplyUnattend> logger)
                : base(state,imageServicing, logger) {
            this.Name = Resources.RemoveAppxPackage;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the names of the appx packages that should be removed
        /// from being provisioned to all new users.
        /// </summary>
        /// <remarks>
        /// <para> You can use regular expressions to match the packages names
        /// (not the display names!).</para>
        /// </remarks>
        [Required]
        public IEnumerable<string> Packages {
            get;
            set;
        } = Enumerable.Empty<string>();
        #endregion
         
        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            if (!this.Packages.Any()) {
                this._logger.LogWarning("No packages specified to remove.");
                return Task.CompletedTask;

            } else {
                return Task.Run(() => {
                    try {
                        var regex = new Regex(string.Join("|",
                            this.Packages.Select(p => $"({p})")),
                            RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        this._logger.LogInformation("Removing provisioned appx "
                            + "packages matching {Packages} from {Image}.",
                            regex, this.EffectiveInstallationPath);

                        var servicing = this.Open();
                        var packages = servicing.GetProvisionedAppxPackages();
                        this._logger.LogTrace("Found {Count} provisioned appx "
                            + "packages in {Image}.", packages.Count(),
                            this.EffectiveInstallationPath);

                        foreach (var p in packages) {
                            if (regex.IsMatch(p.PackageName)) {
                                this._logger.LogInformation("Removing package "
                                    + "{Name} from {Image}.", p.PackageName,
                                    this.EffectiveInstallationPath);
                                servicing.RemoveProvisionedAppxPackage(
                                    p.PackageName);

                            } else {
                                this._logger.LogTrace("Skipping package {Name} "
                                    + "as it does not match the specified"
                                    + " pattern.", p.PackageName);
                            }
                        }
                    } finally {
                        this.Close();
                    }
                }, cancellationToken);
            }
        }
        #endregion
    }
}
