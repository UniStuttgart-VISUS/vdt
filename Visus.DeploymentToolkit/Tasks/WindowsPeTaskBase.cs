// <copyright file="WindowsPeTaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A base class for tasks working with Windows PE images.
    /// </summary>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public abstract class WindowsPeTaskBase : TaskBase {

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
        /// Gets or sets the root directory for the deployment tools, most
        /// importantly including the firmware files and the oscdimg tool for
        /// creating ISOs.
        /// </summary>
        [DirectoryExists]
        public string DeploymentToolsRootDirectory {
            get;
            set;
        } = Waik.Defaults.DeploymentToolsPath;

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
        } = Waik.Defaults.WinPePath;
        #endregion

        #region Protected constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        protected WindowsPeTaskBase(IState state, ILogger logger)
            : base(state, logger) { }
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
        protected string WimPath => Path.Combine(
            this.MediaDirectory, "sources", "boot.wim");

        /// <summary>
        /// Gets the architecture string used in the WinPE paths, which is
        /// derived from <see cref="Architecture"/>.
        /// </summary>
        protected string WinPeArchitecture => Waik.Tools.GetArchitecturePath(
            this.Architecture);
        #endregion
    }
}
