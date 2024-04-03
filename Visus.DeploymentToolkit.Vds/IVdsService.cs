// <copyright file="IVdsService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


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
        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
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
        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void GetObject(Guid objectId, VDS_OBJECT_TYPE type,
            [MarshalAs(UnmanagedType.IUnknown)] out object unknown);

        /// <summary>
        /// Enumerates the drive letters of the server.
        /// </summary>
        /// <param name="firstLetter">The first drive letter to query as a
        /// single uppercase or lowercase alphabetical (A-Z) Unicode character.
        /// </param>
        /// <param name="count">The total number of drive letters to retrieve,
        /// beginning with the letter that <paramref name="firstLetter">
        /// specifies. This <i>must</i> also be the number of elements in the
        /// <paramref name="driveLetterPropArray"/>. It <i>must not</i> exceed
        /// the total number of drive letters between the letter in
        /// <paramref name="firstLetter"/> and the last possible drive letter
        /// (Z), inclusive.</param>
        /// <param name="driveLetterPropArray">An array of
        /// <see cref="VDS_DRIVE_LETTER_PROP "/> structures that, if the
        /// operation is successfully completed, receives the array of drive
        /// letter properties.</param>
        [Obsolete("This mapping works, but always returns "
            + "VDS_E_INVALID_DRIVE_LETTER...")]
        void QueryDriveLetters(char firstLetter,
            int count,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            VDS_DRIVE_LETTER_PROP[] driveLetterPropArray);

        /// <summary>
        /// Returns property details for all file systems known to VDS.
        /// </summary>
        /// <param name="fileSystemTypeProps">A pointer to an array of 
        /// <see cref="VDS_FILE_SYSTEM_TYPE_PROP "/> structures that, if the
        /// operation is successfully completed, receives the array of file
        /// system type properties.</param>
        /// <param name="numberOfFileSystems">A pointer to a variable that, if
        /// the operation is successfully completed, receives the total number
        /// of elements returned in <paramref name="fileSystemTypeProps" />.
        /// </param>
        void QueryFileSystemTypes(out IntPtr fileSystemTypeProps,
            //[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            //out VDS_FILE_SYSTEM_TYPE_PROP[] fileSystemTypeProps,
            out int numberOfFileSystems);

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
        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
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
