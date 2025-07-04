﻿// <copyright file="WellKnownStates.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


using System.CodeDom;

namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The keys of well known state objects.
    /// </summary>
    /// <remarks>
    /// Tasks can store (and this way communicate to other tasks) arbitrary data
    /// in <see cref="IState"/>, but should avoid using any of these well-known
    /// identifiers unless they intend to modify the specific state described
    /// here (and in the corresponding property of <see cref="IState"/>).
    /// </remarks>
    public static class WellKnownStates {

        /// <summary>
        /// The architecture of the system that is being serviced.
        /// </summary>
        public const string Architecture = nameof(IState.Architecture);

        /// <summary>
        /// The path to the binary of the deployment agent.
        /// </summary>
        public const string AgentPath = nameof(IState.AgentPath);

        /// <summary>
        /// The path where the boot drive is mounted.
        /// </summary>
        public const string BootDrive = nameof(IState.BootDrive);

        /// <summary>
        /// The path to the binary of the bootstrapper.
        /// </summary>
        public const string BootstrapperPath = nameof(IState.BootstrapperPath);

        /// <summary>
        /// The directory where the deployment share is mounted.
        /// </summary>
        public const string DeploymentDirectory
            = nameof(IState.DeploymentDirectory);

        /// <summary>
        /// The location of the deployment share.
        /// </summary>
        public const string DeploymentShare = nameof(IState.DeploymentShare);

        /// <summary>
        /// The domain the deployment server is joined to.
        /// </summary>
        public const string DeploymentShareDomain
            = nameof(IState.DeploymentShareDomain);

        /// <summary>
        /// The password used for the deployment share.
        /// </summary>
        public const string DeploymentSharePassword
            = nameof(IState.DeploymentSharePassword);

        /// <summary>
        /// The name of the user connecting to the deployment share.
        /// </summary>
        public const string DeploymentShareUser
            = nameof(IState.DeploymentShareUser);

        /// <summary>
        /// Gets or sets the path where the installation disk is mounted in
        /// WinPE.
        /// </summary>
        public const string InstallationDirectory
            = nameof(IState.InstallationDirectory);

        /// <summary>
        /// The disk to which Windows should be installed.
        /// </summary>
        public const string InstallationDisk = nameof(IState.InstallationDisk);

        /// <summary>
        /// The path to the WIM image to be applied.
        /// </summary>
        public const string InstallationImage
            = nameof(IState.InstallationImage);

        /// <summary>
        /// The one-based index of the WIM image to be applied.
        /// </summary>
        public const string InstallationImageIndex
            = nameof(IState.InstallationImageIndex);

        /// <summary>
        /// The current <see cref="Workflow.Phase"/>.
        /// </summary>
        public const string Phase = nameof(IState.Phase);

        /// <summary>
        /// The current progress of the active task sequence.
        /// </summary>
        public const string Progress = nameof(IState.Progress);

        /// <summary>
        /// The password for the current session, which is used to derive
        /// encryption keys.
        /// </summary>
        public const string SessionKey = nameof(IState.SessionKey);

        /// <summary>
        /// The path to the state file.
        /// </summary>
        public const string StateFile = nameof(IState.StateFile);

        /// <summary>
        /// Identifies the task sequence that is currently executing.
        /// </summary>
        public const string TaskSequence = nameof(IState.TaskSequence);

        /// <summary>
        /// The working directory where the agent will store temporary files.
        /// </summary>
        public const string WorkingDirectory = nameof(IState.WorkingDirectory);

        /// <summary>
        /// The currently mounted WIM image.
        /// </summary>
        public const string WimMount = nameof(IState.WimMount);
    }
}
