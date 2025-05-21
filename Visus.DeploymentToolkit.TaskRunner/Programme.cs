// <copyright file="Programme.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;
using Visus.DeploymentToolkit;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.TaskRunner.Properties;
using Visus.DeploymentToolkit.Tasks;


var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true, true)
    .AddCommandLine(args)
    .Build();

var options = new ApplicationOptions();
config.Bind(options);

var services = new ServiceCollection()
    .AddDeploymentServices()
    .AddLogging(options.LogFile)
    .ConfigureDism(config)
    .ConfigureTaskSequenceStore(config)
    .AddState(options.StateFile)
    .BuildServiceProvider();

var log = services.GetRequiredService<ILogger<Program>>();

try {
    if (string.IsNullOrWhiteSpace(options.Task)) {
        throw new ArgumentException(Resources.ErrorNoTask);
    }

    log.LogInformation("Obtaining task \"{Task}\" ...", options.Task);
    var pattern = new Regex($"{Regex.Escape(options.Task)}$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    Type? taskType = null;

    foreach (var a in Assembly.GetExecutingAssembly().GetReferencedAssemblies()) {
        var assembly = Assembly.Load(a);
        taskType = (from t in assembly.GetTypes()
                    where typeof(ITask).IsAssignableFrom(t) && !t.IsAbstract
                    where pattern.IsMatch(t.Name)
                    select t).SingleOrDefault();
        if (taskType != null) {
            break;
        }
    }

    if (taskType == null) {
        throw new ArgumentException(string.Format(Resources.ErrorTaskNotFound,
            options.Task));
    }

    log.LogInformation("Instantiating \"{Task}\" ...", taskType);

    var task = services.GetRequiredService(taskType) as ITask;
    if (task == null) {
        throw new ArgumentException(string.Format(
            Resources.ErrorTaskInstantiation,
            taskType));
    }

    log.LogInformation("Applying parameters to \"{Task}\" ...", task.Name);
    var parameters = config.GetSection("Parameters");
    task.CopyFrom(parameters);

    log.LogInformation("Running \"{Task}\" ...", task.Name);
    await task.ExecuteAsync();

} catch (Exception ex) {
    log.LogError(ex, "Failed to run task.");
}
