// <copyright file="IVdsVolumeMF.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    [ComImport]
    [Guid("ee2d5ded-6236-4169-931d-b9778ce03dc6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsVolumeMF {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        void GetFileSystemProperties(out VDS_FILE_SYSTEM_PROP fileSystemProp);

        void Format(VDS_FILE_SYSTEM_TYPE type,
            string label,
            uint unitAllocationSize,
            bool force,
            bool quickFormat,
            bool enableCompression,
            out IVdsAsync vdsAsync);

        void AddAccessPath(string path);

        // TODO: This is not correctly mapped.
        void QueryAccessPaths(
            [MarshalAs(UnmanagedType.LPArray,
                ArraySubType = UnmanagedType.LPWStr,
                SizeParamIndex = 1)]
            out string[] pathArray,
            out int numberOfAccessPaths);

        void QueryReparsePoints(out IntPtr reparsePointProps,
            out int umberOfReparsePointProps);

        void DeleteAccessPath(string path, bool force);

        void Mount();

        void Dismount(bool force, bool permanent);

        void SetFileSystemFlags(VDS_FILE_SYSTEM_FLAG flags);
        
        void ClearFileSystemFlags(VDS_FILE_SYSTEM_FLAG flags);
    }
}
