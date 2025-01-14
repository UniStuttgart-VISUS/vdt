// <copyright file="CryptoTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class CryptoTest {

        [TestMethod]
        public void TestAutoKeyRoundtrip() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            ISessionSecurity security = new SessionSecurityService(state,
                this._loggerFactory.CreateLogger<SessionSecurityService>());

            var input = "Hatred is the emperor's greatest gift to humanity.";
            var encrypted = security.EncryptString(input);
            var decrypted = security.DecryptString(encrypted);
            Assert.AreEqual(input, decrypted);
        }

        [TestMethod]
        public void TestUserKeyRoundtrip() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            state.SessionKey = "super-secret password";
            ISessionSecurity security = new SessionSecurityService(state,
                this._loggerFactory.CreateLogger<SessionSecurityService>());

            var input = "Hope is the first step on the road to disappointment.";
            var encrypted = security.EncryptString(input);
            var decrypted = security.DecryptString(encrypted);
            Assert.AreEqual(input, decrypted);
        }


        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
