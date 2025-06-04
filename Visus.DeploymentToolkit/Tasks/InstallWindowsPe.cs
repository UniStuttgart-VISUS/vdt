// <copyright file="InstallWindowsPe.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Sets a task sequence as the active one that installs the boot image that
    /// Windows PE was booted from on the hard disk.
    /// </summary>
    /// <remarks>
    /// This task is used for debugging purposes only. It has no practical use
    /// for any real deployment scenario because no one would want to install
    /// the pre-installed environment. We authored this task to test the
    /// deployment agent in an early stage of development where not all tasks
    /// required for a full deployment were implemented yet.
    /// </remarks>
    [SupportsPhase(Phase.Installation)]
    public sealed class InstallWindowsPe : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="tasks"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InstallWindowsPe(IState state,
                ITaskSequenceFactory tasks,
                ILogger<InstallWindowsPe> logger)
                : base(state, logger) {
            this._tasks = tasks
                ?? throw new ArgumentNullException(nameof(tasks));
            this.Name = Resources.InstallWindowsPe;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets a path where the task sequence is optionally saved to.
        /// </summary>
        public string? SavePath { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this._logger.LogInformation("Creating a task sequence for "
                + "installing Windows PE on a system.");

            this._state.TaskSequence = this._tasks.CreateBuilder()
                .ForPhase(Phase.PreinstalledEnvironment)
                .Add<SelectInstallDisk>()
                .Add<PartitionFormatDisk>()
                .Add<ApplyImage>((t, s) => {
                    t.Image = Path.Combine("sources", "boot.wim");
                    t.ImageIndex = 1;
                })
                .Add<RestoreBootConfiguration>()
                .Add<RebootMachine>()
                .Build();

            if (!string.IsNullOrEmpty(this.SavePath)) {
                this._logger.LogInformation("Saving task sequence to "
                    + "{Path}.", this.SavePath);
                var desc = TaskSequenceDescription.FromTaskSequence(
                    (ITaskSequence) this._state.TaskSequence);
                desc.ID = "_WINPE";
                desc.Phase = Phase.Installation;
                desc.Name = this.Name;
                desc.Description = Resources.InstallWindowsPeDescription;
                await desc.SaveAsync(this.SavePath);
            }
        }
        #endregion

        #region Private fields
        private ITaskSequenceFactory _tasks;
        #endregion
    }

}
