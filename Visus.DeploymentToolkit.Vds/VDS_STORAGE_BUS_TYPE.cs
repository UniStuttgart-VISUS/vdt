// <copyright file="VDS_STORAGE_BUS_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid bus types of a storage device.
    /// </summary>
    public enum VDS_STORAGE_BUS_TYPE : uint {
        Unknown = 0,
        Scsi = 0x1,
        Atapi = 0x2,
        Ata = 0x3,
        Ieee1394 = 0x4,
        Ssa = 0x5,
        Fibre = 0x6,
        Usb = 0x7,
        RAID = 0x8,
        iScsi = 0x9,
        Sas = 0xa,
        Sata = 0xb,
        Sd = 0xc,
        Mmc = 0xd,
        Max = 0xe,
        Virtual = 0xe,
        FileBackedVirtual = 0xf,
        Spaces = 0x10,
        NVMe = 0x11,
        Scm = 0x12,
        Ufs = 0x13,
        MaxReserved = 0x7f
    }
}
