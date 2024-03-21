// <copyright file="IEnvironment.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a service providing information about the
    /// environment variables of the machine.
    /// </summary>
    public interface IEnvironment : IEnumerable<KeyValuePair<string, string>> {

        /// <summary>
        /// Gets or sets the environment variable with the specified
        /// <paramref name="name"/>.
        /// </summary>
        /// <remarks>
        /// If the environment variable is set, it will be set for
        /// <see cref="System.EnvironmentVariableTarget.Process"/> and
        /// <see cref="System.EnvironmentVariableTarget.User"/>, ie the variable
        /// will be persisted for the current user and affect all new processes.
        /// </remarks>
        /// <param name="name">The name of the environment variable to retrieve.
        /// </param>
        /// <returns>The value of the environment variable or <c>null</c> is the
        /// variable is not set.</returns>
        string? this[string name] {
            get;
            set;
        }
    }
}
