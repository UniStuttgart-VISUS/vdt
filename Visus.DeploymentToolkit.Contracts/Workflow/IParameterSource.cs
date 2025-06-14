// <copyright file="IParameterSource.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Describes where a <see cref="IParameterDescription"/> gets its value from.
    /// </summary>
    public interface IParameterSource {

        /// <summary>
        /// Gets the source, which is a hard-coded default value or the name of
        /// a state or environment variable depending on <see cref="Type"/>.
        /// </summary>
        object? Source { get; }

        /// <summary>
        /// Gets the type of the parameter source, which allows for interpreting
        /// the <see cref="Source"/> property.
        /// </summary>
        ParameterSourceType Type { get; }
    }
}
