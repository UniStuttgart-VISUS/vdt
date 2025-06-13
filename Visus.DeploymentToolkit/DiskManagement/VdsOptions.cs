// <copyright file="VdsOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Allows for customising the behaviour of the VDS (Virtual Disk Service)
    /// implementation of <see cref="Services.IDiskManagement"/>.
    /// </summary>
    public sealed class VdsOptions {

        #region Public constants
        /// <summary>
        /// The suggested name for the configuration section to be mapped to
        /// this class.
        /// </summary>
        public const string SectionName = "Vds";
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the number of times failed operations should be
        /// retried.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the timeout for retrying failed operations.
        /// </summary>
        public TimeSpan RetryTimeout { get; set; } = TimeSpan.FromSeconds(5);
        #endregion
    }
}
