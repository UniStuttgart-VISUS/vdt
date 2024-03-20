// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides extension methods for adding deployment services to 
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// Adds the infrastructure to build <see cref="ICommand"/>s via the
        /// <see cref="ICommandBuilderFactory"/> to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCommands(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ICommandBuilderFactory,
                CommandBuilderFactory>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEnvironment"/> service to the service
        /// collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddEnvironment(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IEnvironment, EnvironmentService>();
            return services;
        }
    }
}
