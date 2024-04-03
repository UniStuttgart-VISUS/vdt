// <copyright file="VDS_FILE_SYSTEM_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    [Flags]
    public enum VDS_FILE_SYSTEM_FLAG {
        SUPPORT_FORMAT = 0x1,
        SUPPORT_QUICK_FORMAT = 0x2,
        SUPPORT_COMPRESS = 0x4,
        SUPPORT_SPECIFY_LABEL = 0x8,
        SUPPORT_MOUNT_POINT = 0x10,
        SUPPORT_REMOVABLE_MEDIA = 0x20,
        SUPPORT_EXTEND = 0x40,
        ALLOCATION_UNIT_512 = 0x10000,
        ALLOCATION_UNIT_1K = 0x20000,
        ALLOCATION_UNIT_2K = 0x40000,
        ALLOCATION_UNIT_4K = 0x80000,
        ALLOCATION_UNIT_8K = 0x100000,
        ALLOCATION_UNIT_16K = 0x200000,
        ALLOCATION_UNIT_32K = 0x400000,
        ALLOCATION_UNIT_64K = 0x800000,
        ALLOCATION_UNIT_128K = 0x1000000,
        ALLOCATION_UNIT_256K = 0x2000000

    }
}
