// <copyright file="SelectInstallationSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that prompts the user to select a task sequence from the store.
    /// Once this task has completed successfully, the
    /// <see cref="IState.TaskSequence"/> will be an <see cref="ITaskSequence"/>
    /// object that performs the installation.
    /// </summary>
    public sealed class SelectInstallationSequence : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="store"></param>
        /// <param name="input"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectInstallationSequence(IState state,
                ITaskSequenceStore store,
                IConsoleInput input,
                ILogger<SelectInstallationSequence> logger)
                : base(state, logger) {
            this._input = input
                ?? throw new ArgumentNullException(nameof(input));
            this._store = store
                ?? throw new ArgumentNullException(nameof(store));
        }
        #endregion

        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            if (this.IsNoTaskSequence) {
                this._logger.LogInformation("Obtaining the installation task "
                    + "sequence from the user.");

                var options = (from t in await this._store.GetTaskSequencesAsync()
                               where t.Phase == Phase.Installation
                               select t).ToArray();
                var option = this._input.Select(Resources.PromptTaskSequence,
                    options.Select(o => $"{o.ID} ({o.Name})"));

                if (option < 0) {
                    throw new InvalidOperationException(
                        Errors.EmptySequenceStore);
                }

                this._state.TaskSequence = options[option].ID;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // At this point, we should have a valid task sequence configured in
            // the state object, for which we can get the description.
            throw new NotImplementedException("TODO");
            //var description = await this._store.GetTaskSequenceAsync(
            //    this._state.TaskSequence as string);
        }

        #region Private properties
        /// <summary>
        /// Gets whether there is no task sequence or reference to a task
        /// sequence in the state.
        /// </summary>
        private bool IsNoTaskSequence
            => (this._state.TaskSequence == null)
            || ((this._state.TaskSequence is string n)
            && string.IsNullOrWhiteSpace(n));
        #endregion

        #region Private fields
        private readonly IConsoleInput _input;
        private readonly ITaskSequenceStore _store;
        #endregion
    }
}
