// <copyright file="IVdsVolumeMF3.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    [ComImport]
    [Guid("6788FAF9-214E-4b85-BA59-266953616E09")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsVolumeMF3 {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        void QueryVolumeGuidPathnames(out nint pathArray,
            out uint numberOfPaths);

        void FormatEx2(string fileSystemTypeName,
            ushort fileSystemRevision,
            uint desiredUnitAllocationSize,
            string? label,
            uint options,
            out IVdsAsync vdsAsync);

        void OfflineVolume();
    }
}
