// <copyright file="IVdsVolumeMF2.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    [ComImport]
    [Guid("4dbcee9a-6343-4651-b85f-5e75d74d983c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsVolumeMF2 {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        void GetFileSystemTypeName(out string fileSystemTypeName);

        void QueryFileSystemFormatSupport(
            nint fileSystemSupportProps,
            //out VDS_FILE_SYSTEM_FORMAT_SUPPORT_PROP fileSystemSupportProps,
            out int numberOfFileSystems);

        void FormatEx(string fileSystemTypeName,
            ushort fileSystemRevision,
            uint desiredUnitAllocationSize,
            string? label,
            bool force,
            bool quickFormat,
            bool nableCompression,
            out IVdsAsync vdsAsync);
    }
}
