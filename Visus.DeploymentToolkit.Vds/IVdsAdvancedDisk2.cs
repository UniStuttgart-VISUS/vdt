// <copyright file="IVdsAdvancedDisk2.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Provides a method to change partition types.
    /// </summary>
    [ComImport]
    [Guid("9723f420-9355-42de-ab66-e31bb15beeac")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsAdvancedDisk2 {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Changes the partition type on the disk at a specified byte offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="force"></param>
        /// <param name="parameters"></param>
        void ChangePartitionType(ulong offset, bool force,
            ref CHANGE_PARTITION_TYPE_PARAMETERS parameters);
    }
}
