// <copyright file="VDS_LUN_INFORMATION.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines information about a LUN or disk. Applications can use this
    /// structure to uniquely identify a LUN at all times.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_LUN_INFORMATION {

        public uint Version;
        public byte DeviceType;
        public byte DeviceTypeModifier;
        public bool CommandQueueing;
        public VDS_STORAGE_BUS_TYPE BusType;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string VendorId;
         [MarshalAs(UnmanagedType.LPWStr)]
        public string ProductId;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ProductRevision;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string SerialNumber;
        public Guid DiskSignature;
        public VDS_STORAGE_DEVICE_ID_DESCRIPTOR DeviceIdDescriptor;
        public uint NumberOfInterconnects;
        VDS_INTERCONNECT[] Interconnects;    // TODO
    }
}
