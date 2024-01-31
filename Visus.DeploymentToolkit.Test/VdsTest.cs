// <copyright file="VdsTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class VdsTest {

        [TestMethod]
        public void BasicObjects() {
            var loader = new VdsServiceLoader() as IVdsServiceLoader;
            Assert.IsNotNull(loader, "Have IVdsServiceLoader");

            IVdsService service;
            loader.LoadService(null, out service);
            Assert.IsNotNull(service, "Have IVdsService");

            {
                var status = service.WaitForServiceReady();
                Assert.AreEqual(0u, status, "WaitForServiceReady succeeded");
            }
        }

        [TestMethod]
        public void TestMarshalling() {
            Assert.AreEqual(8, Marshal.SizeOf<VDS_PARTITION_INFO_MBR>(), "VDS_PARTITION_INFO_MBR");
            Assert.AreEqual(112, Marshal.SizeOf<VDS_PARTITION_INFO_GPT>(), "VDS_PARTITION_INFO_GPT");
            Assert.AreEqual(144, Marshal.SizeOf<VDS_PARTITION_PROP>(), "VDS_PARTITION_PROP");
        }

        [TestMethod]
        public void EnumerateSoftware() {
            var loader = new VdsServiceLoader() as IVdsServiceLoader;
            Assert.IsNotNull(loader, "Have IVdsServiceLoader");

            IVdsService service;
            loader.LoadService(null, out service);
            Assert.IsNotNull(service, "Have IVdsService");

            {
                var status = service.WaitForServiceReady();
                Assert.AreEqual(0u, status, "WaitForServiceReady succeeded");
            }

            IEnumVdsObject enumProviders;
            service.QueryProviders(VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS,
                out enumProviders);
            Assert.IsNotNull(enumProviders, "Have enumerator for provider");

            enumProviders.Reset();

            while (true) {
                enumProviders.Next(1, out var unknown, out uint cnt);
                if (cnt == 0) {
                    break;
                }

                var provider = unknown as IVdsSwProvider;
                Assert.IsNotNull(provider, "Provider is IVdsVdProvider");

                IEnumVdsObject enumPacks;
                provider.QueryPacks(out enumPacks);
                Assert.IsNotNull(enumPacks, "Have pack enumerator");

                while (true) {
                    enumPacks.Next(1, out unknown, out cnt);
                    if (cnt == 0) {
                        break;
                    }

                    var pack = unknown as IVdsPack;
                    Assert.IsNotNull(pack, "Got IVdsPack");

                    VDS_PACK_PROP packProp;
                    pack.GetProperties(out packProp);

                    if (packProp.Status != VDS_PACK_STATUS.ONLINE) {
                        continue;
                    }

                    IEnumVdsObject enumDisks;
                    pack.QueryDisks(out enumDisks);
                    Assert.IsNotNull(enumDisks, "Have disk enumerator");

                    enumDisks.Next(1, out unknown, out cnt);
                    Assert.AreEqual(1u, cnt, "At least one disk");

                    var disk = unknown as IVdsDisk;
                    Assert.IsNotNull(disk, "Have IVdsDisk");

                    VDS_DISK_PROP diskProp;
                    disk.GetProperties(out diskProp);

                    var advDisk = disk as IVdsAdvancedDisk;
                    Assert.IsNotNull(advDisk, "Have IVdsAdvancedDisk");

                    VDS_PARTITION_PROP[] partitionProps;
                    advDisk.QueryPartitions(out partitionProps, out var cntPartitions);

                    VDS_PARTITION_PROP partitionProp;
                    advDisk.GetPartitionProperties(partitionProps[0].Offset, out partitionProp);
                    Assert.AreEqual(partitionProps[0].PartitionNumber, partitionProp.PartitionNumber, "PartitionNumber matches");

                }
            }
        }
    }
}
