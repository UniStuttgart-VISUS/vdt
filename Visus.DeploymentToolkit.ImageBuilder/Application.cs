// <copyright file="Application.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Application;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.ImageBuilder {

    /// <summary>
    /// Implements the image builder application.
    /// </summary>
    /// <param name="args">The arguments from the command line.</param>
    internal sealed class Application(string[] args)
            : ApplicationBase<Options>(args) {

        /// <inheritdoc/>
        protected override void ConfigureServices(IServiceCollection services,
                IConfiguration configuration) {
            base.ConfigureServices(services, configuration);
            services.ConfigureDism(configuration);
            services.ConfigureTaskSequenceStore(configuration);
            services.ConfigureUnattendBuilder(configuration);
            services.ConfigureTools(configuration);
            services.AddDeploymentServices();
        }

        /// <inheritdoc/>
        protected override async Task<int> RunAsync() {
            try {
                // Load or create the task sequence to run.
                var task = this.GetRequiredService<SelectWindowsPeSequence>();
                await task.ExecuteAsync();

                // If the previous task succeeded, the task sequence now must be
                // set in the state and we can just run it.
                this.Logger.LogInformation("Running the installation task "
                    + "sequence.");
                var state = this.GetRequiredService<IState>();
                var sequence = state.TaskSequence as ITaskSequence;
                await sequence!.ExecuteAsync(state);

                return 0;
            } catch (Exception ex) {
                this.Logger.LogCritical(ex, "Building a boot image failed.");
                return 1;
            }
        }

    }
}
