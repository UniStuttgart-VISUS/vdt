using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visus.DeploymentToolkit.Vds {

    [Flags]
    public enum VIRTUAL_DISK_ACCESS_MASK : uint {
        NONE = 0x00000000,
        ATTACH_RO = 0x00010000,
        ATTACH_RW = 0x00020000,
        DETACH = 0x00040000,
        GET_INFO = 0x00080000,
        CREATE = 0x00100000,
        METAOPS = 0x00200000,
        READ = 0x000d0000,
        ALL = 0x003f0000,
        WRITABLE = 0x00320000
    }
}
