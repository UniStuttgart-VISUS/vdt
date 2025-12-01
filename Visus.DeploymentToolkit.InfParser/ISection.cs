// <copyright file="ISection.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


using System.Diagnostics;

namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// Defines the minimal interface of a section in an INF file.
    /// </summary>
    public interface ISection {

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name { get; }
    }
}
