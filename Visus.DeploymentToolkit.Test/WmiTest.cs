// <copyright file="WmiTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class WmiTest {

        [TestMethod]
        public void TestBcdStore() {
            var wmi = new ManagementService(CreateLogger<ManagementService>());

            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var result = wmi.GetObject("BcdStore.FilePath=''", wmi.WmiScope, default);
                Assert.IsNotNull(result);
            }

            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var result = wmi.GetClass("BcdStore", wmi.WmiScope, default);
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public async Task TestGetDisksAsync() {
            var wmi = new ManagementService(CreateLogger<ManagementService>());
            var result = wmi.Query("SELECT * FROM Win32_DiskDrive", null);
            Assert.IsTrue(result.Any());

            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var vds = new VdsService(CreateLogger<VdsService>());
                var wmid = new WmiDiskService(wmi, vds, CreateLogger<WmiDiskService>());
                var disks = await wmid.GetDisksAsync(CancellationToken.None);
                Assert.AreEqual(disks.Count(), result.Count());
            }
        }

        private static ILogger<T> CreateLogger<T>() => Loggers.CreateLogger<T>();
        private static readonly ILoggerFactory Loggers = LoggerFactory.Create(l => l.AddDebug());

    }
}
