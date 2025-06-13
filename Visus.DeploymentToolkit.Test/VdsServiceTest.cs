// <copyright file="VdsServiceTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Principal;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for the <see cref="VdsService"/> wrapper class.
    /// </summary>
    [TestClass]
    public sealed class VdsServiceTest {

        [TestMethod]
        public void GetDisks() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var service = new VdsService(Options.Create(new VdsOptions()), CreateLogger<VdsService>());
                Assert.IsNotNull(service);

                var task = service.GetDisksAsync(CancellationToken.None);
                task.Wait();
                var disks = task.Result;
                Assert.IsTrue(disks.Any());

                foreach (var disk in disks) {
                    Assert.IsTrue(disk.Partitions.Any());
                    Assert.IsNotNull(disk.Volumes);
                }
            }
        }

        [TestMethod]
        public async Task SelectDisks() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var logger = CreateLogger<VdsService>();
                var service = new VdsService(Options.Create(new VdsOptions()), logger);
                Assert.IsNotNull(service);

                var task = service.GetDisksAsync(CancellationToken.None);
                task.Wait();
                var disks = task.Result;
                Assert.IsTrue(disks.Any());

                var selection = new DiskSelectionStep() {
                    Action = DiskSelectionAction.Include,
                };

                {
                    selection.BuiltInCondition = BuiltInCondition.None;
                    selection.Condition = "BusType == \"Nvme\"";
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsTrue(selected.Any());
                }

                {
                    selection.BuiltInCondition = BuiltInCondition.None;
                    selection.Condition = "BusType == \"SD\"";
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsFalse(selected.Any());
                }

                {
                    selection.BuiltInCondition = BuiltInCondition.HasMicrosoftPartition;
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsTrue(selected.Any());
                }

                {
                    selection.BuiltInCondition = BuiltInCondition.IsSmallest;
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsTrue(selected.Any());
                }

                {
                    selection.BuiltInCondition = BuiltInCondition.IsLargest;
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsTrue(selected.Any());
                }

                {
                    selection.BuiltInCondition = BuiltInCondition.IsEfiSystemDisk;
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsTrue(selected.Any());
                }

                {
                    selection.BuiltInCondition = BuiltInCondition.None;
                    selection.Condition = "PartitionStyle == \"Gpt\"";
                    var selected = await selection.ApplyAsync(disks, service, logger);
                    Assert.IsTrue(selected.Any());
                }
            }
        }

        private ILogger<T> CreateLogger<T>()=> Loggers.CreateLogger<T>();
        private static readonly ILoggerFactory Loggers = LoggerFactory.Create(l => l.AddDebug());
    }
}
