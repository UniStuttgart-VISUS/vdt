// <copyright file="TaskTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Principal;
using System.Xml.Linq;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Unattend;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for <see cref="ITask"/>s.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"TestData\Unattend_Core_x64.xml")]
    public sealed class TaskTest {

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public async Task TestCopyFilesFlat() {
            var state = new State(CreateLogger<State>());
            var dir = new DirectoryService(CreateLogger<DirectoryService>());
            var copy = new CopyService(dir, CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, CreateLogger<CopyFiles>());
            task.Source = ".";
            task.Destination = Path.Combine(Path.GetTempPath(), "DeimosTest2");
            task.IsRecursive = false;
            task.IsOverwrite = true;
            await task.ExecuteAsync();
            Assert.IsFalse(Directory.GetDirectories(task.Destination).Any());
        }

        [TestMethod]
        public async Task TestCopyFilesRecursive() {
            var state = new State(  CreateLogger<State>());
            var dir = new DirectoryService(CreateLogger<DirectoryService>());
            var copy = new CopyService(dir,CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, CreateLogger<CopyFiles>());
            task.Source = ".";
            task.Destination = Path.Combine(Path.GetTempPath(), "DeimosTest");
            task.IsOverwrite = true;
            await task.ExecuteAsync();
        }

        [TestMethod]
        public async Task TestRunCommand() {
            var state = new State(CreateLogger<State>());
            var task = new RunCommand(state, new CommandBuilderFactory(), CreateLogger<RunCommand>());
            task.Path = @"c:\Windows\System32\cmd.exe";
            task.Arguments = "/c @(call)";
            task.SucccessExitCodes = [ 1 ];
            await task.ExecuteAsync();
        }

        [TestMethod]
        public async Task TestCustomiseUnattend() {
            var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_Core_x64.xml");
            Assert.IsTrue(File.Exists(file));
            var state = new State(CreateLogger<State>());
            var task = new CustomiseUnattend(state, CreateLogger<CustomiseUnattend>());
            task.Path = file;
            task.OutputPath = "TestCustomiseUnattend.xml";
            task.Customisations = [
                new XmlValueCustomisation(CreateLogger < XmlValueCustomisation >()) {
                    Path = "//unattend:UILanguage",
                    Value = "de-DE",
                    IsRequired = true
                },
            ];
            await task.ExecuteAsync();

            Assert.IsTrue(File.Exists(task.OutputPath!));
            var doc = XDocument.Load(task.OutputPath!);

            {
                var elements = doc.DescendantNodes().Where(n => n is XElement e && e.Name.LocalName == "UILanguage").Select(n => (XElement) n);
                Assert.IsNotNull(elements);
                Assert.IsTrue(elements.Any());
                Assert.IsTrue(elements.All(n => n.Value == "de-DE"));
            }
        }

        [TestMethod]
        public async Task TestSetEnvironmentVariable() {
            var state = new State(CreateLogger<State>());
            var env = new EnvironmentService(CreateLogger<EnvironmentService>());

            var task = new SetEnvironmentVariable(state, env, CreateLogger<SetEnvironmentVariable>());
            task.Variable = "__DEIMOS_TEST_VARIABLE__";

            task.Value = "TestValue";
            task.IsNoOverwrite = false;
            await task.ExecuteAsync();
            Assert.AreEqual(task.Value, Environment.GetEnvironmentVariable(task.Variable));

            task.Value = "AnotherValue";
            task.IsNoOverwrite = true;
            await task.ExecuteAsync();
            Assert.AreEqual("TestValue", Environment.GetEnvironmentVariable(task.Variable));

            task.IsNoOverwrite = false;
            await task.ExecuteAsync();
            Assert.AreEqual(task.Value, Environment.GetEnvironmentVariable(task.Variable));

            task.Value = null;
            task.IsNoOverwrite = true;
            await task.ExecuteAsync();
            Assert.AreEqual("AnotherValue", Environment.GetEnvironmentVariable(task.Variable));

            task.IsNoOverwrite = false;
            await task.ExecuteAsync();
            Assert.IsNull(Environment.GetEnvironmentVariable(task.Variable));
        }

        [TestMethod]
        public async Task TestSelectInstallDisk() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var state = new State(CreateLogger<State>());
                var vds = new VdsService(Options.Create(new VdsOptions()), CreateLogger<VdsService>());
                var wmi = new ManagementService(CreateLogger<ManagementService>());
                var diskMgmt = new WmiDiskService(wmi, vds, CreateLogger<WmiDiskService>());
                var task = new SelectInstallDisk(state, diskMgmt, CreateLogger<SelectInstallDisk>());

                await task.ExecuteAsync();
                Assert.IsNotNull(state.InstallationDisk);

                // Make a selection that should yield nothing.
                task.Steps = [
                    new() {
                        Action = DiskSelectionAction.Include,
                        BuiltInCondition = BuiltInCondition.IsEmpty
                    }
                ];
                await Assert.ThrowsAsync<InvalidOperationException>(task.ExecuteAsync);
            }
        }

        [TestMethod]
        public async Task TestReinterpretState() {
            var state = new State(CreateLogger<State>());
            var task = new ReinterpretState(state, CreateLogger<ReinterpretState>());

            state.AgentPath = "horst";

            task.Source = WellKnownStates.AgentPath;
            task.Destination = WellKnownStates.DeploymentShareUser;
            await task.ExecuteAsync();

            Assert.AreEqual("horst", state.DeploymentShareUser);
        }

        [TestMethod]
        public async Task TestClearState() {
            var state = new State(CreateLogger<State>());
            var task = new ClearState(state, CreateLogger<ClearState>());

            state.AgentPath = "horst";

            task.Variable = WellKnownStates.AgentPath;
            await task.ExecuteAsync();

            Assert.IsNull(state.AgentPath);
        }

        private static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(l => l.AddDebug());
    }
}
