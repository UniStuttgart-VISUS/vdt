// <copyright file="VdsTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class VdsTest {

        [TestMethod]
        public void BasicObjects() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
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
        }

        [TestMethod]
        public async Task DisksFromVdsService() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var vds = new VdsService(this._loggerFactory.CreateLogger<VdsService>());
                var disks = await vds.GetDisksAsync(CancellationToken.None);
                Assert.IsTrue(disks.Any());
                Assert.IsTrue(disks.Any(d => d.Partitions.Any()));
                Assert.IsTrue(disks.Any(d => d.Volumes.Any()));
            }
        }

        [TestMethod]
        public void EnumerateSoftware() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
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

                        advDisk.QueryPartitions(out var partitionProps, out var cntPartitions);

                        advDisk.GetPartitionProperties(partitionProps[0].Offset, out var partitionProp);
                        Assert.AreEqual(partitionProps[0].PartitionNumber, partitionProp.PartitionNumber, "PartitionNumber matches");
                    }
                }
            }
        }


        [TestMethod]
        public void EnumerateExtents() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var loader = new VdsServiceLoader() as IVdsServiceLoader;
                Assert.IsNotNull(loader, "Have IVdsServiceLoader");

                IVdsService service;
                loader.LoadService(null, out service);
                Assert.IsNotNull(service, "Have IVdsService");

                {
                    var status = service.WaitForServiceReady();
                    Assert.AreEqual(0u, status, "WaitForServiceReady succeeded");
                }

                foreach (var provider in service.QuerySoftwareProviders()) {
                    foreach (var pack in provider.QueryPacks()) {
                        VDS_PACK_PROP packProp;
                        pack.GetProperties(out packProp);

                        if (packProp.Status != VDS_PACK_STATUS.ONLINE) {
                            continue;
                        }

                        foreach (var d in pack.QueryDisks()) {
                            d.QueryExtents(out var extents, out var _);
                            ((IVdsAdvancedDisk) d).QueryPartitions(out var parts, out var _);
                            Assert.IsNotNull(extents);
                            Assert.IsTrue(extents.Any());
                            Assert.IsTrue(extents.Length >= parts.Length);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void EnumerateVolumes() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
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

                        IEnumVdsObject enumVolumes;
                        pack.QueryVolumes(out enumVolumes);

                        while (true) {
                            enumVolumes.Next(1, out unknown, out cnt);
                            if (cnt == 0) {
                                break;
                            }

                            var volume = unknown as IVdsVolume;
                            Assert.IsNotNull(volume, "Got IVdsVolume");

                            volume.GetProperties(out var volumeProp);
                            Assert.IsNotNull(volumeProp.Name);

                            var volume2 = unknown as IVdsVolume2;
                            Assert.IsNotNull(volume2);

                            volume2.GetProperties2(out var volumeProp2);
                            Assert.AreEqual(volumeProp.Name, volumeProp2.Name);
                            Assert.AreEqual(volumeProp.ID, volumeProp2.ID);

                            var volumeMF = unknown as IVdsVolumeMF;
                            Assert.IsNotNull(volumeMF, "Got IVdsVolumeMF");

                            volumeMF.GetFileSystemProperties(out var fsProp);
                            Assert.IsNotNull(fsProp.Label);

                            volumeMF.QueryAccessPaths(out var paths, out int cntPaths);
                            Assert.IsNotNull(paths);
                            Assert.AreEqual(cntPaths, paths.Length);

                            var reparsePoints = volumeMF.QueryReparsePoints();
                            Assert.IsNotNull(reparsePoints);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void FileSystemTypes() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var loader = new VdsServiceLoader() as IVdsServiceLoader;
                Assert.IsNotNull(loader, "Have IVdsServiceLoader");

                IVdsService service;
                loader.LoadService(null, out service);
                Assert.IsNotNull(service, "Have IVdsService");

                {
                    var status = service.WaitForServiceReady();
                    Assert.AreEqual(0u, status, "WaitForServiceReady succeeded");
                }

                var fstp = service.QueryFileSystemTypes();
                Assert.IsTrue(fstp.Any());
            }
        }


        [TestMethod]
        public void TestMarshalling() {
            Assert.AreEqual(8, Marshal.SizeOf<VDS_PARTITION_INFO_MBR>(), "VDS_PARTITION_INFO_MBR");
            Assert.AreEqual(112, Marshal.SizeOf<VDS_PARTITION_INFO_GPT>(), "VDS_PARTITION_INFO_GPT");
            Assert.AreEqual(144, Marshal.SizeOf<VDS_PARTITION_PROP>(), "VDS_PARTITION_PROP");
            Assert.AreEqual(2, Marshal.SizeOf<MbrPartitionParameters>(), "MbrPartitionParameters");
            Assert.AreEqual(120, Marshal.SizeOf<CREATE_PARTITION_PARAMETERS>(), "CREATE_PARTITION_PARAMETERS");
            Assert.AreEqual(32, Marshal.SizeOf<VDS_ASYNC_OUTPUT>(), "VDS_ASYNC_OUTPUT");
        }

        [TestMethod]
        public async Task PartitionForVolume() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var vds = new VdsService(this._loggerFactory.CreateLogger<VdsService>());
                var disks = await vds.GetDisksAsync(CancellationToken.None);
                Assert.IsTrue(disks.Any());
                Assert.IsTrue(disks.Any(d => d.VolumePartitions.Any()));

                foreach (var d in disks) {
                    foreach (var v in d.VolumePartitions) {
                        Assert.IsNotNull(v.Item1);
                        Assert.IsNotNull(v.Item2);
                    }
                }
            }
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
