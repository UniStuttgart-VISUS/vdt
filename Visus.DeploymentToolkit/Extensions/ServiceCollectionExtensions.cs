// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.DependencyInjection;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides extension methods for adding deployment services to 
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// Adds all deployment services to the given DI container.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddDeploymentServices(
                this IServiceCollection that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));

            that.AddTransient<InjectDrivers>();
            that.AddTransient<RunCommand>();
            that.AddSingleton<IEnvironment, EnvironmentService>();

            return that;
        }
    }
}
