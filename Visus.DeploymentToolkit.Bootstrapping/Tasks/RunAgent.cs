// <copyright file="RunAgent.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Runs the deployment agent.
    /// </summary>
    public class RunAgent : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="factory"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RunAgent(IState state,
                ICommandBuilderFactory factory,
                ILogger<RunAgent> logger)
                : base(state,logger) {
            this._factory = factory
                ?? throw new ArgumentNullException(nameof(factory));
            this.Name = Resources.RunAgent;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or set the location of the task sequence store.
        /// </summary>
        public string TaskSequenceStore { get; set; } = Layout.TaskSequencePath;
        #endregion

        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(this._state.AgentPath)) {
                throw new InvalidOperationException(Errors.NoAgent);
            }
            if (string.IsNullOrEmpty(this._state.StateFile)) {
                throw new InvalidOperationException(Errors.NoStateFile);
            }

            // If the agent path is relative, we assume it to be relative to the
            // deployment share.
            if (!Path.IsPathRooted(this._state.AgentPath)) {
                this._logger.LogTrace("Resolving agent path {Path}.",
                    this._state.AgentPath);
                if (Directory.Exists(this._state.DeploymentDirectory)) {
                    this._state.AgentPath = Path.Combine(
                        this._state.DeploymentDirectory,
                        this._state.AgentPath);

                } else if (Directory.Exists(this._state.DeploymentShare)) {
                    this._state.AgentPath = Path.Combine(
                        this._state.DeploymentShare,
                        this._state.AgentPath);
                }

                this._logger.LogTrace("The deployment agent is assumed to "
                    + "be at {Path}.", this._state.AgentPath);
            }

            if (!Directory.Exists(this.TaskSequenceStore)) {
                this._logger.LogTrace("Resolving task sequence store {Store}.",
                    this.TaskSequenceStore);

                if (Directory.Exists(this._state.DeploymentDirectory)) {
                    this.TaskSequenceStore = Path.Combine(
                        this._state.DeploymentDirectory,
                        this.TaskSequenceStore);

                } else if (Directory.Exists(this._state.DeploymentShare)) {
                    this.TaskSequenceStore = Path.Combine(
                        this._state.DeploymentShare,
                        this.TaskSequenceStore);
                }

                this._logger.LogTrace("Task sequence store is assumed to "
                    + "be at {Path}.", this.TaskSequenceStore);
            }

            // Note: we must set the phase and the progress here via the command
            // line, because the agent cannot reset the progress on its own as
            // it might restart the machine and therefore cannot now whether it
            // was invoked after a restart or for the first time from this
            // bootstrapper.
            var cmd = this._factory.Run(this._state.AgentPath)
                .WithArgumentList(
                    $"--StateFile={this._state.StateFile}",
                    $"--Phase={Phase.Installation}",
                    $"--TaskSequenceStore:Path={this.TaskSequenceStore}",
                    $"--Progress=0")
                .InWorkingDirectory(this._state.WorkingDirectory)
                .DoNotWaitForProcess()
                .Build();

            this._logger.LogInformation("Running deployment agent as "
                + "{Command}.", cmd);
            await cmd.ExecuteAsync();
        }

        #region Private fields
        private readonly ICommandBuilderFactory _factory;
        #endregion
    }
}
