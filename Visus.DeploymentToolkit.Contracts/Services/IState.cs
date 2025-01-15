// <copyright file="IState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading.Tasks;
using Visus.DeploymentToolkit.Compliance;
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
        /// Gets or sets the path to the deployment agent.
        /// </summary>
        /// <remarks>
        /// The format depends of the phase the system is running in. Typically,
        /// for the bootstrapper, the location is relative to the deployment
        /// share, but for subsequent phases, we try to use absolute paths on
        /// the local disk whenever possible.
        /// </remarks>
        string? AgentPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the mount point where the
        /// <see cref="DeploymentShare"/> is mounted.
        /// </summary>
        string? DeploymentDirectory { get; set; }

        /// <summary>
        /// Gets or sets the location of the deployment share, which is the UNC
        /// path of a network location where the deployment agent and all
        /// configuration data are centrally stored.
        /// </summary>
        string? DeploymentShare { get; set; }

        /// <summary>
        /// Gets or sets the name of the domain the server hosting the deployment
        /// share is joined to.
        /// </summary>
        string? DeploymentShareDomain { get; set; }

        /// <summary>
        /// Gets or sets the encrypted password used to connect to the
        /// deployment share.
        /// </summary>
        [SensitiveData]
        string? DeploymentSharePassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the user connecting to the deployment share.
        /// </summary>
        string? DeploymentShareUser { get; set; }

        /// <summary>
        /// Gets or sets a potentially open DISM session, which can be used to
        /// modfiy an image or Windows installation.
        /// </summary>
        IDismScope? DismScope { get; set; }

        /// <summary>
        /// Gets or sets the disk where Windows will be installed.
        /// </summary>
        IDisk? InstallationDisk { get; set; }

        /// <summary>
        /// Gets or sets the phase that we are running.
        /// </summary>
        Phase Phase { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the current task.
        /// </summary>
        /// <remarks>
        /// The bootstrapper and the deployment agent use this variable to track
        /// which task is to be executed next. As the state is being persisted in
        /// <see cref="StateFile"/>, this information can also survive a reboot,
        /// which allows the agent to continue a task sequence after the machine
        /// has restarted.
        /// </remarks>
        int Progress { get; set; }

        /// <summary>
        /// Gets or sets the location of the file where the state is persisted.
        /// </summary>
        string? StateFile { get; set; }

        /// <summary>
        /// Gets or sets the key used for encrypting sensitive data in this
        /// session.
        /// </summary>
        /// <remarks>
        /// The session key itself is a sensitive information as it allows for
        /// decrypting other sensitive data. The encryption feature makes data
        /// only less obvious in the state file and logs, but the data cannot be
        /// safe as we need to be able to restore them without user intervention.
        /// </remarks>
        [SensitiveData]
        string? SessionKey { get; set; }    // TODO: we should find a way to exclude sensitive data at least from the log. Use attribute?

        /// <summary>
        /// Gets or sets the ID or location of the task sequence that is
        /// currently executing (if the property is a <see cref="string"/>) or
        /// the task sequence (<see cref="ITaskSequence"/>) itself.
        /// </summary>
        /// <exception cref="System.ArgumentException">If the new value is
        /// neither a <see cref="string"/>, an <see cref="ITaskSequence"/>, nor
        /// <c>null</c>.</exception>
        object? TaskSequence { get; set; }

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
        /// <remarks>
        /// The method will update the <see cref="StateFile"/> variable to the
        /// given <paramref name="path"/> if this string is non-<c>null</c>.
        /// Otherwise, an existing <see cref="StateFile"/> will be used. If
        /// either are <c>null</c>, the operation will fail.
        /// </remarks>
        /// <param name="path"></param>
        /// <returns></returns>
        Task SaveAsync(string? path);
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
