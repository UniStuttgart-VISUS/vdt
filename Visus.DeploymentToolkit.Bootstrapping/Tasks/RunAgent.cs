// <copyright file="RunAgent.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
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

        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(this._state.StateFile)) {
                throw new InvalidOperationException(Errors.NoStateFile);
            }

            var cmd = this._factory.Run("")
                .WithArgumentList(
                    $"--StateFile={this._state.StateFile}",
                    $"--Phase={Phase.Installation}")
                .InWorkingDirectory(this._state.WorkingDirectory)
                .DoNotWaitForProcess()
                .Build();

            this._logger.LogInformation("Running deployment agent as "
                + "\"{Command}\".", cmd);
            await cmd.ExecuteAsync();
        }

        #region Private fields
        private readonly ICommandBuilderFactory _factory;
        #endregion
    }
}
