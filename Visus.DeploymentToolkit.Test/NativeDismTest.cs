// <copyright file="NativeDismTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Dism;

namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests the low-level DISM API projection.
    /// </summary>
    [TestClass]
    public class NativeDismTest {

        [TestMethod]
        public void DismInitialise() {
            Native.DismInitialise(DismLogLevel.ErrorsWarningsInfo, "log.txt", null);
            Native.DismShutdown();
        }
    }
}
