// <copyright file="WellKnownStates.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The keys of well known state objects.
    /// </summary>
    public static class WellKnownStates {

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
        /// The current DISM session.
        /// </summary>
        public const string DismScope = nameof(IState.DismScope);

        /// <summary>
        /// The disk to which Windows should be installed.
        /// </summary>
        public const string InstallationDisk = nameof(IState.InstallationDisk);

        /// <summary>
        /// The current <see cref="Workflow.Phase"/>.
        /// </summary>
        public const string Phase = nameof(IState.Phase);

        /// <summary>
        /// The current progress of the active task sequence.
        /// </summary>
        public const string Progress = nameof(IState.Progress);

        /// <summary>
        /// The working directory where the agent will store temporary files.
        /// </summary>
        public const string WorkingDirectory = nameof(IState.WorkingDirectory);
    }
}
