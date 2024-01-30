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

        public Guid Id;

        public VDS_DISK_STATUS Status;

        public VDS_LUN_RESERVE_MODE ReserveMode;

        public VDS_HEALTH Health;

        public uint DeviceType;

        public uint MediaType;

        public ulong Size;

        public uint BytesPerSector;

        public uint SectorsPerTrack;

        public uint TracksPerCylinder;

        public uint Flags;

        public VDS_STORAGE_BUS_TYPE BusType;

        public VDS_PARTITION_STYLE PartitionStyle;

        public Guid DiskGuid;
        //    union {
        //DWORD dwSignature;
        //    GUID DiskGuid;
        //};

        [MarshalAs(UnmanagedType.LPWStr)]
        public string DiskAddress;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Name;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string FriendlyName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string AdaptorName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string DevicePath;

    }
}
