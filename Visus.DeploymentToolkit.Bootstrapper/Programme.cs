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
using System.Linq;
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


// Perform bootstrapping.
try {
    var state = services.GetRequiredService<IState>();
    state.Set(WellKnownStates.Phase, Phase.Bootstrapping);
} catch (Exception ex) {
    log.LogError(ex, "Failed to set bootstrapping as the current state.");
}

try {
    var drives = services.GetRequiredService<IDriveInfo>();
    var input = services.GetRequiredService<IConsoleInput>();
    var task = services.GetRequiredService<MountNetworkShare>();
    var state = services.GetRequiredService<IState>();

    task.Path = input.ReadInput(Resources.PromptDeploymentShare,
        options.DeploymentShare)!;
    task.MountPoint = input.ReadInput(Resources.PromptMountPoint,
        options.DeploymentDrive ?? drives.GetFreeDrives().Last());

    var domain = input.ReadInput(Resources.PromptDomain, options.Domain);
    var user = input.ReadInput(Resources.PromptUser, options.User);
    var password = input.ReadPassword(Resources.PromptPassword);
    task.Credential = new(user, password, domain);

    await task.ExecuteAsync(state);

    state.Set(WellKnownStates.DeploymentShare, task.MountPoint);
} catch (Exception ex) {
    log.LogCritical(ex, "Failed to mount the deployment share.");
}

try {
    var state = services.GetRequiredService<IState>();
    var task = services.GetRequiredService<CreateWorkingDirectory>();
    task.Path = options.WorkingDirectory;
    await task.ExecuteAsync(state);
} catch (Exception ex) {
    log.LogCritical(ex, "Failed to prepare the working directory "
        + "\"{WorkingDirectory}\".", options.WorkingDirectory);
}

// TODO: That does not make sense. Only once the destintation disk has been prepared, a copy should be created by the agent.
//try {
//    var state = services.GetRequiredService<IState>();
//    var task = services.GetRequiredService<CopyFiles>();

//    task.Source = Path.Combine(state.DeploymentShare!, options.BinaryPath);
//    task.Destination = options.WorkingDirectory;
//    task.IsOverwrite = true;
//    task.IsRecursive = true;

//    log.LogInformation("Copying deployment agent binaries from "
//        + "\"{DeploymentShare}\" to \"{WorkingDirectory}\".",
//        task.Source,
//        task.Destination);
//    await task.ExecuteAsync(state);

//} catch (Exception ex) {
//    log.LogCritical(ex, "Failed to copy binaries from the deployment share.");
//}

//try {
//    var state = services.GetRequiredService<IState>();
//    var path = Path.Combine(state.DeploymentShare!,
//        options.TaskSequenceFolder);

//    log.LogInformation("Loading task sequences from "
//        + "\"{TaskSequenceFolder}\".", path);

//    Directory.GetFiles(path, "*.json")
//        .Select(TaskSequenceDescription.ParseAsync)
//        .Select(async taskSequence => {
//            var sequence = await taskSequence;
//            if (sequence is not null) {
//                state.Set(sequence.ID, sequence);
//            }
//        });
//} catch (Exception ex) {
//    log.LogCritical(ex, "Failed to select a task sequence to run.");
//}


// Persist the state for the agent to run next.
try {
    var state = services.GetRequiredService<IState>();
    state.Set(WellKnownStates.Phase, Phase.Installation);
    log.LogInformation(Resources.PersistState, options.StateFile);
    await state.SaveAsync(options.StateFile);
} catch (Exception ex) {
    log.LogError(ex, "Failed to persist the deployment state for the agent.");
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
