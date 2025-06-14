// <copyright file="InvokeVdtTask.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Powershell.Properties;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Powershell {

    /// <summary>
    /// Instantiates a deployment task and invokes it.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "VdtTask")]
    public class InvokeVdtTask : Cmdlet {

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the tasks to be invoked.
        /// </summary>
        [Parameter(Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Specifies the name of the task to run.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the path to a state file that should be loaded before
        /// running the task.
        /// </summary>
        [Parameter(Position = 2,
            HelpMessage = "Specifies the path to a state file.")]
        public string? State { get; set; }
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void BeginProcessing() {
            base.BeginProcessing();

            var collection = new ServiceCollection()
                .AddDeploymentServices();

            if (string.IsNullOrWhiteSpace(this.State)) {
                this.WriteVerbose(string.Format(Resources.UseEmptyState));
                collection.AddState();
            } else {
                this.WriteVerbose(string.Format(Resources.UseStateFile,
                    this.State));
                collection.AddState(this.State);
            }

            this._services = collection.BuildServiceProvider();
        }

        /// <inheritdoc />
        protected override void ProcessRecord() {
            base.ProcessRecord();

            // Retrieve the task by its name.
            var desc = TaskDescriptionFactory.FromType(this.Name);
            var task = this._services.GetRequiredService(desc.Type) as ITask;
            if (task is null) {
                throw new ArgumentException(string.Format(Errors.TypeNotTask,
                    desc.Task));
            }

            // TODO: parameters

            // Execute the task.
            task.ExecuteAsync().Wait();
        }
        #endregion

        #region Private fields
        private IServiceProvider _services = null!;
        #endregion
    }
}
