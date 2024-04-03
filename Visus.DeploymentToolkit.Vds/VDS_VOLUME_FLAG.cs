// <copyright file="VDS_VOLUME_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid flags for a volume object.
    /// </summary>
    [Flags]
    public enum VDS_VOLUME_FLAG : uint {
        SYSTEM_VOLUME = 0x1,
        BOOT_VOLUME = 0x2,
        ACTIVE = 0x4,
        READONLY = 0x8,
        HIDDEN = 0x10,
        CAN_EXTEND = 0x20,
        CAN_SHRINK = 0x40,
        PAGEFILE = 0x80,
        HIBERNATION = 0x100,
        CRASHDUMP = 0x200,
        INSTALLABLE = 0x400,
        LBN_REMAP_ENABLED = 0x800,
        FORMATTING = 0x1000,
        NOT_FORMATTABLE = 0x2000,
        NTFS_NOT_SUPPORTED = 0x4000,
        FAT32_NOT_SUPPORTED = 0x8000,
        FAT_NOT_SUPPORTED = 0x10000,
        NO_DEFAULT_DRIVE_LETTER = 0x20000,
        PERMANENTLY_DISMOUNTED = 0x40000,
        PERMANENT_DISMOUNT_SUPPORTED = 0x80000,
        SHADOW_COPY = 0x100000,
        FVE_ENABLED = 0x200000,
        DIRTY = 0x400000,
        REFS_NOT_SUPPORTED = 0x800000,
        BACKS_BOOT_VOLUME = 0x1000000,
        BACKED_BY_WIM_IMAGE = 0x2000000
    }
}
