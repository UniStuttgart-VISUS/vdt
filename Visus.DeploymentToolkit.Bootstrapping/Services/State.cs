// <copyright file="State.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Compliance;
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
        public string? AgentPath {
            get => this[WellKnownStates.AgentPath] as string;
            set => this[WellKnownStates.AgentPath] = value;
        }

        /// <inheritdoc />
        public string? DeploymentDirectory {
            get => this[WellKnownStates.DeploymentDirectory] as string;
            set => this[WellKnownStates.DeploymentDirectory] = value;
        }

        /// <inheritdoc />
        public string? DeploymentShare {
            get => this[WellKnownStates.DeploymentShare] as string;
            set => this[WellKnownStates.DeploymentShare] = value;
        }

        /// <inheritdoc />
        public string? DeploymentShareDomain {
            get => this[WellKnownStates.DeploymentShareDomain] as string;
            set => this[WellKnownStates.DeploymentShareDomain] = value;
        }

        /// <inheritdoc />
        [SensitiveData]
        public string? DeploymentSharePassword {
            get => this[WellKnownStates.DeploymentSharePassword] as string;
            set => this[WellKnownStates.DeploymentSharePassword] = value;
        }

        /// <inheritdoc />
        public string? DeploymentShareUser {
            get => this[WellKnownStates.DeploymentShareUser] as string;
            set => this[WellKnownStates.DeploymentShareUser] = value;
        }

        /// <inheritdoc />
        public string? InstallationDirectory {
            get => this[WellKnownStates.InstallationDirectory] as string;
            set => this[WellKnownStates.InstallationDirectory] = value;
        }

        /// <inheritdoc />
        public IDisk? InstallationDisk {
            get => this[WellKnownStates.InstallationDisk] as IDisk;
            set => this[WellKnownStates.InstallationDisk] = value;
        }

        /// <inheritdoc />
        public Phase Phase {
            get => this[WellKnownStates.Phase] as Phase? ?? Phase.Unknown;
            set => this[WellKnownStates.Phase] = value;
        }

        /// <inheritdoc />
        public int Progress {
            get => this[WellKnownStates.Progress] as int? ?? 0;
            set => this[WellKnownStates.Progress] = value;
        }

        /// <inheritdoc />
        [SensitiveData]
        public string SessionKey {
            get {
                var retval = this[WellKnownStates.SessionKey] as string;

                if (retval is null) {
                    this[WellKnownStates.SessionKey]
                        = retval
                        = RandomNumberGenerator.GetString(Vocabulary, 256);
                }

                return retval;
            }
            set {
                var current = this[WellKnownStates.SessionKey] as string;

                // Note: we allow the same key to be set multiple times just
                // to make serialisation easier.
                if ((current is not null) && (current != value)) {
                    throw new InvalidOperationException(
                        Errors.CannotChangeSessionKey);
                }

                this[WellKnownStates.SessionKey] = value;
            }
        }

        /// <inheritdoc />
        public string? StateFile {
            get => this[WellKnownStates.StateFile] as string;
            set => this[WellKnownStates.StateFile] = value;
        }

        /// <inheritdoc />
        public object? TaskSequence {
            get => this[WellKnownStates.TaskSequence];
            set {
                this[WellKnownStates.TaskSequence] = value switch {
                    string => value,
                    ITaskSequence => value,
                    null => value,
                    _ => throw new ArgumentException(Errors.NoTaskSequence)
                };
            }
        }

        /// <inheritdoc />
        public string? WorkingDirectory {
            get => this[WellKnownStates.WorkingDirectory] as string;
            set => this[WellKnownStates.WorkingDirectory] = value;
        }

        /// <inheritdoc />
        public IDismMount? WimMount {
            get => this[WellKnownStates.WimMount] as IDismMount;
            set => this[WellKnownStates.WimMount] = value;
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task SaveAsync(string? path) {
            // If we have no user-defined path, use the state file. If that is
            // also not set, it is a critical error.
            if (string.IsNullOrEmpty(path)) {
                path = this.StateFile;
            }
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentNullException(nameof(path));
            }

            // Update the location of the state file before writing it such that
            // anyone reading it will use the same path.
            this.StateFile = path;

            this._logger.LogTrace("Persisting the deployment state to "
                + "\"{StateFile}\".", path);
            using var file = File.Open(path, FileMode.Create,
                FileAccess.ReadWrite);
            var opts = new JsonSerializerOptions() {
                WriteIndented = true,
            };

            await JsonSerializer.SerializeAsync(file, this._values, opts)
                .ConfigureAwait(false);
        }
        #endregion

        #region Public indexers
        /// <inheritdoc />
        public object? this[string key] {
            get {
                lock (this._lock) {
                    return this._values.TryGetValue(key, out var value)
                        ? value
                        : null;
                }
            }
            set {
                lock (this._lock) {
                    this._values.TryGetValue(key, out var retval);
                    this._values[key] = value;
                    // TODO: redaction must be applied manually here!
                    this._logger.LogInformation("Changing state \"{State}\" "
                        + "from \"{OldValue}\" to \"{NewValue}\".",
                        key, retval, value);
                }
            }
        }
        #endregion

        #region Private fields
        private const string Vocabulary = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefg"
            + "hijklmnopqrstuvwxyzäöü0123456789+-*/!§$%&/()=?*_:;{}[]#-.,+~@|µ";
        private readonly object _lock = new();
        private readonly ILogger _logger;
        private readonly Dictionary<string, object?> _values = new();
        #endregion
    }
}
