// <copyright file="ImageServicing.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using System;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Imaging {

    /// <summary>
    /// Implements servicing of WIM images using DISM.
    /// </summary>
    internal sealed class ImageServicing : IImageServicing {

        #region Public constructors
        public ImageServicing(IDismScope dism,
                ILogger<IImageServicing> logger) {
            this._dism = dism
                ?? throw new ArgumentNullException(nameof(dism));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Commit() {
            _ = this._session
                ?? throw new InvalidOperationException(Errors.NoDismSession);
            DismApi.CommitImage(this._session, false);
        }


        /// <inheritdoc />
        public void Dispose() {
            if (this._session != null) {
                this._session.Dispose();
                this._session = null;
            }
        }

        /// <inheritdoc />
        public void InjectDrivers(string folder,
                bool recursive = false,
                bool forceUnsigned = false) {
            _ = this._session
                ?? throw new InvalidOperationException(Errors.NoDismSession);
            DismApi.AddDriversEx(this._session,
                folder,
                forceUnsigned,
                recursive);
        }

        /// <inheritdoc />
        public void Open(string path) {
            _ = path ?? throw new ArgumentNullException(nameof(path));
            this._logger.LogInformation(Resources.DismOpenOffline, path);
            this._session = DismApi.OpenOfflineSessionEx(path);

        }

        /// <inheritdoc />
        public void RollBack() {
            _ = this._session
                ?? throw new InvalidOperationException(Errors.NoDismSession);
            DismApi.CommitImage(this._session, true);
        }
        #endregion

        #region Private fields
        private readonly IDismScope _dism;
        private readonly ILogger _logger;
        private DismSession? _session;
        #endregion
    }
}
