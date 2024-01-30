using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visus.DeploymentToolkit.Vds {


    public enum VDS_VDISK_STATE : uint {
        UNKNOWN = 0,
        ADDED,
        OPEN,
        ATTACH_PENDING,
        ATTACHED_NOT_OPEN,
        ATTACHED,
        DETACH_PENDING,
        COMPACTING,
        MERGING,
        EXPANDING,
        DELETED,
        MAX
    }
}
