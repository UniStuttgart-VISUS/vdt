// <copyright file="IVdsAdvancedDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Creates and deletes partitions, and modifies partition attributes.
    /// </summary>
    [ComImport]
    [Guid("6e6f6b40-977c-4069-bddd-ac710059f8c0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsAdvancedDisk {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        void GetPartitionProperties(ulong offset,
            out VDS_PARTITION_PROP partitionProp);

        //[ManagedToNativeComInteropStub(typeof(InteropStubs),
        //    nameof(InteropStubs.IVdsAdvancedDiskQueryPartitions))]
        void QueryPartitions(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            out VDS_PARTITION_PROP[] partitionProps,
            out uint numberOfPartitions);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void CreatePartition(
            /* [in]  ULONGLONG ullOffset,
            /* [in]  ULONGLONG ullSize,
            /* [in]  __RPC__in CREATE_PARTITION_PARAMETERS *para,
            /* [out]  __RPC__deref_out_opt IVdsAsync **ppAsync*/);

        void DeletePartition(ulong offset, bool force, bool forceProtected);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void ChangeAttributes(
            /* [in] ULONGLONG ullOffset,
            /* [in]  __RPC__in CHANGE_ATTRIBUTES_PARAMETERS *para*/);

        void AssignDriveLetter(ulong offset,
            [MarshalAs(UnmanagedType.LPWStr, SizeConst = 1)] char letter);

        void DeleteDriveLetter(ulong offset,
            [MarshalAs(UnmanagedType.LPWStr, SizeConst = 1)] char letter);

        void GetDriveLetter(ulong offset,
            [MarshalAs(UnmanagedType.LPWStr, SizeConst = 1)] out char letter);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void FormatPartition(
            /* [in]  ULONGLONG ullOffset,
            /* [in]  VDS_FILE_SYSTEM_TYPE type,
            /* [stri][in] __RPC__in_string LPWSTR pwszLabel,
            /* [in]  DWORD dwUnitAllocationSize,
            /* [in]  BOOL bForce,
            /* [in]  BOOL bQuickFormat,
            /* [in]  BOOL bEnableCompression,
            /* [out] __RPC__deref_out_opt IVdsAsync **ppAsync*/);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void Clean(/*bool force, bool forceOem, bool fullClean,
            /* [out]  __RPC__deref_out_opt IVdsAsync **ppAsync*/);
    }
}
