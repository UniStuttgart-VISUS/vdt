// <copyright file="DriveInfoTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public class DriveInfoTest {

        [TestMethod]
        public void GetDriveLetters() {
            var service = new DriveInfoService();
            var letters = service.GetLogicalDrives();
            var expected = Environment.GetLogicalDrives();
            Assert.IsTrue(letters.SequenceEqual(expected));
        }

        [TestMethod]
        public void GetFreeDriveLetters() {
            var service = new DriveInfoService();
            var letters = service.GetFreeDrives();
            var used = Environment.GetLogicalDrives();
            Assert.IsFalse(letters.Any(l => used.Contains(l)));
        }
    }
}
