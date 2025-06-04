// <copyright file="IDriveInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Linq;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service to determine which drive letters on the system are free.
    /// </summary>
    public interface IDriveInfo {

        /// <summary>
        /// Gets the first free drive letter.
        /// </summary>
        /// <returns>The first free drive letter.</returns>
        string GetFreeDrive() => this.GetFreeDrives().First();

        /// <summary>
        /// Gets all free drive letters.
        /// </summary>
        /// <returns>The free drive letters on the system.</returns>
        IEnumerable<string> GetFreeDrives();

        /// <summary>
        /// Gets the logical drives on the system.
        /// </summary>
        /// <returns>The logical drives on the system.</returns>
        IEnumerable<string> GetLogicalDrives();
    }
}
