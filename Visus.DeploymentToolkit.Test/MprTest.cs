// <copyright file="MprTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.ComponentModel;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for the network share API.
    /// </summary>
    [TestClass]
    public sealed class MprTest {

        [TestMethod]
        public void MapUnmap() {
            if (this._secrets.NetworkShare != null) {
                var drive = new DriveInfoService().GetFreeDrives().First();
                MprApi.Connect(drive,
                    this._secrets.NetworkShare,
                    this._secrets.NetworkUser,
                    this._secrets.NetworkPassword,
                    MprApi.ConnectionFlags.Temporary);
                MprApi.Disconnect(drive);
            }
        }

        [TestMethod]
        public void MapFail() {
            var drive = new DriveInfoService().GetFreeDrives().First();
            Assert.ThrowsException<Win32Exception>(() => {
                MprApi.Connect(drive,
                    @"\\honcho3000.honcho\honcho",
                    this._secrets.NetworkUser,
                    this._secrets.NetworkPassword,
                    MprApi.ConnectionFlags.Temporary);
            });
        }

        private readonly TestSecrets _secrets = TestSecrets.Load();
    }
}
