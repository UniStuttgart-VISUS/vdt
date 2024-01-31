# Visus.DeploymentToolkit.Vds
This library provides a C# interface to the [Virtual Disk Service (VDS)](https://learn.microsoft.com/en-us/windows/win32/vds/virtual-disk-service-portal) API. This API is used for low-level diskpart-style operations to prepare a target system.

> [!WARNING]
> The library is a very bare-bones interface to VDS that only exposes the minimum we need for our tasks. Some methods of the COM interface are not mapped correctly, because we do not need them. Calling these methods will most likely crash the application. 

In order to partition and format a disk, the following must be done:

```c#
// Obtain the VDS root object.
var loader = new VdsServiceLoader() as IVdsServiceLoader;
Debug.Assert(loader != null);

// Get the VDS service from the loader and wait until it is ready.
IVdsService service;
loader.LoadService(null, out service);
Debug.Assert(service != null);

{
    var status = service.WaitForServiceReady();
    Debug.Assert(status == 0);
}

// Enumerate all software provider and find the one with our disk.
IEnumVdsObject enumProviders;
service.QueryProviders(VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS, out enumProviders);
Debug.Assert(enumProviders != null);

while (true) {
    enumProviders.Next(1, out var unknown, out uint cnt);
    if (cnt == 0) {
        // There are no more providers.
        break;
    }

    var provider = unknown as IVdsSwProvider;
    Debug.Assert(provider != null);

    // Enumerate all packs of the provider.
    IEnumVdsObject enumPacks;
    provider.QueryPacks(out enumPacks);
    Debug.Assert(enumPacks != null);

    while (true) {
        enumPacks.Next(1, out unknown, out cnt);
        if (cnt == 0) {
            // There are no more packs.
            break;
        }

        var pack = unknown as IVdsPack;
        Debug.Assert(pack != null);

        // Enumerate all disks in the pack.
        IEnumVdsObject enumDisks;
        pack.QueryDisks(out enumDisks);
        Debug.Assert(enumDisks != null);

        while (true) {
            enumDisks.Next(1, out unknown, out cnt);
            if (cnt == 0) {
                // There are no more disks.
                break;
            }

            var disk = unknown as IVdsDisk;
            Debug.Assert(disk != null);

            VDS_DISK_PROP diskProperties;
            disk.GetProperties(out diskProperties);

            if (/* diskProperties match what we are looking for */) {
                var advDisk = disk as IVdsAdvancedDisk;
                Debug.Assert(advDisk != null);

                // Do stuff, for instance check for existing partitions.
                advDisk.QueryPartitions(out var partitionProperties, out var cntPartitions);

            }
        }
   }
}
```