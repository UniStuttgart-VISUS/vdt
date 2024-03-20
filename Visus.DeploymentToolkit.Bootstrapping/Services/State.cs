// <copyright file="State.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The default implementation of the application state.
    /// </summary>
    internal sealed class State : IState {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger">A logger used to persist changes in the state.
        /// </param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="logger"/> is <c>null</c>.</exception>
        public State(ILogger<State> logger) {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public string Phase { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task SaveAsync(string path) {
            this._logger.LogTrace(Resources.SavingState, path);

            using (var fs = File.OpenWrite(path)) {
                var opts = new JsonSerializerOptions() {
                    WriteIndented = true
                };
                await JsonSerializer.SerializeAsync(fs, this, opts);
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        #endregion
    }
}
