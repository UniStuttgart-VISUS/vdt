// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.ImageBuilder {

    /// <summary>
    /// Entry point of the image builder application that is used to service the
    /// WinPE image to hold the bootstrapper.
    /// </summary>
    internal class Programme {

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        internal static async Task Main(string[] args) {
            var options = new ApplicationOptions();

            new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", true, true)
                    .AddCommandLine(args)
                    .Build()
                    .Bind(options);

            var services = new ServiceCollection()
                .AddDeploymentServices()
                .AddLogging(o => { o.AddConsole(); })
                .AddState()
                .BuildServiceProvider();

            var log = services.GetRequiredService<ILogger<Programme>>();

            log.LogInformation("Preparing Windows PE task sequence.");
            var taskSequence = services.GetRequiredService<ITaskSequenceBuilder>()
                .ForPhase(Phase.PreinstalledEnvironment)
                .Add<CopyWindowsPe>()
                .Add<CreateWindowsPeIso>()
                .Build();

            log.LogInformation("Running Windows PE task sequence.");
            try {
                var state = services.GetRequiredService<IState>();
                await taskSequence.ExecuteAsync(state);
            } catch (Exception ex) {
                log.LogCritical(ex, "The Windows PE task sequence failed.");
            }
        }
    }
}
