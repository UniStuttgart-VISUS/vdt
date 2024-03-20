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
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Bootstrapper;
using Visus.DeploymentToolkit.Bootstrapper.Properties;
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
    })
    .BuildServiceProvider();

// Perform bootstrapping.
{
    var log = services.GetRequiredService<ILogger<Program>>();
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var task = services.GetRequiredService<MountNetworkShare>();
    task.Path = opts.Value.DeploymentShare;
    task.MountPoint = "a:";

    log.LogInformation(Resources.MountDeploymentShare,
        task.Path,
        task.MountPoint);
    //await task.ExecuteAsync(host.Services.GetRequiredService<IState>());
}


// Persist the state for the agent to run next.
{
    var opts = services.GetRequiredService<IOptions<BootstrappingOptions>>();
    var state = services.GetRequiredService<IState>();
    await state.SaveAsync(opts.Value.StateFile);
}
