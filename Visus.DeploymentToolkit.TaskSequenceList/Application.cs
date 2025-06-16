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
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.TaskSequenceList {

    /// <summary>
    /// Hosts the task runninger application.
    /// </summary>
    internal sealed class Application(string[] args)
            : ApplicationBase<Options>(args) {

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection services,
                IConfiguration configuration) {
            base.ConfigureServices(services, configuration);
            services.ConfigureTaskSequenceStore(configuration);
            services.AddDeploymentServices();
        }

        /// <inheritdoc />
        protected override async Task<int> RunAsync() {
            try {
                var store = this.Services.GetRequiredService<
                    ITaskSequenceStore>();

                foreach (var s in await store.GetTaskSequencesAsync()) {
                    Console.WriteLine($"{s.ID}: {s.Name} ({s.Phase})");

                    if (!string.IsNullOrWhiteSpace(s.Description)) {
                        Console.WriteLine(s.Description);
                    }

                    Console.WriteLine();

                    foreach (var t in s.Tasks) {
                        Console.WriteLine($"{t.Task}");
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                }

                return 0;
            } catch (Exception ex) {
                this.Logger.LogCritical(ex, "Listing tasks failed.");
                return -1;
            }
        }
    }
}
