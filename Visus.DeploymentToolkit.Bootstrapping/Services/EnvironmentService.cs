// <copyright file="EnvironmentService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The default implementation of <see cref="IEnvironment"/>, which uses
    /// <see cref="Environment"/> to provide access to the environment
    /// variables.
    /// </summary>
    internal sealed class EnvironmentService : IEnvironment {

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
        }
        #endregion
    }
}
