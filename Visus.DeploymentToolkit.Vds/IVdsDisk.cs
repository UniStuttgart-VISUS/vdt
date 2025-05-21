// <copyright file="IVdsDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    [ComImport]
    [Guid("07e5c822-f00c-47a1-8fce-b244da56fd06")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsDisk {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Returns property information for a disk.
        /// </summary>
        /// <param name="diskProperties"></param>
        void GetProperties(out VDS_DISK_PROP diskProperties);

        /// <summary>
        /// Returns the disk pack to which the current disk is a member.
        /// </summary>
        /// <param name="IVdsPack"></param>
        /// <param name=""></param>
        void GetPack(out IVdsPack pack);

        void GetIdentificationData(out VDS_LUN_INFORMATION lunInfo);

        // TODO: Check https://learn.microsoft.com/en-us/dotnet/framework/interop/marshalling-different-types-of-arrays
        void QueryExtents(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            out VDS_DISK_EXTENT[] extentArray,
            out uint numberOfExtents);

        void ConvertStyle(VDS_PARTITION_STYLE newStyle);

        void SetFlags(VDS_DISK_FLAG flags);

        void ClearFlags(VDS_DISK_FLAG flags);
    }
}
