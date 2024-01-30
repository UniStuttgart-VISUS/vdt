// <copyright file="EnvironmentServiceTest.cs" company="Visualisierungsinstitut der Universit�t Stuttgart">
// Copyright � 2024 Visualisierungsinstitut der Universit�t Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph M�ller</author>


namespace Visus.DeploymentToolkit.Test {


    [TestClass]
    public sealed class EnvironmentServiceTest {

        [TestMethod]
        public void Enumerate() {
            var service = new Services.EnvironmentService();
            Assert.IsTrue(service.Any(), "Any environment variable");
        }

        [TestMethod]
        public void Index() {
            var service = new Services.EnvironmentService();
            Assert.IsNotNull(service["COMPUTERNAME"], "COMPUTERNAME is available");
            Assert.IsNull(service["::__%%]"], "::__%%] is not available");
        }
    }
}