// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart. Alle Rechte vorbehalten.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Agent;
using Visus.DeploymentToolkit.Extensions;


var builder = Host.CreateDefaultBuilder(args);

// Configure application options from either a file or from the command line.
builder.ConfigureAppConfiguration(o => {
    o.AddJsonFile("appsettings.json", true);
    o.AddCommandLine(args);
});

// Configure logging.
builder.ConfigureLogging(o => {
    o.AddConsole();
    o.AddDebug();
});

// Configure the service services.
builder.ConfigureServices((c, s) => {
    //var config = new Configuration();
    //c.Configuration.Bind(config);

    s.Configure<Configuration>(c.Configuration);

    s.AddDeploymentServices();

    s.AddHostedService<Agent>();
});

// Run the deployment agent.
builder.RunConsoleAsync().Wait();
