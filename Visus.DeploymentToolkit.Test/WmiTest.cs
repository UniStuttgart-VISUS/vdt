// <copyright file="WmiTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test
{

    [TestClass]
    public sealed class WmiTest {

        [TestMethod]
        public async Task TestGetDisks() {
            var wmi = new ManagementService(this._loggerFactory.CreateLogger<ManagementService>());
            var result = wmi.Query("SELECT * FROM Win32_DiskDrive", null);
            Assert.IsTrue(result.Any());

            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var vds = new VdsService(this._loggerFactory.CreateLogger<VdsService>());
                var disks = await vds.GetDisksAsync(CancellationToken.None);
                Assert.AreEqual(disks.Count(), result.Count());
            }
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());

    }
}
