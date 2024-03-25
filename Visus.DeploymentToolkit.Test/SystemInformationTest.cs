// <copyright file="SystemInformationTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class SystemInformationTest {

        public SystemInformationTest() {
            var loggers = LoggerFactory.Create(l => l.AddDebug());
            var registry = new RegistryService(loggers.CreateLogger<RegistryService>());
            var wmi = new ManagementService(loggers.CreateLogger<ManagementService>());
            this._sysInfo = new SystemInformationService(registry, wmi, loggers.CreateLogger<SystemInformationService>()); ;
        }

        [TestMethod]
        public void Hal() {
            Assert.IsNotNull(this._sysInfo.Hal);
        }

        [TestMethod]
        public void HostName() {
            Assert.AreEqual(Environment.MachineName, this._sysInfo.HostName);
        }

        [TestMethod]
        public void OperatingSystem() {
            Assert.AreEqual(PlatformID.Win32NT, this._sysInfo.OperatingSystemPlatform);
            Assert.IsFalse(this._sysInfo.IsWinPE);
            Assert.IsFalse(this._sysInfo.IsServerCore);
        }

        private readonly SystemInformationService _sysInfo;
    }
}
