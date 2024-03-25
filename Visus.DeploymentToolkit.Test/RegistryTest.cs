// <copyright file="RegistryTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class RegistryTest {

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

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
