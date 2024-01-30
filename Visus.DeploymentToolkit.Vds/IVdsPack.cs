// <copyright file="IVdsPack.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

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

        [Obsolete("This is not correctly mapped")]
        void GetProperties(/* [out] VDS_PACK_PROP *pPackProp*/);

        [Obsolete("This is not correctly mapped")]
        void GetProvider(/* [out] __RPC__deref_out_opt IVdsProvider **ppProvider*/ );

        void QueryVolumes(out IEnumVdsObject enumerator);
        
        void  QueryDisks(out IEnumVdsObject enumerator);

        [Obsolete("This is not correctly mapped")]
        void CreateVolume(/* [in] VDS_VOLUME_TYPE type,
            /* [size_is][in] __RPC__in_ecount_full(lNumberOfDisks) VDS_INPUT_DISK *pInputDiskArray,
        /* [in]  LONG lNumberOfDisks,
            /* [in] ULONG ulStripeSize,
            /* [out]  __RPC__deref_out_opt IVdsAsync **ppAsync*/);
        
        void AddDisk(Guid diskId, VDS_PARTITION_STYLE partitionStyle,
            bool asHotSpare);

        [Obsolete("This is not correctly mapped")]
        void MigrateDisks(
            /* [size_is][in]  __RPC__in_ecount_full(lNumberOfDisks) VDS_OBJECT_ID *pDiskArray,
            /* [in]  LONG lNumberOfDisks,
            /* [in]  VDS_OBJECT_ID TargetPack,
            /* [in]  BOOL bForce,
            /* [in]  BOOL bQueryOnly,
            /* [size_is][out]  __RPC__out_ecount_full(lNumberOfDisks) HRESULT *pResults,
            /* [out]  __RPC__out BOOL *pbRebootNeeded*/);

        [Obsolete("This is not correctly mapped")]
        void ReplaceDisk(
            /* [in]  Guid OldDiskId,
            /* [in] Guid NewDiskId,
            /* [out] __RPC__deref_out_opt IVdsAsync **ppAsync*/);
        
        void RemoveMissingDisk(Guid diskId);

        [Obsolete("This is not correctly mapped")]
        void Recover(/* [out] __RPC__deref_out_opt IVdsAsync **ppAsync*/);
    }
}
