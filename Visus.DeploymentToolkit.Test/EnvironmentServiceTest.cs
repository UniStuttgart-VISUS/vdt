// <copyright file="EnvironmentServiceTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {


    [TestClass]
    public sealed class EnvironmentServiceTest {

        [TestMethod]
        public void Enumerate() {
            var service = new EnvironmentService(this._loggerFactory.CreateLogger<EnvironmentService>());
            Assert.IsTrue(service.Any(), "Any environment variable");
        }

        [TestMethod]
        public void Index() {
            var service = new EnvironmentService(this._loggerFactory.CreateLogger<EnvironmentService>());
            Assert.IsNotNull(service["COMPUTERNAME"], "COMPUTERNAME is available");
            Assert.IsNull(service["::__%%]"], "::__%%] is not available");
        }

        [TestMethod]
        public void Set() {
            var expected = "Hello from environment testing";
            var variable = "ProjectDeimosTest";
            var service = new EnvironmentService(this._loggerFactory.CreateLogger<EnvironmentService>());
            service[variable] = expected;
            Assert.AreEqual(expected, service[variable]);
            service[variable] = null;
            Assert.IsNull(service[variable]);
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}