// <copyright file="DeploymentShareOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


using System.Net;

namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// The options specifiying the deployment share being mounted by the
    /// <see cref="MountDeploymentShare"/> task.
    /// </summary>
    public sealed class DeploymentShareOptions {

        /// <summary>
        /// Gets or sets the credential used to connect to the share.
        /// </summary>
        public NetworkCredential Credential {
            get => new(this.User, this.Password, this.Domain);
        }

        /// <summary>
        /// Gets or sets the domain for creating the <see cref="Credential"/>.
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="MountDeploymentShare"/> task
        /// should prompt for the connection data.
        /// </summary>
        public bool Interactive { get; set; } = true;

        /// <summary>
        /// Gets or sets the mount point for the share, i.e. the drive letter.
        /// </summary>
        /// <remarks>
        /// If this string is empty, the <see cref="MountDeploymentShare"/> task
        /// will choose the first free drive letter on the system.
        /// </remarks>
        public string? MountPoint { get; set; }

        /// <summary>
        /// Gets or sets the path where the deployment share is located.
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password of the <see cref="User"/>.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the name of the user used to connect to the share.
        /// </summary>
        /// <remarks>
        /// If this string is empty, the <see cref="MountDeploymentShare"/> task
        /// will use the currently logged on user.
        /// </remarks>
        public string? User { get; set; }
    }
}
