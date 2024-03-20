// <copyright file="CommandTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for <see cref="Command"/>.
    /// </summary>
    [TestClass]
    public class CommandTest {

        [TestMethod]
        public async Task TestCallHack() {
            // Cf. https://stackoverflow.com/questions/20892882/set-errorlevel-in-windows-batch-file
            {
                var exitCode = await new CommandBuilder(@"c:\Windows\System32\cmd.exe")
                    .WithArgumentList("/c", "@(call)")
                    .Build()
                    .ExecuteAsync();
                Assert.IsNotNull(exitCode);
                Assert.AreEqual(1, exitCode);
            }

            {
                var exitCode = await new CommandBuilder(@"c:\Windows\System32\cmd.exe")
                    .WithArguments("/c @(call)")
                    .Build()
                    .ExecuteAsync();
                Assert.IsNotNull(exitCode);
                Assert.AreEqual(1, exitCode);
            }
        }

        [TestMethod]
        public async Task TestEmpty() {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => {
                await new CommandBuilder("")
                .Build()
                .ExecuteAsync();
            });
        }

        [TestMethod]
        public async Task TestHostname() {
            var exitCode = await new CommandBuilder("hostname")
                .Build()
                .ExecuteAsync();
            Assert.IsNotNull(exitCode);
            Assert.AreEqual(0, exitCode);
        }

        [TestMethod]
        public async Task TestNull() {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => {
                await new CommandBuilder(null)
                    .Build()
                    .ExecuteAsync();
            });
        }

    }
}
