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
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Unattend;
using Visus.DeploymentToolkit.Workflow;
using Visus.DirectoryAuthentication;
using Visus.DirectoryAuthentication.Configuration;


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
            services.AddBootService();
            services.AddBootstrappingServices();
            services.AddDism();
            services.AddDiskManagement();
            services.AddImaging();
            services.AddLdapAuthentication(o => { });
            services.AddRegistry();
            services.AddSystemInformation();
            services.AddTasks();
            services.AddTaskSequenceDescriptionBuilder();
            services.AddTaskSequenceFactory();
            services.AddTaskSequenceStore();
            services.AddUnattendBuilder();
            services.AddUnattendCustomisations();
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
        /// Configures the <see cref="LdapOptions"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureLdap(
                this IServiceCollection services,
                IConfiguration configuration,
                string sectionName = LdapOptions.Section) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionName);
            services.Configure<LdapOptions>(configuration.GetSection(
                sectionName));
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

        /// <summary>
        /// Configures the <see cref="UnattendBuilderOptions"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureUnattendBuilder(
                this IServiceCollection services,
                IConfiguration configuration,
                string sectionName = UnattendBuilderOptions.SectionName) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionName);
            return services.Configure<UnattendBuilderOptions>(
                configuration.GetSection(sectionName));
        }

        /// <summary>
        /// Configures the <see cref="VdsOptions"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureVds(
                this IServiceCollection services,
                IConfiguration configuration,
                string sectionName = VdsOptions.SectionName) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(sectionName);
            return services.Configure<VdsOptions>(
                configuration.GetSection(sectionName));
        }

        #region Internal services
        /// <summary>
        /// Adds the <see cref="BootService"/> to <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddBootService(
                this IServiceCollection services) {
            ArgumentNullException.ThrowIfNull(services);
            services.AddSingleton<IBootService, BootService>();
            return services;
        }

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
        /// Adds a <see cref="IDiskManagement"/> to <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddDiskManagement(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.AddSingleton<VdsService, VdsService>();
                services.AddSingleton<WmiDiskService, WmiDiskService>();
                services.AddSingleton<IDiskManagement, VdsService>();
            }
            return services;
        }

        /// <summary>
        /// Adds the WIM image manipulation service to
        /// <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddImaging(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.AddSingleton<IImageServicing, ImageServicing>();
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                services.AddSingleton<IRegistry, RegistryService>();
            }
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
        /// Adds the <see cref="UnattendBuilder"/> to
        /// <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddUnattendBuilder(
                this IServiceCollection services) {
            ArgumentNullException.ThrowIfNull(services);
            services.AddSingleton<IUnattendBuilder, UnattendBuilder>();
            return services;
        }

        /// <summary>
        /// Adds the customisation steps for an unattend.xml file to
        /// <paramref name="services"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddUnattendCustomisations(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var assembly = MethodBase.GetCurrentMethod()!.DeclaringType!.Assembly;
            var steps = from t in assembly.GetTypes()
                        where t.IsAssignableTo(typeof(ICustomisation)) && !t.IsAbstract
                        select t;
            foreach (var step in steps) {
                services.AddTransient(step);
            }

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
