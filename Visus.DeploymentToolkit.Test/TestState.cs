// <copyright file="TestState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class TestState {

        [TestMethod]
        public void Properties() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            Assert.IsNull(state.DeploymentShare);
            Assert.AreEqual(Phase.Unknown, state.Phase);

            state.DeploymentShare = "test";
            Assert.AreEqual("test", state.DeploymentShare);

            state.Phase = Phase.PreinstalledEnvironment;
            Assert.AreEqual(Phase.PreinstalledEnvironment, state.Phase);
        }

        [TestMethod]
        public async Task Save() {
            var state = new ServiceCollection()
                .AddLogging()
                .AddState()
                .BuildServiceProvider()
                .GetRequiredService<IState>();

            state.Set(WellKnownStates.DeploymentShare, "x:\\");
            Assert.AreEqual("x:\\", state.DeploymentShare);

            state.Set(WellKnownStates.Phase, Phase.Installation);
            Assert.AreEqual(Phase.Installation, state.Phase);

            await state.SaveAsync("state.json");

            var restored = new ServiceCollection()
                .AddLogging()
                .AddState("state.json")
                .BuildServiceProvider()
                .GetRequiredService<IState>();

            Assert.AreEqual(state.DeploymentShare, restored.DeploymentShare);
            Assert.AreEqual(state.Phase, restored.Phase);
        }


        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());

    }
}
