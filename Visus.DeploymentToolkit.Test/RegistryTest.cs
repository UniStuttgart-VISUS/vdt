// <copyright file="RegistryTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Security;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    // Cf. https://stackoverflow.com/questions/883270/problems-with-deploymentitem-attribute
    [DeploymentItem(@"TestData\Unattend_PE_x64.xm")]
    public sealed class RegistryTest {

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ValueExists() {
            var registry = new RegistryService(this._loggerFactory.CreateLogger<RegistryService>());

            {
                var value = registry.ValueExists(@"HKLM\SYSTEM\CurrentControlSet\Control\ProductOptions", "ProductType");
                Assert.IsTrue(value);
            }
        }

        [TestMethod]
        public void GetValue() {
            var registry = new RegistryService(this._loggerFactory.CreateLogger<RegistryService>());

            {
                var value = registry.GetValue(@"HKLM\SYSTEM\CurrentControlSet\Control\ProductOptions", "ProductType", null);
                Assert.IsNotNull(value);
            }
        }

        [TestMethod]
        public void KeyExists() {
            var registry = new RegistryService(this._loggerFactory.CreateLogger<RegistryService>());
            Assert.IsFalse(registry.KeyExists("hugo"));
            Assert.IsTrue(registry.KeyExists("HKEY_LOCAL_MACHINE"));
            Assert.IsTrue(registry.KeyExists("HKLM"));
            Assert.IsTrue(registry.KeyExists("hkey_current_user"));
            Assert.IsTrue(registry.KeyExists("hkcu"));
            Assert.IsTrue(registry.KeyExists(@"hklm\software"));
            Assert.IsFalse(registry.KeyExists(@"hklm\horst 3000"));
        }

        [TestMethod]
        public void MountHive() {
            var hive = Path.Combine(this.TestContext.DeploymentDirectory!, "SYSTEM");
            var registry = new RegistryService(this._loggerFactory.CreateLogger<RegistryService>());

            Advapi32.AdjustTokenPrivileges("SeBackupPrivilege", true);
            Advapi32.AdjustTokenPrivileges("SeRestorePrivilege", true);

            registry.LoadHive(hive, @"hklm\test");
            registry.UnloadHive(@"hklm\test");
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
