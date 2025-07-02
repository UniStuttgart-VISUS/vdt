// <copyright file="StringExtensionsTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Extensions;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests extension methods for strings.
    /// </summary>
    [TestClass]
    public sealed class StringExtensionsTest {

        [TestMethod]
        public void TestSplitAccount() {
            {
                var (domain, account) = "DOMAIN\\Account".SplitAccount();
                Assert.AreEqual("DOMAIN", domain);
                Assert.AreEqual("Account", account);
            }

            {
               var (domain, account) = "Account".SplitAccount();
                Assert.IsNull(domain);
                Assert.AreEqual("Account", account);
            }

            {
                var (domain, account) = "Account@DOMAIN".SplitAccount();
                Assert.AreEqual("DOMAIN", domain);
                Assert.AreEqual("Account", account);
            }

            {
                var (domain, account) = "Account.DOMAIN".SplitAccount();
                Assert.AreEqual("DOMAIN", domain);
                Assert.AreEqual("Account", account);
            }

            {
                var (domain, account) = "Account.DOMAIN.TLD".SplitAccount();
                Assert.AreEqual("DOMAIN.TLD", domain);
                Assert.AreEqual("Account", account);
            }

            {
                var (domain, account) = "".SplitAccount();
                Assert.IsNull(domain);
                Assert.AreEqual("", account);
            }

            {
                var (domain, account) = ((string) null!).SplitAccount();
                Assert.IsNull(domain);
                Assert.IsNull(account);
            }

            {
                var (domain, account) = "@".SplitAccount();
                Assert.IsNull(domain);
                Assert.AreEqual("@", account);
            }
        }

    }
}
