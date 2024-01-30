// <copyright file="VdsTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class VdsTest {

        [TestMethod]
        public void BasicObjects() {
            var loader = new VdsServiceLoader() as IVdsServiceLoader;
            Assert.IsNotNull(loader, "Have IVdsServiceLoader");

            IVdsService service;
            loader.LoadService(null, out service);
            Assert.IsNotNull(service, "Have IVdsService");

            {
                var status = service.WaitForServiceReady();
                Assert.AreEqual(0u, status, "WaitForServiceReady succeeded");
            }
        }

        [TestMethod]
        public void EnumerateHardware() {
            var loader = new VdsServiceLoader() as IVdsServiceLoader;
            Assert.IsNotNull(loader, "Have IVdsServiceLoader");

            IVdsService service;
            loader.LoadService(null, out service);
            Assert.IsNotNull(service, "Have IVdsService");

            {
                var status = service.WaitForServiceReady();
                Assert.AreEqual(0u, status, "WaitForServiceReady succeeded");
            }

            IEnumVdsObject enumProviders;
            service.QueryProviders(VDS_QUERY_PROVIDER_FLAG.VIRTUALDISK_PROVIDERS,
                out enumProviders);
            Assert.IsNotNull(enumProviders, "Have enumerator for provider");

            enumProviders.Reset();

            {
                enumProviders.Next(1, out var unknown, out uint cnt);
                Assert.AreEqual(1u, cnt, "At least one provider found");

                var provider = (IVdsVdProvider) unknown;
            }
        }
    }
}
