// <copyright file="ImageServicing.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implements servicing of WIM images using DISM.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ImageServicing : IImageServicing {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="dism">The DISM scope which makes sure that the API has
        /// been loaded.</param>
        /// <param name="logger">A logger for reporting progress and errors.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public ImageServicing(IDismScope dism,
                ILogger<IImageServicing> logger) {
            this._dism = dism
                ?? throw new ArgumentNullException(nameof(dism));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public bool IsOpen => this._session != null;

        /// <inheritdoc />
        public string Name => this._path ?? RuntimeInformation.OSDescription;

        /// <inheritdoc />
        public string? Path => this._path;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void AddPackage(string path, bool ignoreCheck,
                bool preventPending) {
            this.CheckSession();
            this._logger.LogTrace("Adding component {Path} to image {Image}. "
                + "Ignore check is {IgnoreCheck}, prevent pending is "
                + "{PreventPending}", path, this.Name, ignoreCheck,
                preventPending);
            DismApi.AddPackage(this._session, path, ignoreCheck, preventPending);
        }

        /// <inheritdoc />
        public void AddProvisionedAppxPackage(string appPath,
                List<string>? dependencyPackages,
                string? licencePath,
                string? customDataPath) {
            this.CheckSession();
            this._logger.LogTrace("Adding provisioned appx package {Package} "
                + "to image {Image}.", appPath, this.Name);
            DismApi.AddProvisionedAppxPackage(this._session,
                appPath,
                dependencyPackages,
                licencePath,
                customDataPath!);
        }

        /// <inheritdoc />
        public void ApplyUnattend(string path, bool singleSession) {
            this.CheckSession();
            this._logger.LogTrace("Applying unattend file {Path} to "
                + "image {Image}. Single session is {SingleSession}.",
                path, this.Name, singleSession);
            DismApi.ApplyUnattend(this._session, path, singleSession);
        }

        /// <inheritdoc />
        public IEnumerable<DismAppxPackage> GetProvisionedAppxPackages() {
            this.CheckSession();
            this._logger.LogTrace("Retrieving provisioned appx packages for "
                + "image {Image}.", this.Name);
            return DismApi.GetProvisionedAppxPackages(this._session);
        }

        /// <inheritdoc />
        public void Commit() {
            this.CheckSession();
            if (this._path is not null) {
                this._logger.LogTrace("Committing changes to DISM image "
                    + "{Image}.", this.Name);
                DismApi.CommitImage(this._session, false);
            } else {
                this._logger.LogTrace("Changes to the current Windows in an "
                    + "online session are effective immediately and do not "
                    + "need to be committed.");
            }

            this.Close();
        }

        /// <inheritdoc />
        public void Dispose() {
            if (this._session != null) {
                this._session.Dispose();
                this._session = null;
            }
        }

        /// <inheritdoc />
        public void EnableFeature(string feature, bool limitAccess,
                bool enableAll) {
            this.CheckSession();
            this._logger.LogTrace("Enabling feature {Feature} on image "
                + "{Image}. Limit access is {LimitAccess}, enable all is "
                + "{EnableAll}", feature, this.Name, limitAccess, enableAll);
            DismApi.EnableFeature(this._session, feature, limitAccess,
                enableAll);
        }

        /// <inheritdoc />
        public void InjectDrivers(string folder, bool recursive,
                bool forceUnsigned) {
            this.CheckSession();
            this._logger.LogTrace("Injecting drivers from {Path} to "
                + "image {Image}. Recursive is {Recursive}, force unsigned "
                + "is {ForceUnsigned}.", folder, this.Name, recursive,
                forceUnsigned);
            DismApi.AddDriversEx(this._session,
                folder,
                forceUnsigned,
                recursive);
        }

        /// <inheritdoc />
        public void Open(string? path) {
            this.CheckNoSession();

            if ((this._path = path) is not null) {
                this._logger.LogTrace("Opening an offline servicing session "
                    + "for {Image}.", this._path);
                this._session = DismApi.OpenOfflineSessionEx(this._path);

            } else {
                this._logger.LogTrace("Opening an online servicing session for "
                    + "the current Windows installation.");
                this._session = DismApi.OpenOnlineSessionEx();
                Debug.Assert(this._path is null);
            }
        }

        /// <inheritdoc />
        public void RemoveProvisionedAppxPackage(string packageName) {
            this.CheckSession();
            this._logger.LogTrace("Removing provisioned appx package {Package} "
                + "from image {Image}.", packageName, this.Name);
            DismApi.RemoveProvisionedAppxPackage(this._session, packageName);
        }

        /// <inheritdoc />
        public void RollBack() {
            this.CheckSession();
            this._logger.LogTrace("Reverting changes to DISM image "
                + "{Image}.", this.Name);
            DismApi.CommitImage(this._session, true);
            this.Close();
        }
        #endregion

        #region Private methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckNoSession() {
            if (this.IsOpen) {
                throw new InvalidOperationException(
                    Errors.DuplicateDismSession);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MemberNotNull(nameof(_session))]
        private void CheckSession() => _ = this._session
            ?? throw new InvalidOperationException(Errors.NoDismSession);

        private void Close() {
            this._logger.LogTrace("Disposing session handle.");
            this._session?.Dispose();
            this._session = null;
            //DismApi.GetProvisionedAppxPackages()

        }
        #endregion

        #region Private fields
        private readonly IDismScope _dism;
        private readonly ILogger _logger;
        private string? _path;
        private DismSession? _session;
        #endregion
    }
}
