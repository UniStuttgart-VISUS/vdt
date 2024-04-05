// <copyright file="DismScope.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Initialises the DISM library and clears it when being finalised.
    /// </summary>
    internal sealed class DismScope : IDismScope, IDisposable {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="options"></param>
        public DismScope(IOptions<DismOptions> options,
                ILogger<DismScope> logger) {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            Options = options?.Value ?? new();

            _logger.LogTrace(Resources.DismInitialise,
                Options.LogFile,
                Options.ScratchDirectory);
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
            Dispose(false);
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public DismOptions Options { get; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private methods
        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _logger.LogTrace(Resources.DismShutdown);
                    DismApi.Shutdown();
                }

                // No unmanaged resources to free here.

                _disposed = true;
            }

            Debug.Assert(_disposed);
        }
        #endregion

        #region Private fields
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion
    }
}
