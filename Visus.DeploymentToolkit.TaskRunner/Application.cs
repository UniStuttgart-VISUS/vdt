// <copyright file="Application.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;
using Visus.DeploymentToolkit.Application;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.TaskRunner.Properties;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.TaskRunner {

    /// <summary>
    /// Hosts the task runninger application.
    /// </summary>
    internal sealed class Application(string[] args)
            : ApplicationBase<Options>(args, Configure) {

        /// <inheritdoc />
        protected override async Task<int> RunAsync() {
            try {
#if false
                var task = this.Services.GetRequiredService<InstallWindowsPe>();
                task.SavePath = "installwinpe.json";
                await task.ExecuteAsync();
                return 0;
#endif
                if (string.IsNullOrWhiteSpace(this.Options.Task)) {
                    throw new ArgumentException(Resources.ErrorNoTask);
                }

                this.Logger.LogInformation("Obtaining task {Task} ...",
                    this.Options.Task);
                var pattern = new Regex($"{Regex.Escape(this.Options.Task)}$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Type? taskType = null;

                var self = Assembly.GetExecutingAssembly();
                foreach (var a in self.GetReferencedAssemblies()) {
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
                    throw new ArgumentException(string.Format(
                        Resources.ErrorTaskNotFound,
                        this.Options.Task));
                }

                this.Logger.LogInformation("Instantiating {Task} ...",
                    taskType);

                var task = this.Services.GetRequiredService(taskType) as ITask;
                if (task == null) {
                    throw new ArgumentException(string.Format(
                        Resources.ErrorTaskInstantiation,
                        taskType));
                }

                this.Logger.LogInformation("Applying parameters to " +
                    "{Task} ...", task.Name);
                var parameters = this.Configuration.GetSection("Parameters");
                task.CopyFrom(parameters, true);

                this.Logger.LogInformation("Running {Task} ...", task.Name);
                await task.ExecuteAsync();
                return 0;
            } catch (Exception ex) {
                this.Logger.LogCritical(ex, "Failed to run task.");
                return -1;
            }
        }

        #region Private methods
        private static void Configure(IServiceCollection services,
                IConfiguration config) {
            services.ConfigureDism(config);
            services.ConfigureTaskSequenceStore(config);
            services.AddDeploymentServices();
        }
        #endregion
    }
}
