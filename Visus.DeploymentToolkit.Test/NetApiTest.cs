// <copyright file="NetApiTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.ComponentModel;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for creating netowkr shares.
    /// </summary>
    [TestClass]
    public sealed class NetApiTest {

        [TestMethod]
        public void ShareUnshare() {
            try {
                NetApi.ShareFolder(null, "test$", Directory.GetCurrentDirectory());
                Assert.IsTrue(Directory.Exists(@"\\localhost\test$"));
            } finally {
                NetApi.Unshare(null, "test$");
            }
        }

        [TestMethod]
        public void MapFail() {
            Assert.ThrowsException<Win32Exception>(() => {
                NetApi.ShareFolder(null, "test$", "fwrtg2tg$'!Ä$KFWÖGJ");
            });

            Assert.ThrowsException<ArgumentException>(() => {
                var share = new NetApi.SHARE_INFO_2 {
                    shi2_netname = "test$",
                    shi2_path = Directory.GetCurrentDirectory(),
                    shi2_type = NetApi.ShareType.Mask
                };

                NetApi.Share(null, ref share);
            });
        }

        private readonly TestSecrets _secrets = TestSecrets.Load();
    }
}
