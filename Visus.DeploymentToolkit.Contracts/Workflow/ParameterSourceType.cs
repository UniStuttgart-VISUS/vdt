// <copyright file="IParameterSource.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Possible types of sources for parameter values besides the obvious user
    /// input.
    /// </summary>
    public enum ParameterSourceType {

        /// <summary>
        /// The parameter value is set as a default.
        /// </summary>
        Default,

        /// <summary>
        /// The parameter might come from <see cref="Services.IState"/>.
        /// </summary>
        State,

        /// <summary>
        /// The parameter might come from an environment variable.
        /// </summary>
        Environment
    }
}
