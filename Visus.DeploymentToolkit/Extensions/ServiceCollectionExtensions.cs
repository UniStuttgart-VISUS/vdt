// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides extension methods for adding deployment services to 
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// Adds all deployment services to the given DI container, which
        /// includes all bootstrapping services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddDeploymentServices(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddBootstrappingServices();
            services.AddDiskManagement();
            services.AddRegistry();
            services.AddSystemInformation();
            services.AddWmi();
            return services;
        }

        #region Internal Services
        /// <summary>
        /// Adds the <see cref="VdsService"/> to <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddDiskManagement(
                this IServiceCollection services,
                Action<DismOptions>? options = null) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                if (options == null) {
                    options = o => { };
                }

                services.Configure(options)
                    .AddSingleton<IDiskManagement, VdsService>();
            }
            return services;
        }

        /// <summary>
        /// Adds the <see cref="RegistryService"/> to
        /// <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddRegistry(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IRegistry, RegistryService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="SystemInformationService"/> to
        /// <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddSystemInformation(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ISystemInformation,
                SystemInformationService>();
            return services;
        }

        /// <summary>
        /// Adds the WMI service to <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddWmi(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.AddSingleton<IManagementService, ManagementService>();
            }
            return services;
        }
        #endregion
    }
}
