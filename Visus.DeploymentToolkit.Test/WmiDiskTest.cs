// <copyright file="WmiDiskTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for the <see cref="WmiDiskService"/> wrapper class.
    /// </summary>
    [TestClass]
    public sealed class WmiDiskTest {

        [TestMethod]
        public async Task GetDisks() {
            var wmi = new ManagementService(CreateLogger<ManagementService>());
            var service = new WmiDiskService(wmi, CreateLogger<WmiDiskService>());
            Assert.IsNotNull(service);

            var disks = await service.GetDisksAsync(CancellationToken.None);
            Assert.IsTrue(disks.Any());

            foreach (var disk in disks) {
                Assert.IsTrue(disk.Partitions.Any());
                Assert.IsNotNull(disk.Volumes);
                Assert.IsNotNull(disk.Volumes.ToArray());
            }
        }

        //[TestMethod]
        //public async Task SelectDisks() {
        //    if (WindowsIdentity.GetCurrent().IsAdministrator()) {
        //        var logger = this._loggerFactory.CreateLogger<VdsService>();
        //        var service = new VdsService(logger);
        //        Assert.IsNotNull(service);

        //        var task = service.GetDisksAsync(CancellationToken.None);
        //        task.Wait();
        //        var disks = task.Result;
        //        Assert.IsTrue(disks.Any());

        //        var selection = new DiskSelectionStep() {
        //            Action = DiskSelectionAction.Include,
        //        };

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.None;
        //            selection.Condition = "BusType == \"Nvme\"";
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsTrue(selected.Any());
        //        }

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.None;
        //            selection.Condition = "BusType == \"SD\"";
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsFalse(selected.Any());
        //        }

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.HasMicrosoftPartition;
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsTrue(selected.Any());
        //        }

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.IsSmallest;
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsTrue(selected.Any());
        //        }

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.IsLargest;
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsTrue(selected.Any());
        //        }

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.IsEfiSystemDisk;
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsTrue(selected.Any());
        //        }

        //        {
        //            selection.BuiltInCondition = BuiltInCondition.None;
        //            selection.Condition = "PartitionStyle == \"Gpt\"";
        //            var selected = await selection.ApplyAsync(disks, service, logger);
        //            Assert.IsTrue(selected.Any());
        //        }
        //    }
        //}

        private static ILogger<T> CreateLogger<T>() => Loggers.CreateLogger<T>();
        private static readonly ILoggerFactory Loggers = LoggerFactory.Create(l => l.AddDebug());
    }
}
