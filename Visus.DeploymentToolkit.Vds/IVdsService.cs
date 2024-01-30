// <copyright file="IVdsService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Provides methods to query and interact with VDS.
    /// </summary>
    [ComImport]
    [Guid("0818a8ef-9ba9-40d8-a6f9-e22833cc771e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsService {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Returns the initialization status of VDS.
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        uint IsServiceReady();

        /// <summary>
        /// Waits for VDS initialization to complete and returns the status of
        /// the VDS initialization.
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        uint WaitForServiceReady();

        /// <summary>
        /// Returns the properties of VDS.
        /// </summary>
        [Obsolete("This is not correctly mapped")]
        void GetProperties(/* [out] VDS_SERVICE_PROP *pServiceProp */);

        /// <summary>
        /// Returns an enumeration object containing a list of the hardware and
        /// software providers known to VDS.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="providers"></param>
        void QueryProviders(VDS_QUERY_PROVIDER_FLAG mask,
            out IEnumVdsObject providers);

        /// <summary>
        /// Not supported. This method is reserved for future use.
        /// </summary>
        /// <param name="disks"></param>
        void QueryMaskedDisks(out IEnumVdsObject disks);

        /// <summary>
        /// Returns an enumeration object containing a list of the unallocated
        /// disks managed by VDS.
        /// </summary>
        /// <param name="disks"></param>
        void QueryUnallocatedDisks(out IEnumVdsObject disks);

        /// <summary>
        /// Returns an object pointer for the identified object.
        /// </summary>
        [Obsolete("This is not correctly mapped")]
        void GetObject(/*
            [in]  VDS_OBJECT_ID   ObjectId,
            [in]  VDS_OBJECT_TYPE type,
            [out] IUnknown        **ppObjectUnk
        */);

        /// <summary>
        /// Returns property details for a set of drive letters.
        /// </summary>
        [Obsolete("This is not correctly mapped")]
        void QueryDriveLetters(/*
            [in]  WCHAR                 wcFirstLetter,
            [in]  DWORD                 count,
            [out] VDS_DRIVE_LETTER_PROP *pDriveLetterPropArray
                                */);

        /// <summary>
        /// Returns property details for all file systems known to VDS.
        /// </summary>
        /// <param name="fileSystemTypeProps"></param>
        /// <param name="numberOfFileSystems"></param>
        void QueryFileSystemTypes(out IntPtr fileSystemTypeProps,   //VDS_FILE_SYSTEM_TYPE_PROP **
            out uint numberOfFileSystems);

        /// <summary>
        /// Discovers newly added and newly removed disks.
        /// </summary>
        void Reenumerate();

        /// <summary>
        /// Refreshes disk-ownership and disk-layout information.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Removes user-mode paths and mounted folders for volumes that no
        /// longer exist.
        /// </summary>
        void CleanupObsoleteMountPoints();

        /// <summary>
        /// Registers the caller's IVdsAdviseSink interface with VDS so 
        /// that the caller receives notifications from the VDS service.
        /// </summary>
        [Obsolete("This is not correctly mapped")]
        void Advise(/*
            [in]  IVdsAdviseSink *pSink,
            [out] DWORD          *pdwCookie
        */);

        /// <summary>
        /// Unregisters the caller's IVdsAdviseSink interface so that the caller
        /// no longer receives notifications from the VDS service.
        /// </summary>
        /// <param name="cookie"></param>
        void Unadvise(uint cookie);

        /// <summary>
        /// Restarts the computer hosting the provider.
        /// </summary>
        void Reboot();

        /// <summary>
        /// Sets service object flags.
        /// </summary>
        /// <param name="flags"></param>
        void SetFlags(VDS_SERVICE_FLAG flags);

        /// <summary>
        /// Clears service object flags.
        /// </summary>
        /// <param name="flags"></param>
        void ClearFlags(VDS_SERVICE_FLAG flags);

    }
}
