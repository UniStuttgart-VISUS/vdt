// <copyright file="DismScope.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024  - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Initialises the DISM library and clears it when being finalised.
    /// </summary>
    internal sealed class DismScope : IDismScope {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="options"></param>
        public DismScope(IOptions<DismOptions> options,
                ILogger<DismScope> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            Options = options?.Value ?? new();

            this._logger.LogTrace("Initialising DISM with {LogFile} as "
                + "log file and {ScratchDirectory} as scratch directory.",
                Options.LogFile, Options.ScratchDirectory);
            DismApi.Initialize(DismLogLevel.LogErrorsWarningsInfo,
                Options.LogFile,
                Options.ScratchDirectory);
        }
        #endregion

        #region Finaliser
        /// <summary>
        /// Finalises the instance.
        /// </summary>
        ~DismScope() {
            this.Dispose(false);
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public DismOptions Options { get; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Performs a shutdown of the DISM API if <paramref name="disposing"/>
        /// is <c>true</c> and <see cref="_disposed"/> is <c>false</c>.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing) {
            if (!this._disposed) {
                if (disposing) {
                    this._logger.LogTrace("Shutting down DISM.");
                    DismApi.Shutdown();
                }

                // No unmanaged resources to free here.

                this._disposed = true;
            }

            Debug.Assert(this._disposed);
        }
        #endregion

        #region Private fields
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion
    }
}
