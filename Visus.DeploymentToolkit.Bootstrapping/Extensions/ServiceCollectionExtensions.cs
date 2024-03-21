// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides extension methods for adding deployment services to 
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions {

        #region Public methods
        /// <summary>
        /// Adds all services exported by the bootstrapping library.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="services"/> is <c>null</c>.</exception>
        public static IServiceCollection AddBootstrappingServices(
                this IServiceCollection services) {
            services.AddBootstrappingTasks();
            services.AddCommands();
            services.AddConsoleInput();
            services.AddDriveInfo();
            services.AddEnvironment();
            return services;
        }

        /// <summary>
        /// Configures our own logging to the console and to a log file (and to
        /// diverse debug outputs).
        /// </summary>
        /// <param name="services"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLogging(
                this IServiceCollection services,
                string file) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = file ?? throw new ArgumentNullException(nameof(file));

            services.AddLogging(o => {
#if DEBUG
                o.AddDebug();
#endif // DEBUG

                var config = new LoggerConfiguration().WriteTo.File(file);
#if DEBUG
                config.MinimumLevel.Verbose();
#else // DEBUG
                config.MinimumLevel.Info();
#endif // DEBUG

                o.AddSerilog(config.CreateLogger());
                o.AddSimpleConsole(f => {
                    f.IncludeScopes = false;
                    f.SingleLine = true;
                });
            });

            return services;
        }

        public static IServiceCollection AddLogging(
                this IServiceCollection services,
                Func<string> file)
            => services.AddLogging(file());

        /// <summary>
        /// Adds <see cref="IState"/> to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddState(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IState, State>();
            return services;
        }

        /// <summary>
        /// Adds <see cref="IState"/> to the service collection and restores
        /// it from the given JSON file.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="stateFile"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddState(
                this IServiceCollection services,
                string stateFile) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = stateFile ?? throw new ArgumentNullException(nameof(stateFile));

            services.AddSingleton<IState>(s => {
                var logger = s.GetRequiredService<ILogger<State>>();
                var state = new State(logger);
                state.LoadAsync(stateFile).Wait();
                return state;
            });

            return services;
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Adds the tasks exported by the bootstrapping library as transient
        /// services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="services"/> is <c>null</c>.</exception>
        internal static IServiceCollection AddBootstrappingTasks(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddTransient<CopyFiles>();
            services.AddTransient<MountNetworkShare>();
            services.AddTransient<RunCommand>();
            return services;
        }

        /// <summary>
        /// Adds the infrastructure to build <see cref="ICommand"/>s via the
        /// <see cref="ICommandBuilderFactory"/> to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="services"/> is <c>null</c>.</exception>
        internal static IServiceCollection AddCommands(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ICommandBuilderFactory,
                CommandBuilderFactory>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IConsoleInput"/> service to the service
        /// collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddConsoleInput(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IConsoleInput, ConsoleInputService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDriveInfo"/> service to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddDriveInfo(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IDriveInfo, DriveInfoService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IEnvironment"/> service to the service
        /// collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddEnvironment(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IEnvironment, EnvironmentService>();
            return services;
        }
        #endregion
    }
}
