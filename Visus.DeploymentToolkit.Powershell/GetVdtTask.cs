// <copyright file="GetVdtTask.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Management.Automation;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Powershell {

    /// <summary>
    /// Retrieves the description of a deployment task.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "VdtTask")]
    [OutputType(typeof(ITaskDescription))]
    public class GetVdtTask : Cmdlet {

        #region Public propertie
        /// <summary>
        /// Gets or sets the namesof the task to be retrieved.
        /// </summary>
        [Parameter(Position = 0,
            ValueFromPipeline = true,
            Mandatory = true,
            HelpMessage = "Specifies the name of the task to retrieve.")]
        public string Name { get; set; } = null!;
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void ProcessRecord() {
            base.ProcessRecord();
            this.WriteObject(TaskDescriptionFactory.FromType(this.Name));
        }
        #endregion
    }
}
