﻿// <copyright file="DiskSelectionAction.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Possible actions in a <see cref="DiskSelectionStep"/>.
    /// </summary>
    public enum DiskSelectionAction {

        /// <summary>
        /// No nothing, which disables the step with this action.
        /// </summary>
        None,

        /// <summary>
        /// Include all matching disks.
        /// </summary>
        Include,

        /// <summary>
        /// Exclude all matching disks.
        /// </summary>
        Exclude,

        /// <summary>
        /// Prefer the matching disks, but if there are no matching disks,
        /// the original selection will be used.
        /// </summary>
        Prefer,

        /// <summary>
        /// Avoid matching disks, but if there are no other disks available,
        /// keep the original selection.
        /// </summary>
        Avoid
    }
}
