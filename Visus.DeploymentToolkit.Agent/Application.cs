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
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Agent {

    /// <summary>
    /// Implements the image builder application.
    /// </summary>
    /// <param name="args">The arguments from the command line.</param>
    internal sealed class Application(string[] args)
            : ApplicationBase<Options>(args, Configure) {

        /// <inheritdoc/>
        protected override async Task<int> RunAsync() {
            try {
                this.Logger.LogInformation("Project Deimos agent is starting.");

                this.Logger.LogInformation("Assigning user-provided state.");
                this.Options.CopyTo(this.State);

                // Find out what task sequence we are running. If there is none
                // specified in the startup options, we ask the user to provide
                // one. If we restart, we must schedule the same task sequence
                // via the command line, in which case we must not prompt the
                // user to provide a different name, but continue with the one
                // that was provided to us. The SelectInstallationSequence task
                // handles all of this and as we are running a prelimiary task
                // sequence of only one task, we execute it directly without
                // constructing a sequence for it.
                this.Logger.LogInformation("Selecting task sequence.");
                var t = this.GetRequiredService<SelectInstallationSequence>();
                await t.ExecuteAsync();

                // If the previous task succeeded, the task sequence now must be
                // set in the state and we can just run it.
                this.Logger.LogInformation("Running the installation task "
                    + "sequence.");
                var s = this.State.TaskSequence as ITaskSequence;
                await s!.ExecuteAsync(this.State);

                return 0;
            } catch (Exception ex) {
                this.Logger.LogCritical(ex, "Building a boot image failed.");
                return 1;
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
