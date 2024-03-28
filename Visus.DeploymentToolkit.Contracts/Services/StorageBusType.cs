// <copyright file="StorageBusType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The possible storage buses of a disk.
    /// </summary>
    /// <remarks>
    /// The values of the enumeration matches the <c>VDS_STORAGE_BUS_TYPE</c> of
    /// the Virtual Disk Service API.
    /// </remarks>
    public enum StorageBusType : uint {
        Unknown = 0x00,
        Scsi = 0x01,
        Atapi = 0x02,
        Ata = 0x03,
        Ieee1394 = 0x04,
        Ssa = 0x05,
        Fibre = 0x06,
        Usb = 0x07,
        Raid = 0x08,
        iScsi = 0x09,
        Sas = 0x0a,
        Sata = 0x0b,
        SD = 0x0c,
        Mmc = 0x0d,
        Virtual = 0x0e,
        FileBackedVirtual = 0x0f,
        Spaces = 0x10,
        NVMe = 0x11,
        Scm = 0x12,
        Ufs = 0x13
    }
}
