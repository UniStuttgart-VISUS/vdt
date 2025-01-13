// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Visus.DeploymentToolkit;
using Visus.DeploymentToolkit.Bootstrapper.Properties;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;

// The Project Deimos bootstrapper is responsible for preparing the automated
// installation, which is performed by the agent. The bootstrapper is fairly
// minimal by design. It only mounts the deployment share and makes the agent
// and its configuration available. The reason for this design is that the
// boostrapper needs to live in the Windows PE image. It should therefore be
// as small as possible as the image is transferred via TFTP. Furhtermore, we
// want to minimise the need to rebuild the Windows PE image as far as
// possible. Therefore, changes the the configuration of the agent should not
// require a rebuild as only the boostrapper is in the image.


// Build the configuration from appsettings.json and the command line.
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true, true)
    .AddCommandLine(args)
    .Build();

var options = new BootstrappingOptions();
configuration.Bind(options);

// Collect all the services we need for bootstrapping.
var services = new ServiceCollection()
    .Configure<BootstrappingOptions>(configuration.Bind)
    .AddBootstrappingServices()
    .AddState()
    .AddLogging(options.LogFile)
    .BuildServiceProvider();


// Prepare the global application log.
var log = services.GetRequiredService<ILogger<Program>>();
log.LogInformation("Project Deimos is bootstrapping.");


// Perform bootstrapping by building and executing the bootstrapping task
// sequence.
try {
    var state = services.GetRequiredService<IState>();
    state.Phase = Phase.Bootstrapping;
    state.WorkingDirectory = options.WorkingDirectory;
} catch (Exception ex) {
    log.LogError(ex, "Failed to set bootstrapping as the current state.");
}

log.LogInformation("Preparing bootstrapping task sequence.");
var taskSequenceBuilder = services.GetRequiredService<ITaskSequenceBuilder>()
    .ForPhase(Phase.Bootstrapping)
    .Add<MountDeploymentShare>(services, t => options.CopyTo(t.Options))
    .Add<CreateWorkingDirectory>(services)
    .Add<PersistState>(services, t => t.Path = options.StatePath);
var taskSequence = taskSequenceBuilder.Build();


log.LogInformation("Running bootstrapping task sequence.");
{
    var state = services.GetRequiredService<IState>();
    await taskSequence.ExecuteAsync(state);
}


// Start the agent.
try {
    var factory = services.GetRequiredService<ICommandBuilderFactory>();
    var state = services.GetRequiredService<IState>();
    var agent = Path.Combine(options.DeploymentShare,
        options.BinaryPath,
        options.AgentPath);
    var command = factory.Run(agent)
        .WithArgumentList($"--DeploymentShare={state.DeploymentShare}",
            $"--StateFile={options.StateFile}",
            $"--Phase={Phase.Installation}")
        .DoNotWaitForProcess()
        .Build();
    log.LogInformation(Resources.StartAgent, command);
    await command.ExecuteAsync();
} catch (Exception ex) {
    log.LogCritical(ex, "Failed to start the deployment agent.");
}

log.LogInformation("The boostrapper is exiting.");
