// <copyright file="IParameterDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Describes in a <see cref="ITaskDescription"/> where the values for a
    /// parameter might come from.
    /// </summary>
    public interface IParameterDescription {

        /// <summary>
        /// Gets whether the parameter is required.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the source of the parameter values. A user input is considered
        /// an implicit source, so this property lists all possible implicit
        /// sources only.
        /// </summary>
        IEnumerable<IParameterSource> Sources { get; }

    }
}
