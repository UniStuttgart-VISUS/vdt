// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Visus.DeploymentToolkit;
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

// Load the options such that we can initialise the log and copy the input
// provided by the user to the state object.
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
    options.CopyTo(state);
    state.Phase = Phase.Bootstrapping;
} catch (Exception ex) {
    log.LogError(ex, "Failed to initialise user-provided state.");
}

log.LogInformation("Preparing bootstrapping task sequence.");
var taskSequenceBuilder = services.GetRequiredService<ITaskSequenceBuilder>()
    .ForPhase(Phase.Bootstrapping)
    .Add<MountDeploymentShare>()
    .Add<CreateDirectory>(t => {
        t.Path = options.WorkingDirectory;
        t.State = WellKnownStates.WorkingDirectory;
    })
    .Add<PersistState>(t => t.Path = options.StatePath)
    .Add<RunAgent>();
    //.Add<CopyFiles>(services, t => t.Source = options.LogFile)
var taskSequence = taskSequenceBuilder.Build();


// Run the task sequence, which will start the deployment agent from the share.
log.LogInformation("Running bootstrapping task sequence.");
try {
    var state = services.GetRequiredService<IState>();
    await taskSequence.ExecuteAsync(state);
} catch (Exception ex) {
    log.LogCritical(ex, "The bootstrapping task sequence failed.");
}

log.LogInformation("The boostrapper is exiting.");
