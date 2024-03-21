// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using Visus.DeploymentToolkit.Bootstrapper;
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
    .AddBootstrappingServices()
    .AddState()
    .Configure<BootstrappingOptions>(configuration.Bind)
    .AddLogging(options.LogFile)
    .BuildServiceProvider();


// Prepare the global application log.
var log = services.GetRequiredService<ILogger<Program>>();
log.LogInformation(Resources.BootstrapperStart);


// Perform bootstrapping.
try {
    var state = services.GetRequiredService<IState>();
    state.Set(WellKnownStates.Phase, Phase.Bootstrapping);
} catch (Exception ex) {
    log.LogError(ex, Errors.SetBoostrappingState);
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

    log.LogInformation(Resources.MountDeploymentShare,
        task.Path,
        task.MountPoint);
    await task.ExecuteAsync(state);

    state.Set(WellKnownStates.DeploymentShare, task.MountPoint);
} catch (Exception ex) {
    log.LogCritical(ex, Errors.MountDeploymentShare);
}

// Persist the state for the agent to run next.
try {
    var state = services.GetRequiredService<IState>();
    state.Set(WellKnownStates.Phase, Phase.Installation);
    log.LogInformation(Resources.PersistState, options.StateFile);
    await state.SaveAsync(options.StateFile);
} catch (Exception ex) {
    log.LogError(ex, Errors.PersistState);
}

// Start the agent.
try {
    var factory = services.GetRequiredService<ICommandBuilderFactory>();
    var state = services.GetRequiredService<IState>();
    var agent = Path.Combine(state.DeploymentShare!, options.AgentPath);
    var command = factory.Run(agent)
        .WithArgumentList($"--DeploymentShare={state.DeploymentShare}",
            $"--StateFile={options.StateFile}",
            $"--Phase={Phase.Installation}")
        .DoNotWaitForProcess()
        .Build();
    log.LogInformation(Resources.StartAgent, command);
    await command.ExecuteAsync();
} catch (Exception ex) {
    log.LogCritical(ex, Errors.StartAgent);
}

log.LogInformation(Resources.BootstrapperExit);
