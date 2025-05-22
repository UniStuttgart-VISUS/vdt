// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.ImageBuilder;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true, true)
    .AddCommandLine(args)
    .Build();

var services = new ServiceCollection()
    .AddDeploymentServices()
    .AddLogging(o => { o.AddConsole(); })
    .Configure<ApplicationOptions>(config.Bind)
    .ConfigureDism(config)
    .ConfigureTaskSequenceStore(config)
    .AddState()
    .BuildServiceProvider();

var log = services.GetRequiredService<ILogger<Program>>();
var options = services.GetRequiredService<IOptions<ApplicationOptions>>().Value;

// Inject the configuration stuff from the application options.
{
    var state = services.GetRequiredService<IState>();
    state.DeploymentShare = options.DeploymentShare;
    log.LogInformation("Using deployment share \"{DeploymentShare}\".",
        state.DeploymentShare);
}

// Load or create the task sequence to run.
try {
    var task = services.GetRequiredService<SelectWindowsPeSequence>();
    await task.ExecuteAsync();
} catch (Exception ex) {
    log.LogError(ex, "Failed to select the task sequence to run.");
    return;
}

// If the previous task succeeded, the task sequence now must be set in the
// state and we can just run it.
try {
    log.LogInformation("Running the installation task sequence.");
    var state = services.GetRequiredService<IState>();
    var sequence = state.TaskSequence as ITaskSequence;
    await sequence!.ExecuteAsync(state);
} catch (Exception ex) {
    log.LogError(ex, "Failed to execute the task sequence.");
    return;
}
