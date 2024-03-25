// <copyright file="NotFoundException.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// An exception indicating that a resource was not found.
    /// </summary>
    public sealed class NotFoundException : Exception {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="message">The error message associated with the
        /// exception.</param>
        public NotFoundException(string message) : base(message) { }
    }
}
