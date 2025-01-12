// <copyright file="State.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The default implementation of the application state.
    /// </summary>
    /// <remarks>
    /// Manipulation of the state is thread-safe.
    /// </remarks>
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
        public string? DeploymentShare {
            get => this.Get(WellKnownStates.DeploymentShare) as string;
            set => this.Set(WellKnownStates.DeploymentShare, value);
        }

        /// <inheritdoc />
        [JsonIgnore]
        public IDismScope? DismScope {
            get => this.Get(WellKnownStates.DismScope) as IDismScope;
            set {
                if ((this.DismScope != null) && (value != null)) {
                    throw new InvalidOperationException(
                        Errors.DuplicateDismScope);
                }

                this.Set(WellKnownStates.DismScope, value);
            }
        }

        /// <inheritdoc />
        public IDisk? InstallationDisk {
            get => this.Get(WellKnownStates.InstallationDisk) as IDisk;
            set => this.Set(WellKnownStates.InstallationDisk, value);
        }

        /// <inheritdoc />
        public Phase Phase {
            get => this.Get(WellKnownStates.Phase) as Phase? ?? Phase.Unknown;
            set => this.Set(WellKnownStates.Phase, value);
        }

        /// <inheritdoc />
        public int Progress {
            get => this.Get(WellKnownStates.Progress) as int? ?? 0;
            set => this.Set(WellKnownStates.Progress, value);
        }

        /// <inheritdoc />
        public string? WorkingDirectory {
            get => this.Get(WellKnownStates.WorkingDirectory) as string;
            set => this.Set(WellKnownStates.WorkingDirectory, value);
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public object? Get(string key) {
            lock (this._lock) {
                return this._values.TryGetValue(key, out var value)
                    ? value
                    : null;
            }
        }

        /// <inheritdoc />
        public async Task SaveAsync(string path) {
            this._logger.LogTrace("Persisting the deployment state to "
                + "\"{StateFile}\".\t\r\n", path);
            using var file = File.Open(path, FileMode.Create,
                FileAccess.ReadWrite);
            var opts = new JsonSerializerOptions() {
                WriteIndented = true,
            };

            await JsonSerializer.SerializeAsync(file, this._values, opts)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public object? Set(string key, object? value) {
            lock (this._lock) {
                this._values.TryGetValue(key, out var retval);
                this._values[key] = value;
                this._logger.LogInformation("Changing state \"{State}\" "
                    + "from \"{OldValue}\" to \"{NewValue}\".",
                    key, retval, value);
                return retval;
            }
        }
        #endregion

        #region Private fields
        private readonly object _lock = new();
        private readonly ILogger _logger;
        private readonly Dictionary<string, object?> _values = new();
        #endregion
    }
}
