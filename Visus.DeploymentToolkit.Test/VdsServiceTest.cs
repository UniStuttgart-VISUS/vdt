// <copyright file="VdsServiceTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Security.Principal;
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
                var service = new VdsService(this._loggerFactory.CreateLogger<VdsService>());
                Assert.IsNotNull(service);

                var task = service.GetDisksAsync(CancellationToken.None);
                task.Wait();
                var disks = task.Result;
                Assert.IsTrue(disks.Any());

                foreach (var disk in disks) {
                    Assert.IsTrue(disk.Partitions.Any());
                }
            }
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
