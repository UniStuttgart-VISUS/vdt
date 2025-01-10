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


// Build the configuration from appsettings.json and the command line.
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true, true)
    .AddCommandLine(args)
    .Build();

var options = new AgentOptions();
configuration.Bind(options);

// Collect all the bootstrapping and standard services.
var services = new ServiceCollection()
    .AddBootstrappingServices()
    .AddState(options.StateFile)
    .Configure<AgentOptions>(configuration.Bind)
    .AddLogging(options.LogFile)
    .BuildServiceProvider();

// Prepare the global application log.
var log = services.GetRequiredService<ILogger<Program>>();
log.LogInformation("Project Deimos agent is starting.");
