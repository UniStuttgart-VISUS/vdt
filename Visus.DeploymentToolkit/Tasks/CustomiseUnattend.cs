// <copyright file="CustomiseUnattend.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Unattend;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for customising an unattend.xml file.
    /// </summary>
    public sealed class CustomiseUnattend : TaskBase {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        public CustomiseUnattend(IState state,
                ILogger<CustomiseUnattend> logger)
                : base(state, logger) {
            this.Name = Resources.CustomiseUnattend;
            this.IsCritical = true;
        }

        #region Public properties
        /// <summary>
        /// Gets or sets the customisations to be applied to the unattend file.
        /// </summary>
        [Required]
        public IEnumerable<ICustomisation> Customisations { get; set; } = [];

        /// <summary>
        /// Gets ors sets the path where the customised unattend.xml file should
        /// be saved.
        /// </summary>
        /// <remarks>
        /// This parameter is optional. If not set, the customised file will be
        /// mofified in place, i.e. the original file will be overwritten.
        /// </remarks>
        public string? OutputPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the unattend.xml file to be customised.
        /// </summary>
        [Required]
        [FileExists]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.Validate();

            if (this.OutputPath == null) {
                this._logger.LogTrace("Setting output path to input path "
                    +"\"{Path}\".", this.Path);
                this.OutputPath = this.Path;
            }

            this._logger.LogInformation("Opening unatten file \"{Path}\" "
                + "for customisation.", this.Path);
            XDocument doc = null!;

            using (var s = File.OpenRead(this.Path)) {
                doc = await XDocument.LoadAsync(s,
                    LoadOptions.None,
                    cancellationToken);
            }
            Debug.Assert(doc is not null);

            foreach (var c in this.Customisations) {
                this._logger.LogTrace("Applying {Type} customisation.",
                    c.GetType().FullName);
                cancellationToken.ThrowIfCancellationRequested();
                c.Apply(doc);
            }

            this._logger.LogInformation("Saving customised unattend file "
                + "to \"{OutputPath}\".", this.OutputPath);
            using (var s = File.OpenWrite(this.OutputPath)) {
                await doc.SaveAsync(s, SaveOptions.None, cancellationToken);
            }
        }
        #endregion
    }
}
