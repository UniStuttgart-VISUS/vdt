// <copyright file="IVdsPack.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Provides methods to query and perform management operations on a pack 
    /// containing disks and volumes.
    /// </summary>
    [ComImport]
    [Guid("3b69d7f5-9d94-4648-91ca-79939ba263bf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsPack {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        void GetProperties(out VDS_PACK_PROP packProp);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void GetProvider(/* [out] __RPC__deref_out_opt IVdsProvider **ppProvider*/ );

        void QueryVolumes(out IEnumVdsObject enumerator);
        
        void  QueryDisks(out IEnumVdsObject enumerator);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void CreateVolume(VDS_VOLUME_TYPE type,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
            VDS_INPUT_DISK[] inputDiskArray,
            int numberOfDisks,
            uint stripeSize,
            out IVdsAsync vdsAsync);

        void AddDisk(Guid diskId, VDS_PARTITION_STYLE partitionStyle,
            bool asHotSpare);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void MigrateDisks(
            /* [size_is][in]  __RPC__in_ecount_full(lNumberOfDisks) VDS_OBJECT_ID *pDiskArray,
            /* [in]  LONG lNumberOfDisks,
            /* [in]  VDS_OBJECT_ID TargetPack,
            /* [in]  BOOL bForce,
            /* [in]  BOOL bQueryOnly,
            /* [size_is][out]  __RPC__out_ecount_full(lNumberOfDisks) HRESULT *pResults,
            /* [out]  __RPC__out BOOL *pbRebootNeeded*/);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void ReplaceDisk(
            /* [in]  Guid OldDiskId,
            /* [in] Guid NewDiskId,
            /* [out] __RPC__deref_out_opt IVdsAsync **ppAsync*/);
        
        void RemoveMissingDisk(Guid diskId);

        void Recover(out IVdsAsync vdsAsync);
    }
}
