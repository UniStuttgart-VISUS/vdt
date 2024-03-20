// <copyright file="ICommandBuilderFactory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A factory for <see cref="ICommandBuilders"/>s.
    /// </summary>
    public interface ICommandBuilderFactory {

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="ICommandBuilder"/> that runs the given
        /// process image.
        /// </summary>
        /// <param name="processImage">The path to the process image, which
        /// must be an absolute path.</param>
        /// <returns>A builder for the given command.</returns>
        ICommandBuilder Run(string processImage);
        #endregion

    }
}
