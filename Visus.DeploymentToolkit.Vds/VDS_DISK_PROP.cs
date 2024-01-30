// <copyright file="VDS_DISK_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a disk object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_DISK_PROP {

        Guid Id;

        VDS_DISK_STATUS Status;

        VDS_LUN_RESERVE_MODE ReserveMode;

        VDS_HEALTH Health;

        uint DeviceType;

        uint MediaType;

        ulong Size;

        uint BytesPerSector;

        uint SectorsPerTrack;

        uint TracksPerCylinder;

        uint Flags;

        VDS_STORAGE_BUS_TYPE BusType;

        VDS_PARTITION_STYLE PartitionStyle;

        Guid DiskGuid;
        //    union {
        //DWORD dwSignature;
        //    GUID DiskGuid;
        //};

        [MarshalAs(UnmanagedType.LPWStr)]
        string DiskAddress;

        [MarshalAs(UnmanagedType.LPWStr)]
        string Name;

        [MarshalAs(UnmanagedType.LPWStr)]
        string FriendlyName;

        [MarshalAs(UnmanagedType.LPWStr)]
        string AdaptorName;

        [MarshalAs(UnmanagedType.LPWStr)]
        string DevicePath;

    }
}
