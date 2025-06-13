// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using Visus.DeploymentToolkit.Compliance;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


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
        /// <remarks>
        /// This method performs &quot;try add&quot; for all services, so it is
        /// safe to call it multiple times.
        /// </remarks>
        /// <param name="services"></param>
        /// <returns><paramref name="services"/>.</returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="services"/> is <c>null</c>.</exception>
        public static IServiceCollection AddBootstrappingServices(
                this IServiceCollection services) {
            services.AddBootstrappingTasks();
            services.AddCommands();
            services.AddConsoleInput();
            services.AddCopyService();
            services.AddDirectoryService();
            services.AddDriveInfo();
            services.AddEnvironment();
            services.AddSessionSecurity();
            services.AddTaskSequenceBuilder();
            return services;
        }

        /// <summary>
        /// Configures our own logging to the console and to a log file (and to
        /// diverse debug outputs).
        /// </summary>
        /// <param name="services">The service collection to add logging to.
        /// </param>
        /// <param name="config">The logging configuration section.</param>
        /// <returns><paramref name="services"/> after injection.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLogging(
                this IServiceCollection services,
                IConfiguration config) {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(config);

            // Configure the log redaction to be enabled in the next step.
            services.AddRedaction(o => {
                o.SetRedactor<ErasingRedactor>([
                    new(Classification.SensitiveData)
                ]);
            });

            // Configure the logging itself.
            services.AddLogging(o => {
                o.EnableRedaction();

#if DEBUG
                o.AddDebug();
#endif // DEBUG

                if (config["PathFormat"] is not null) {
                    // If we specify an invalid path format, Serilog will fail.
                    o.AddFile(config);
                }

                o.AddSimpleConsole(f => {
                    f.IncludeScopes = false;
                    f.SingleLine = true;
                });
            });

            return services;
        }

        /// <summary>
        /// Adds logging using the file provided by the given function.
        /// </summary>
        /// <param name="services">The service collection to add logging to.
        /// </param>
        /// <param name="config">A callback providing the logging configuration.
        /// </param>
        /// <returns><paramref name="services"/> after injection.</returns>
        public static IServiceCollection AddLogging(
                this IServiceCollection services,
                Func<IConfiguration> config)
            => services.AddLogging(config());

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
        /// <param name="required">If <c>true</c>, it is a fatal error if the
        /// state could not be restored.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddState(
                this IServiceCollection services,
                string stateFile,
                bool required = false) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = stateFile ?? throw new ArgumentNullException(nameof(stateFile));

            services.AddSingleton<IState>(s => {
                var logger = s.GetRequiredService<ILogger<State>>();
                var state = new State(logger);

                try {
                    state.LoadAsync(stateFile).Wait();
                } catch (Exception ex) {
                    var msg = string.Format(
                        Errors.RestoringStateFailed,
                        stateFile);

                    if (required) {
                        logger.LogError(ex, msg);
                        throw new AggregateException(msg, ex);
                    } else {
                        logger.LogWarning(ex, msg);
                    }
                }
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

            var assembly = MethodBase.GetCurrentMethod()!.DeclaringType!.Assembly;
            var tasks = from t in assembly.GetTypes()
                        where t.IsAssignableTo(typeof(ITask)) && !t.IsAbstract
                        select t;
            foreach (var task in tasks) {
                services.TryAddTransient(task);
            }

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
            services.TryAddSingleton<ICommandBuilderFactory,
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
            services.TryAddSingleton<IConsoleInput, ConsoleInputService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICopy"/> service to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddCopyService(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.TryAddSingleton<ICopy, CopyService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDirectory"/> service to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddDirectoryService(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.TryAddSingleton<IDirectory, DirectoryService>();
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
            services.TryAddSingleton<IDriveInfo, DriveInfoService>();
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
            services.TryAddSingleton<IEnvironment, EnvironmentService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ISessionSecurity"/> service to the service
        /// collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddSessionSecurity(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.TryAddSingleton<ISessionSecurity,
                SessionSecurityService>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ITaskSequenceBuilder"/> for creating task
        /// sequences from code.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static IServiceCollection AddTaskSequenceBuilder(
                this IServiceCollection services) {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.TryAddTransient<ITaskSequenceBuilder,
                TaskSequenceBuilder>();
            return services;
        }
        #endregion
    }
}
