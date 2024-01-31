using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visus.DeploymentToolkit.Vds {
    internal static class InteropStubs {

        internal static void IVdsAdvancedDiskQueryPartitions(
                IVdsAdvancedDisk that,
                out VDS_PARTITION_PROP[] partitionProps,
                out uint numberOfPartitions) {
            throw new NotImplementedException();
        }
    }
}
