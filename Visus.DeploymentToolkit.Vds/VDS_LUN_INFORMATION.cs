// <copyright file="VDS_LUN_INFORMATION.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines information about a LUN or disk. Applications can use this
    /// structure to uniquely identify a LUN at all times.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_LUN_INFORMATION {

        uint Version;
        byte DeviceType;
        byte DeviceTypeModifier;
        bool CommandQueueing;
        VDS_STORAGE_BUS_TYPE BusType;
        [MarshalAs(UnmanagedType.LPWStr)]
        string VendorId;
        [MarshalAs(UnmanagedType.LPWStr)]
        string ProductId;
        [MarshalAs(UnmanagedType.LPWStr)]
        string ProductRevision;
        [MarshalAs(UnmanagedType.LPWStr)]
        string SerialNumber;
        Guid DiskSignature;
        VDS_STORAGE_DEVICE_ID_DESCRIPTOR DeviceIdDescriptor;
        uint NumberOfInterconnects;
        VDS_INTERCONNECT[] Interconnects;
    }
}
