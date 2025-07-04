// <copyright file="NetApiTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.ComponentModel;
using System.Security.Principal;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for creating netowkr shares.
    /// </summary>
    [TestClass]
    public sealed class NetApiTest {

        [TestMethod]
        public void TestShareUnshare() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                try {
                    NetApi.Unshare(null, "test$");
                } catch { /* This is expected. */ }

                try {
                    NetApi.ShareFolder(null, "test$", Directory.GetCurrentDirectory());
                    Assert.IsTrue(Directory.Exists(@"\\localhost\test$"));
                } finally {
                    NetApi.Unshare(null, "test$");
                }
            } else {
                Assert.Inconclusive("This test requires administrator privileges.");
            }
        }

        [TestMethod]
        public void TestMapFail() {
            Assert.ThrowsException<Win32Exception>(() => {
                NetApi.ShareFolder(null, "test$", "fwrtg2tg$'!Ä$KFWÖGJ");
            });

            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                Assert.ThrowsException<ArgumentException>(() => {
                    var share = new NetApi.SHARE_INFO_2 {
                        shi2_netname = "test$",
                        shi2_path = Directory.GetCurrentDirectory(),
                        shi2_type = NetApi.ShareType.Mask
                    };

                    NetApi.Share(null, ref share);
                });
            } else {
                Assert.Inconclusive("This test requires administrator privileges.");
            }
        }

        [TestMethod]
        public void TestDcGetName() {
            if (this._secrets.CanTestDomain) {
                var guid = Guid.Empty;
                var dcName = NetApi.DsGetDcName(null,
                    this._secrets.Domain!,
                    null,
                    null,
                    NetApi.GetDcNameFlags.IsDnsName);
                Assert.IsNotNull(dcName);
            } else {
                Assert.Inconclusive("This test requires a domain name.");
            }
        }

        private readonly TestSecrets _secrets = TestSecrets.Load();
    }
}
