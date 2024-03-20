// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Visus.DeploymentToolkit.Bootstrapper;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


// Build the configuration from appsettings and the command line.
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddCommandLine(args)
    .Build();

// Collect all the services we need for bootstrapping.
var services = new ServiceCollection()
    .AddBootstrappingServices()
    .AddState()
    .Configure<BootstrappingOptions>(o => {
        configuration.Bind(o);
    })
    .AddLogging(o=> {
        o.AddConsole();
    })
    .BuildServiceProvider();

// Perform bootstrapping.
{
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var task = services.GetRequiredService<MountNetworkShare>();
    task.Path = opts.Value.DeploymentShare;
    task.MountPoint = "a:";
    //await task.ExecuteAsync(host.Services.GetRequiredService<IState>());
}


// Persist the state for the agent to run next.
{
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var state = services.GetRequiredService<IState>();
    await state.SaveAsync(opts.Value.StateFile);
}
