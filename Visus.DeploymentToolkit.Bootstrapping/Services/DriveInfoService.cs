// <copyright file="DriveInfoService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of <see cref="IDriveInfo"/>.
    /// </summary>
    internal sealed class DriveInfoService : IDriveInfo {

        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public IEnumerable<string> GetFreeDrives() {
            var allDrives = from d in Enumerable.Range('a', 'z' - 'a')
                            select $@"{(char) d}:\";
            var usedDrives = this.GetLogicalDrives().Select(d => d.ToLower());
            return allDrives.Except(usedDrives);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetLogicalDrives()
            => Environment.GetLogicalDrives();
    }
}
