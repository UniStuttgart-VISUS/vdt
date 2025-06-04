// <copyright file="IVdsAdvancedDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Creates and deletes partitions, and modifies partition attributes.
    /// </summary>
    [ComImport]
    [Guid("6e6f6b40-977c-4069-bddd-ac710059f8c0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsAdvancedDisk {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Returns the properties of the partition identified by the partition
        /// offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="partitionProp"></param>
        void GetPartitionProperties(ulong offset,
            out VDS_PARTITION_PROP partitionProp);

        /// <summary>
        /// Returns the details of all partitions on the current disk.
        /// </summary>
        /// <param name="partitionProps"></param>
        /// <param name="numberOfPartitions"></param>
        void QueryPartitions(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            out VDS_PARTITION_PROP[] partitionProps,
            out uint numberOfPartitions);

        /// <summary>
        /// Creates a partition on a basic disk. The
        /// IVdsCreatePartitionEx::CreatePartitionEx method supersedes this method.
        /// </summary>
        void CreatePartition(ulong offset, ulong size,
            ref CREATE_PARTITION_PARAMETERS parameters,
            out IVdsAsync vdsAsync);

        /// <summary>
        /// Deletes a partition from a basic disk.
        /// </summary>
        /// <param name="offset">The partition offset.</param>
        /// <param name="force">If this parameter is set to
        /// <see langword="true"/>, VDS deletes all partitions unconditionally
        /// (excluding OEM, ESP or MSR). If it is set to
        /// <see langword="false"/>, the operation fails if the partition
        /// is in use. A partition is considered to be in use if calls to lock
        /// or dismount the volume fail.</param>
        /// <param name="forceProtected">If this parameter is set to
        /// <see langword="true"/>, VDS deletes all protected partitions
        /// (including OEM, ESP and MSR) unconditionally. If it is set to
        /// <see langword="false"/>, the operation fails if the partition is
        /// protected.</param>
        void DeletePartition(ulong offset, bool force, bool forceProtected);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void ChangeAttributes(
            /* [in] ULONGLONG ullOffset,
            /* [in]  __RPC__in CHANGE_ATTRIBUTES_PARAMETERS *para*/);

        void AssignDriveLetter(ulong offset, char letter);

        void DeleteDriveLetter(ulong offset, char letter);

        void GetDriveLetter(ulong offset, out char letter);

        /// <summary>
        /// Defines the set of valid types for a file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <param name="label"></param>
        /// <param name="unitAllocationSize"></param>
        /// <param name="force"></param>
        /// <param name="quickFormat"></param>
        /// <param name="enableCompression"></param>
        /// <param name="vdsAsync"></param>
        void FormatPartition(ulong offset, VDS_FILE_SYSTEM_TYPE type,
            [MarshalAs(UnmanagedType.LPWStr)] string label,
            uint unitAllocationSize, bool force, bool quickFormat,
            bool enableCompression, out IVdsAsync vdsAsync);

        /// <summary>
        /// Removes partition information and uninitializes basic or dynamic
        /// disks.
        /// </summary>
        /// <param name="force">If <see langword="true"/>, cleans a disk
        /// containing data volumes or ESP partitions.</param>
        /// <param name="forceOem">If <see langword="true"/>, cleans an
        /// MBR-based disk containing the known OEM partitions in the following
        /// table or cleans a GPT-based disk containing any OEM partition. An
        /// OEM partition has the
        /// <see cref="GptPartitionAttributes.PlatformRequired"/> flag set on a
        /// GPT-based disk.</param>
        /// <param name="fullClean">If <see langword="true"/>, cleans the entire
        /// disk by replacing the data on each sector with zeros; otherwise,
        /// this method cleans only the first and the last megabytes on the disk.
        /// </param>
        /// <param name="vdsAsync"></param>
        void Clean(bool force, bool forceOem, bool fullClean,
            out IVdsAsync vdsAsync);
    }
}
