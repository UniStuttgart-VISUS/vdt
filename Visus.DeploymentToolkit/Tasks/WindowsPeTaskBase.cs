// <copyright file="WindowsPeTaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A base class for tasks working with Windows PE images.
    /// </summary>
    /// <param name="state">The current state of the task sequence.</param>
    /// <param name="tools">Provides access to the locations of the deployment
    /// tools.</param>
    /// <param name="logger">A logger for progress and error messages.
    /// </param>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public abstract class WindowsPeTaskBase(IState state,
            IOptions<ToolsOptions> tools,
            ILogger logger)
            : TaskBase(state, logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the architecture to use for the PE image.
        /// </summary>
        /// <remarks>
        /// This parameter defaults to the architecture of the current process.
        /// </remarks>
        public Architecture Architecture {
            get;
            set;
        } = RuntimeInformation.ProcessArchitecture;

        /// <summary>
        /// Gets or sets the working directory where the image will be staged.
        /// </summary>
        /// <remarks>
        /// If this property is not set, a temporary directory will be created,
        /// which can be retrieved from this property.
        /// </remarks>
        [FromState(WellKnownStates.WorkingDirectory)]
        public string? WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the root directory where the WinPE images are stored in
        /// the Windows Assessment and Deployment Kit (ADK).
        /// </summary>
        [DirectoryExists]
        public string WinPeSourceDirectory {
            get;
            set;
        } = tools.Value?.WinPePath!;
        #endregion

        #region Protected properties
        /// <summary>
        /// Gets the directory where the firmware files are copied to.
        /// </summary>
        protected string FirmwareDirectory => Path.Combine(
            this.WorkingDirectory!, "fwfiles");

        /// <summary>
        /// Gets the directory where the media files are copied to.
        /// </summary>
        protected string MediaDirectory => Path.Combine(
            this.WorkingDirectory!, "media");

        /// <summary>
        /// Gets the directory where the WinPE image will be mounted.
        /// </summary>
        protected string MountDirectory => Path.Combine(
            this.WorkingDirectory!, "mount");

        /// <summary>
        /// Gets the path where the WinPE image file will be copied to.
        /// </summary>
        protected string WimPath
            => Path.Combine(this._tools.EvaluateArchitecture(
                this.MediaDirectory, this.Architecture),
                "sources", "boot.wim");

        /// <summary>
        /// Gets the architecture string used in the WinPE paths, which is
        /// derived from <see cref="Architecture"/>.
        /// </summary>
        protected string WinPeArchitecture => this.Architecture.GetFolder();
        #endregion

        #region Protected fields
        /// <summary>
        /// Provides access to the locations of the deployment tools.
        /// </summary>
        protected readonly ToolsOptions _tools = tools?.Value
            ?? throw new ArgumentNullException(nameof(tools));
        #endregion
    }
}
