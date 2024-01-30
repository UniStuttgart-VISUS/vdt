// <copyright file="OPEN_VIRTUAL_DISK_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

namespace Visus.DeploymentToolkit.Vds {

    [Flags]
    public enum OPEN_VIRTUAL_DISK_FLAG : uint {
        NONE = 0x00000000,
        NO_PARENTS = 0x00000001,
        BLANK_FILE = 0x00000002,
        BOOT_DRIVE = 0x00000004,
        CACHED_IO = 0x00000008,
        CUSTOM_DIFF_CHAIN = 0x00000010,
        PARENT_CACHED_IO = 0x00000020,
        VHDSET_FILE_ONLY = 0x00000040,
        IGNORE_RELATIVE_PARENT_LOCATOR = 0x00000080,
        NO_WRITE_HARDENING = 0x00000100,
        SUPPORT_COMPRESSED_VOLUMES,
        SUPPORT_SPARSE_FILES_ANY_FS,
        SUPPORT_ENCRYPTED_FILES
    }
}
