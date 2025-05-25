// <copyright file="ImageServicing.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Imaging {

    /// <summary>
    /// Implements servicing of WIM images using DISM.
    /// </summary>
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
        public string Name => this._path ?? RuntimeInformation.OSDescription;

        /// <inheritdoc />
        public string? Path => this._path;
        #endregion

        #region Public metods
        /// <inheritdoc />
        public void ApplyUnattend(string path, bool singleSession) {
            this.CheckSession();
            DismApi.ApplyUnattend(this._session, path, singleSession);
        }

        /// <inheritdoc />
        public void Commit() {
            this.CheckSession();
            this._logger.LogTrace("Committing changes to DISM image "
                + "\"{Image}\".", this.Name);
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
            this.CheckSession();
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
                    + "for \"{Image}\".", this._path);
                this._session = DismApi.OpenOfflineSessionEx(this._path);

            } else {
                this._logger.LogTrace("Opening an online servicing session for "
                    + "the current Windows installation.");
                this._session = DismApi.OpenOnlineSessionEx();
                Debug.Assert(this._path is null);
            }
        }

        /// <inheritdoc />
        public void RollBack() {
            this.CheckSession();
            this._logger.LogTrace("Reverting changes to DISM image "
                + "\"{Image}\".", this.Name);
            DismApi.CommitImage(this._session, true);
        }
        #endregion

        #region Private methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckNoSession() {
            if (this._session != null) {
                throw new InvalidOperationException(
                    Errors.DuplicateDismSession);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MemberNotNull(nameof(_session))]
        private void CheckSession() => _ = this._session
            ?? throw new InvalidOperationException(Errors.NoDismSession);
        #endregion

        #region Private fields
        private readonly IDismScope _dism;
        private readonly ILogger _logger;
        private string? _path;
        private DismSession? _session;
        #endregion
    }
}
