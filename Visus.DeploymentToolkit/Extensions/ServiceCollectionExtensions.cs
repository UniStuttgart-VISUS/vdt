// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides extension methods for adding the core deployment services to
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
            services.AddDism();
            services.AddDiskManagement();
            services.AddRegistry();
            services.AddSystemInformation();
            services.AddTasks();
            services.AddTaskSequenceDescriptionBuilder();
            services.AddTaskSequenceFactory();
            services.AddTaskSequenceStore();
            services.AddWmi();
            return services;
        }

        /// <summary>
        /// Configures the <see cref="DismOptions"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ConfigureDism(
                this IServiceCollection services,
                IConfiguration configuration,
                string sectionName = DismOptions.SectionName) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionName);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.Configure<DismOptions>(configuration.GetSection(
                    sectionName));
            }
            return services;
        }


        /// <summary>
        /// Configures the <see cref="TaskSequenceStoreOptions"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ConfigureTaskSequenceStore(
                this IServiceCollection services,
                IConfiguration configuration,
                string sectionName = TaskSequenceStoreOptions.SectionName) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionName);
            return services.Configure<TaskSequenceStoreOptions>(
                configuration.GetSection(sectionName));
        }

        #region Internal services
        /// <summary>
        /// Adds the <see cref="DismScope"/> to <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddDism(
                this IServiceCollection services) {
            ArgumentNullException.ThrowIfNull(services);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.AddSingleton<IDismScope, DismScope>();
            }
            return services;
        }


        /// <summary>
        /// Adds the <see cref="VdsService"/> to <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddDiskManagement(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.AddSingleton<IDiskManagement, VdsService>();
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
        /// Adds the main deployment tasks implemented in this library, which
        /// does <i>not</i> include the bootstrapping tasks.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddTasks(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var assembly = MethodBase.GetCurrentMethod()!.DeclaringType!.Assembly;
            var tasks = from t in assembly.GetTypes()
                        where t.IsAssignableTo(typeof(ITask)) && !t.IsAbstract
                        select t;
            foreach (var task in tasks) {
                services.AddTransient(task);
            }

            return services;
        }

        /// <summary>
        /// Adds the <see cref="ITaskSequenceDescriptionBuilder"/> for creating
        /// task sequence descriptions from code.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddTaskSequenceDescriptionBuilder(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddTransient<ITaskSequenceDescriptionBuilder,
                TaskSequenceDescriptionBuilder>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ITaskSequenceFactory"/> for creating task
        /// sequences from code.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddTaskSequenceFactory(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddTransient<ITaskSequenceFactory, TaskSequenceFactory>();
            return services;
        }

        /// <summary>
        /// Adds the JSON-based <see cref="TaskSequenceStore"/> to
        /// <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddTaskSequenceStore(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ITaskSequenceStore>(s => {
                var o = s.GetRequiredService<IOptions<TaskSequenceStoreOptions>>();
                var l = s.GetRequiredService<ILogger<TaskSequenceStore>>();
                return new TaskSequenceStore(o, l);
            });
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
