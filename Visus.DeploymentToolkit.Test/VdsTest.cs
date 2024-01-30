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

            IEnumVdsObject enumProviders;
            service.QueryProviders(VDS_QUERY_PROVIDER_FLAG.HARDWARE_PROVIDERS
                | VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS
                | VDS_QUERY_PROVIDER_FLAG.VIRTUALDISK_PROVIDERS,
                out enumProviders);
            Assert.IsNotNull(enumProviders, "Have enumerator for provider");
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
            service.QueryProviders(VDS_QUERY_PROVIDER_FLAG.HARDWARE_PROVIDERS, out enumProviders);
            Assert.IsNotNull(enumProviders, "Have enumerator for provider");

            while (true) {
                enumProviders.Next(1, out var unknown, out uint cnt);

                if (cnt == 0) {
                    break;
                }
            }

            
        }
    }
}
