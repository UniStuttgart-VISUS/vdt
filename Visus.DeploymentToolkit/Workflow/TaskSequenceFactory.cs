// <copyright file="TaskSequenceFactory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Implementation of the <see cref="ITaskSequenceFactory"/>.
    /// </summary>
    internal sealed class TaskSequenceFactory : ITaskSequenceFactory {

        /// <summary>
        /// Initialises a new instsance.
        /// </summary>
        /// <param name="services">The serviec provider that allow us to
        /// create the builders including all their dependencies.</param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="services"/> is <c>null</c>.</exception>
        public TaskSequenceFactory(IServiceProvider services) {
            this._services = services
                ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder CreateBuilder()
            => this._services.GetRequiredService<ITaskSequenceBuilder>();

        /// <inheritdoc />
        public async Task<ITaskSequenceDescription?> LoadDescriptionAsync(
                string path)
            => await TaskSequenceDescription.ParseAsync(path);

        #region Private fields
        private readonly IServiceProvider _services;
        #endregion
    }
}
