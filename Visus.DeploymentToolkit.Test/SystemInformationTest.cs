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

        [TestMethod]
        public void OperatingSystem() {
            var registry = new RegistryService(this._loggerFactory.CreateLogger<RegistryService>());
            var sysInfo = new SystemInformationService(registry, this._loggerFactory.CreateLogger<SystemInformationService>());

            Assert.AreEqual(PlatformID.Win32NT, sysInfo.OperatingSystemPlatform);
            Assert.IsFalse(sysInfo.IsWinPE);
            Assert.IsFalse(sysInfo.IsServerCore);
        }


        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
