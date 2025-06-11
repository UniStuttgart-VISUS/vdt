// <copyright file="Application.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Application;
using Visus.DeploymentToolkit.Bootstrapper.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Bootstrapper {

    /// <summary>
    /// Implements the bootstrapping application.
    /// </summary>
    /// <remarks>
    /// The Project Deimos bootstrapper is responsible for preparing the
    /// automated installation, which is performed by the agent. The
    /// bootstrapper is fairly minimal by design. It only mounts the deployment
    /// share and makes the agent and its configuration available. The reason
    /// for this design is that the boostrapper needs to live in the Windows PE
    /// image. It should therefore be as small as possible as the image is
    /// transferred via TFTP. Furhtermore, we want to minimise the need to
    /// rebuild the Windows PE image as far as possible. Therefore, changes the
    /// the configuration of the agent should not require a rebuild as only the
    /// boostrapper is in the image.
    /// </remarks>
    /// <param name="args">The arguments from the command line.</param>
    internal sealed class Application(string[] args)
            : ApplicationBase<Options>(args, (_, _) => { }) {

        /// <inheritdoc />
        protected override async Task<int> RunAsync() {
            this.Logger.LogInformation("Preparing bootstrapping task sequence.");
            var builder = this.GetRequiredService<ITaskSequenceBuilder>()
                .ForPhase(Phase.Bootstrapping)
                .Add<SetInputLocale>(t => {
                    t.InputLocale = this.Options.InputLocale;
                })
                .Add<Delay>(t => {
                    t.Duration = TimeSpan.FromSeconds(2);
                    t.Reason = Resources.WaitForLog;
                })
                .Add<MountDeploymentShare>()
                .Add<CreateDirectory>(t => {
                    t.Path = this.Options.WorkingDirectory;
                    t.State = WellKnownStates.WorkingDirectory;
                })
                .Add<PersistState>(t => t.Path = this.Options.StatePath)
                .Add<RunAgent>((t, s) => {
                    ArgumentNullException.ThrowIfNull(s.AgentPath);
                    ArgumentNullException.ThrowIfNull(s.DeploymentDirectory);

                    if (!string.IsNullOrEmpty(this.Options.TaskSequenceStore)) {
                        t.TaskSequenceStore = this.Options.TaskSequenceStore;
                    }
                });
            //.Add<CopyFiles>(services, t => t.Source = options.LogFile)
            var taskSequence = builder.Build();

            // Run the task sequence, which will start the deployment agent from
            // the share.
            this.Logger.LogInformation("Running bootstrapping task sequence.");
            try {
                await taskSequence.ExecuteAsync(this.State);
                this.Logger.LogInformation("The boostrapper is exiting.");
                return 0;

            } catch (Exception ex) {
                this.Logger.LogCritical(ex, "The bootstrapping task sequence "
                    + "failed.");
                return -1;
            }
        }
    }
}
