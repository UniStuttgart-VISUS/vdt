// <copyright file="DiskExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.DiskManagement;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="IDisk"/>.
    /// </summary>
    internal static class DiskExtensions {

        /// <summary>
        /// Gets the byte per track of the specified <paramref name="disk"/>.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public static ulong GetBytesPerTrack(this VdsDisk disk) {
            if (disk is null) {
                return 0;
            }

            return disk.Properties.BytesPerSector
                * disk.Properties.SectorsPerTrack;
        }

        /// <summary>
        /// Gets the default cluster size for an NTFS file system on the
        /// specified <paramref name="disk"/>.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        public static ulong GetNtfsClusterSize(this IDisk disk) {
            if (disk is null) {
                return Cluster4K;
            }

            return disk.Size switch {
                >= 2UL * Gigabyte => Cluster4K,
                >= 1UL * Gigabyte => Cluster2K,
                > 512 * Megabyte => Cluster1K,
                _ => Cluster512
            };
        }

        #region Private constants
        private const ulong Cluster4K = 4096;
        private const ulong Cluster2K = 2048;
        private const ulong Cluster1K = 1024;
        private const ulong Cluster512 = 512;
        private const ulong Gigabyte = 1024 * Megabyte;
        private const ulong Megabyte = 1024 * 1024;
        #endregion
    }
}
