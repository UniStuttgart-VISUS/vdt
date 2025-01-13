// <copyright file="IState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to the gobal application state.
    /// </summary>
    /// <remarks>
    /// <para>The application state is created when a task sequence starts
    /// executing. It can be used to transport information from one
    /// <see cref="ITask"/> to another.</para>
    /// <para>Tasks can store any type of data in the state, but serialisation
    /// is only supported for the structured data stored in the properties
    /// of the state object. Anything else is lost when transferring the state
    /// from one process to another.</para>
    /// </remarks>
    public interface IState {

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the mount point where the
        /// <see cref="DeploymentShare"/> is mounted.
        /// </summary>
        string? DeploymentDirectory { get; set; }

        /// <summary>
        /// Gets the location of the deployment share.
        /// </summary>
        string? DeploymentShare { get; set; }

        /// <summary>
        /// Gets a potentially open DISM session, which can be used to modfiy an
        /// image or Windows installation.
        /// </summary>
        IDismScope? DismScope { get; set; }

        /// <summary>
        /// Gets the disk where Windows will be installed.
        /// </summary>
        IDisk? InstallationDisk { get; set; }

        /// <summary>
        /// Gets the phase that we are running.
        /// </summary>
        Phase Phase { get; set; }

        /// <summary>
        /// Gets the zero-based index of the current task.
        /// </summary>
        int Progress { get; set; }

        /// <summary>
        /// Gets or sets the path to the working directory on the local machine
        /// that the agent will use.
        /// </summary>
        string? WorkingDirectory { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Persists the state to a JSON file at the specified location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task SaveAsync(string path);
        #endregion

        #region Public indexers
        /// <summary>
        /// Gets or sets the specified state.
        /// </summary>
        /// <param name="key">The key of the state object.</param>
        /// <returns>The value of the key.</returns>
        object? this[string key] { get; set; }
        #endregion
    }
}
