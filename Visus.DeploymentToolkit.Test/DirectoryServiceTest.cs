// <copyright file="DirectoryServiceTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Test of the directory services.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"TestData\amdpsp.inf")]
    [DeploymentItem(@"TestData\Unattend_x64.xml")]
    public sealed class DirectoryServiceTest {

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public async Task TestHash() {
            var service = new DirectoryService(this._loggerFactory.CreateLogger<DirectoryService>());

            {
                var hash = await service.HashAsync(this.TestContext.DeploymentDirectory!, HashItemsFlags.Recursive);
                Assert.IsNotNull(hash, "Hash is valid.");
            }

            {
                var hash = await service.HashAsync(this.TestContext.DeploymentDirectory!, HashItemsFlags.Recursive | HashItemsFlags.NamesOnly);
                Assert.IsNotNull(hash, "Hash is valid.");
            }
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
