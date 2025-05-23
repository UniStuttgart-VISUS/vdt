// <copyright file="RegistryValueOperation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Possible operations to be peformed by the <see cref="RegistryValue"/>
    /// task.
    /// </summary>
    public enum RegistryValueOperation {

        /// <summary>
        /// Add a new value, but only if it does not already exist.
        /// </summary>
        Add,

        /// <summary>
        /// Change a value, but only if it exists.
        /// </summary>
        Change,

        /// <summary>
        /// Delete a value.
        /// </summary>
        Delete,

        /// <summary>
        /// Set a value, regardless of whether it exists or not.
        /// </summary>
        Set
    }
}
