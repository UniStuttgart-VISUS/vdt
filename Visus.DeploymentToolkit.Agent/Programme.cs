// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Agent;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


// Build the configuration from appsettings.json and the command line.
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true, true)
    .AddCommandLine(args)
    .Build();

// Load the options such that we can initialise the log and copy the input
// provided by the user to the state object. Furthermore, the user might
// already provide the task sequence to run via the command line.
var options = new AgentOptions();
configuration.Bind(options);

// Collect all the bootstrapping and standard services.
var services = new ServiceCollection()
    .AddDeploymentServices()
    //.AddState(options.StateFile)
    .AddState()
    .Configure<AgentOptions>(configuration.Bind)
    .Configure<TaskSequenceStoreOptions>(configuration.GetSection(nameof(AgentOptions.TaskSequenceStore)).Bind)
    .AddLogging(options.LogFile)
    .BuildServiceProvider();

// Prepare the global application log.
var log = services.GetRequiredService<ILogger<Program>>();
log.LogInformation("Project Deimos agent is starting.");

// Assign command line and other input to the state object.
try {
    var state = services.GetRequiredService<IState>();
    options.CopyTo(state);
} catch (Exception ex) {
    log.LogError(ex, "Failed to initialise user-provided state.");
}

// Find out what task sequence we are running. If there is none specified in the
// startup options, we ask the user to provide one. If we restart, we must
// schedule the same task sequence via the command line, in which case we must
// not prompt the user to provide a different name, but continue with the one
// that was provided to us. The SelectInstallationSequence task handles all of
// this and as we are running a prelimiary task sequence of only one task, we
// execute it directly without constructing a sequence for it.
try {
    var task = services.GetRequiredService<SelectInstallationSequence>();
    await task.ExecuteAsync();
} catch (Exception ex) {
    log.LogError(ex, "Failed to select the task sequence to run.");
}

