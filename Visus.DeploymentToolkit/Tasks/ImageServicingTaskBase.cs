// <copyright file="ImageServicingTaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A base class for tasks performing image servicing tasks.
    /// </summary>
    /// <param name="state">The current state of the task sequence.</param>
    /// <param name="imageServicing">A DISM wrapper for manipulating images.
    /// </param>
    /// <param name="logger">A logger for progress and error messages.</param>
    public abstract class ImageServicingTaskBase(
            IState state,
            IImageServicing imageServicing,
            ILogger logger)
            : TaskBase(state, logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the Windows installation being modified.
        /// </summary>
        /// <remarks>
        /// If this path is <c>null</c>, an online servicing session for the
        /// current Windows installation is opened.
        /// </remarks>
        public string? InstallationPath { get; set; }

        /// <summary>
        /// Gets or sets whether the task should commit the changes once it is
        /// done.
        /// </summary>
        /// <remarks>
        /// This property defaults to <c>true</c>. If it is set <c>false</c>,
        /// the session will be kept open for subsequent tasks to modfiy, which
        /// also need to commit the changes. If no subsequent task does the
        /// commit, the changes will be discarded.
        /// </remarks>
        public bool IsCommit { get; set; } = true;

        /// <summary>
        /// Gets or sets whether a missing <see cref="InstallationPath"/>" must
        /// not be replaced by implicit information from the state.
        /// </summary>
        public bool IsForceOnline { get; set; }
        #endregion

        #region Protected properties
        /// <summary>
        /// Gets the <see cref="InstallationPath"/>, or if it is not set, any
        /// mounted image from the current state unless
        /// <see cref="IsForceOnline" /> is set.
        /// </summary>
        protected string? EffectiveInstallationPath {
            get {
                if (this.IsForceOnline) {
                    this._logger.LogTrace("Forcing online servicing was "
                        + "requested.");
                    return null;
                }

                if (this.InstallationPath is not null) {
                    this._logger.LogTrace("Using user-provided installation "
                        + "path \"{Path}\".", this.InstallationPath);
                    return this.InstallationPath;
                }

                this._logger.LogTrace("No installation path was set, so we "
                    + "consider any state specified in "
                    + $"{WellKnownStates.WimMount} as implicit path.");
                this.InstallationPath = this._state.WimMount?.MountPoint;
                if (this.InstallationPath is not null) {
                    return this.InstallationPath;
                }

                this._logger.LogTrace("No active WIM mount point was found, so "
                    + "we consider any state specified in "
                    + $"{nameof(this.InstallationPath)}.");
                this.InstallationPath = this._state[
                    nameof(this.InstallationPath)] as string;

                return this.InstallationPath;
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// If <see cref="IsCommit"/> is set, this method commits the changes
        /// and closes the session. Otherwise, it does nothing in expectation
        /// of a subsequent task to commit the changes.
        /// </summary>
        protected void Close() {
            if (this.IsCommit) {
                this._logger.LogTrace("Committing changes to the image "
                    + "\"{Image}\".", this._imageServicing.Name);
                this._imageServicing.Commit();
            } else {
                this._logger.LogTrace("Keeping imaging session \"{Image}\""
                    + "alive.", this._imageServicing.Name);
            }
        }

        /// <summary>
        /// Opens the installation or online session if there is no active
        /// session from a previous task.
        /// </summary>
        /// <returns>The <see cref="IImageServicing"/> with the open session.
        /// </returns>
        protected IImageServicing Open() {
            if (this._imageServicing.IsOpen) {
                this._logger.LogTrace("An active image servicing session for" +
                    " \"{Image}\" was found and will be reused.",
                    this._imageServicing.Name);
            } else {
                this._imageServicing.Open(this.EffectiveInstallationPath);
            }

            return this._imageServicing;
        }
        #endregion

        #region Protected fields
        private readonly IImageServicing _imageServicing = imageServicing
            ?? throw new ArgumentNullException(nameof(imageServicing));
        #endregion
    }
}
