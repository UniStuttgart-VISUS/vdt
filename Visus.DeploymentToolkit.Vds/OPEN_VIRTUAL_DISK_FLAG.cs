using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
