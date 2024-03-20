// <copyright file="TaskTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Moq;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for <see cref="ITask"/>s.
    /// </summary>
    [TestClass]
    public sealed class TaskTest {

        [TestMethod]
        public async Task TestRunCommand() {
            var task = new RunCommand(new CommandBuilderFactory(), Mock.Of<ILogger<RunCommand>>());
            task.Path = @"c:\Windows\System32\cmd.exe";
            task.Arguments = "/c @(call)";
            task.SucccessExitCodes = new[] { 1 };
            await task.ExecuteAsync(Mock.Of<IState>());
        }
    }
}
