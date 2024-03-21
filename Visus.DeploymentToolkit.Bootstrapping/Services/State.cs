// <copyright file="State.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Workflow;


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
        public string? DeploymentShare
            => this.Get(WellKnownStates.DeploymentShare) as string;

        /// <inheritdoc />
        public Phase Phase {
            get {
                var retval = this.Get(WellKnownStates.Phase);
                return retval as Phase? ?? Phase.Unknown;
            }
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public object? Get(string key) {
            return this._values.TryGetValue(key, out var value) ? value : null;
        }

        /// <inheritdoc />
        public async Task LoadAsync(string path) {
            this._logger.LogTrace(Resources.RestoringState, path);
            using var file = File.Open(path, FileMode.Open, FileAccess.Read);
            await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
                file);
        }

        /// <inheritdoc />
        public async Task SaveAsync(string path) {
            this._logger.LogTrace(Resources.SavingState, path);
            using var file = File.Open(path, FileMode.Create,
                FileAccess.ReadWrite);
            var opts = new JsonSerializerOptions() {
                WriteIndented = true,
            };

            // Copy only the values that we can actually restore.
            var supportedValues = new Dictionary<string, object>();
            foreach (var v in this._values) {
                var valueType = v.Value.GetType();
                var supported = valueType.IsBasicJson()
                    || valueType.IsEnum
                    || valueType.IsEnumerable(TypeExtensions.IsBasicJson)
                    || valueType.IsEnumerable(t => t.IsEnum);
                if (supported) {
                    supportedValues[v.Key] = v.Value;
                }
            }

            await JsonSerializer.SerializeAsync(file, supportedValues, opts);
        }

        /// <inheritdoc />
        public object? Set(string key, object value) {
            this._values.TryGetValue(key, out var retval);
            this._values[key] = value;
            this._logger.LogInformation(Properties.Resources.ChangeState,
                key, retval, value);
            return retval;
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly Dictionary<string, object> _values = new();
        #endregion
    }
}
