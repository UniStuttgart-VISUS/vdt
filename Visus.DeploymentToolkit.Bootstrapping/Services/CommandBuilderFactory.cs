// <copyright file="CommandBuilderFactory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the command builder factory.
    /// </summary>
    internal sealed class CommandBuilderFactory : ICommandBuilderFactory {

        /// <inheritdoc />
        public ICommandBuilder Run(string processImage) {
            return new CommandBuilder(processImage);
        }
    }
}
