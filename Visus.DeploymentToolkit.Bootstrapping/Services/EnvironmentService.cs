// <copyright file="EnvironmentService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The default implementation of <see cref="IEnvironment"/>, which uses
    /// <see cref="Environment"/> to provide access to the environment
    /// variables.
    /// </summary>
    internal sealed class EnvironmentService : IEnvironment {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EnvironmentService(ILogger<EnvironmentService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            var variables = Environment.GetEnvironmentVariables();

            var retval = from v in variables.Cast<DictionaryEntry>()
                         let name = (string) v.Key
                         let value = (string?) v.Value
                         select new KeyValuePair<string, string>(name, value);

            return retval.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion

        #region Public indexers
        /// <inheritdoc />
        public string? this[string name] {
            get {
                var vars = Environment.GetEnvironmentVariables();
                if (vars?.Contains(name) == true) {
                    return (string?) vars[name];
                } else {
                    return null;
                }
            }
            set {
                Environment.SetEnvironmentVariable(name,
                    value,
                    EnvironmentVariableTarget.Process);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    Environment.SetEnvironmentVariable(name,
                    value,
                    EnvironmentVariableTarget.User);
                }

                this._logger.LogInformation(Resources.ChangeEnvironment,
                    name,
                    value);
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        #endregion
    }
}
