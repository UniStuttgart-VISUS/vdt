// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    .AddJsonFile("appsettings.json", true, true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// Collect all the services we need for bootstrapping.
var services = new ServiceCollection()
    .AddBootstrappingServices()
    .AddState()
    .Configure<BootstrappingOptions>(configuration.Bind)
    .AddLogging(o=> {
#if DEBUG
        o.AddDebug();
#endif // DEBUG

        var options = new BootstrappingOptions();
        configuration.Bind(options);

        var config = new LoggerConfiguration()
            .WriteTo.File(options.LogFile);

#if DEBUG
        config.MinimumLevel.Verbose();
#else // DEBUG
        config.MinimumLevel.Info();
#endif // DEBUG

        var logger = config.CreateLogger();

        o.AddSerilog(logger);
        o.AddSimpleConsole(f => {
            f.IncludeScopes = false;
            f.SingleLine = true;
        });
    })
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
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var drives = services.GetRequiredService<IDriveInfo>();
    var input = services.GetRequiredService<IConsoleInput>();
    var task = services.GetRequiredService<MountNetworkShare>();
    var state = services.GetRequiredService<IState>();

    task.Path = input.ReadInput(Resources.PromptDeploymentShare,
        opts.Value.DeploymentShare)!;
    task.MountPoint = input.ReadInput(Resources.PromptMountPoint,
        opts.Value.DeploymentDrive ?? drives.GetFreeDrives().Last());

    var domain = input.ReadInput(Resources.PromptDomain, opts.Value.Domain);
    var user = input.ReadInput(Resources.PromptUser, opts.Value.User);
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
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var state = services.GetRequiredService<IState>();
    state.Set(WellKnownStates.Phase, Phase.Installation);
    log.LogInformation(Resources.PersistState, opts.Value.StateFile);
    await state.SaveAsync(opts.Value.StateFile);
} catch (Exception ex) {
    log.LogError(ex, Errors.PersistState);
}

// Start the agent.
try {
    var factory = services.GetRequiredService<ICommandBuilderFactory>();
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var state = services.GetRequiredService<IState>();
    var agent = Path.Combine(state.DeploymentShare!, opts.Value.AgentPath);
    var command = factory.Run(agent)
        .DoNotWaitForProcess()
        .Build();
    log.LogInformation(Resources.StartAgent, command);
    await command.ExecuteAsync();
} catch (Exception ex) {
    log.LogCritical(ex, Errors.StartAgent);
}

log.LogInformation(Resources.BootstrapperExit);
