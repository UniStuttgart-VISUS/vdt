﻿// <copyright file="ApplicationBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Application {

    /// <summary>
    /// The base class for most applications in Project Deimos. This class
    /// does repeated tasks like setting up the DI container, logging, etc.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options class used by
    /// the application.</typeparam>
    public abstract class ApplicationBase<TOptions>
            where TOptions: OptionsBase, new() {

        #region Public methods
        /// <summary>
        /// Performs the application's main logic.
        /// </summary>
        /// <returns>The exit code of the application.</returns>
        public virtual int Run() => this.RunAsync().GetAwaiter().GetResult();
        #endregion

        #region Protected constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="stateFile"></param>
        /// <param name="stateRequired"></param>
        /// <param name="logFile"></param>
        /// <param name="settingsFile"></param>
        protected ApplicationBase(string[] arguments,
                string? stateFile = null,
                bool stateRequired = false,
                string? logFile = null,
                string settingsFile = "appsettings.json") {
            ArgumentNullException.ThrowIfNull(arguments);
            ArgumentNullException.ThrowIfNull(settingsFile);

            // Prepare the application configuration.
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFile, true, true)
                .AddCommandLine(arguments)
                .AddEnvironmentVariables()
                .AddUserSecrets<TOptions>()
                .Build();

            // Populate the application options.
            this.Configuration.Bind(this.Options);

            // Populate the DI container with basic stuff.
            var provider = new ServiceCollection()
                    .Configure<TOptions>(this.Configuration.Bind)
                    .AddBootstrappingServices()
                    .AddLogging(
                        this.Configuration.GetSection("Logging"),
                        this.ConfigureLogging);

            // If we have a state file from the command line and the actual
            // constructor parameter does not override it, use it.
            if (File.Exists(this.Options.StateFile)
                    && !File.Exists(stateFile)) {
                stateFile = this.Options.StateFile;
            }

            // If we have one, add the state restored from the state file.
            // Otherwise, add blank state and restore from annotated options.
            if (stateFile != null) {
                provider.AddState(stateFile, stateRequired);
            } else {
                provider.AddState();
            }

            // Allow applications to further add services.
            this.ConfigureServices(provider, this.Configuration);

            this.Services = provider.BuildServiceProvider();

            // Get a logger for the application.
            this.Logger = this.Services.GetRequiredService<
                ILogger<ApplicationBase<TOptions>>>();

            // Restore the state from the options.
            if (stateFile == null) {
                var state = this.Services.GetRequiredService<IState>();
                // TODO: should we always do that and allow for override like in the other direction?
                this.Logger.LogInformation("Applying command line options to "
                    + "state.");
                this.Options.CopyTo(state);
            }
        }
        #endregion

        #region Protected properties
        /// <summary>
        /// Gets the configuration obtained from the command line,
        /// appsettings.json or environment variables.
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the logger for the application.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the options populated by tthe <see cref="Configuration"/>.
        /// </summary>
        protected TOptions Options { get; } = new();

        /// <summary>
        /// Gets the DI container for the application.
        /// </summary>
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Gets the application state from the DI container.
        /// </summary>
        protected IState State => this.GetRequiredService<IState>();
        #endregion

        #region Protected methods
        /// <summary>
        /// Configures logging by adding console output.
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void ConfigureLogging(ILoggingBuilder builder) {
            ArgumentNullException.ThrowIfNull(builder);
            builder.AddSimpleConsole(f => {
                f.IncludeScopes = false;
                f.SingleLine = true;
            });
        }

        /// <summary>
        /// Allows derived classes to configure additional services at
        /// construction time. The default implementation does nothing.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        protected virtual void ConfigureServices(IServiceCollection services,
            IConfiguration configuration) { }

        /// <summary>
        /// Gets a required service from <see cref="Services"/>.
        /// </summary>
        /// <typeparam name="TService">The contract of the service to be
        /// retrieved.</typeparam>
        /// <returns>An instance of the requested service.</returns>
        protected TService GetRequiredService<TService>()
                where TService : notnull
            => this.Services.GetRequiredService<TService>();

        /// <summary>
        /// Performs the application's main logic asynchronously.
        /// </summary>
        /// <returns>The exit code of the application.</returns>
        protected abstract Task<int> RunAsync();
        #endregion
    }
}
