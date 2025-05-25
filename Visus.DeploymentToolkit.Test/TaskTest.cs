// <copyright file="TaskTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Xml.Linq;
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

        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task TestCopyFilesFlat() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            var dir = new DirectoryService(this._loggerFactory.CreateLogger<DirectoryService>());
            var copy = new CopyService(dir, this._loggerFactory.CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, this._loggerFactory.CreateLogger<CopyFiles>());
            task.Source = ".";
            task.Destination = Path.Combine(Path.GetTempPath(), "DeimosTest2");
            task.IsRecursive = false;
            task.IsOverwrite = true;
            await task.ExecuteAsync();
            Assert.IsFalse(Directory.GetDirectories(task.Destination).Any());
        }

        [TestMethod]
        public async Task TestCopyFilesRecursive() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            var dir = new DirectoryService(this._loggerFactory.CreateLogger<DirectoryService>());
            var copy = new CopyService(dir,this._loggerFactory.CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, this._loggerFactory.CreateLogger<CopyFiles>());
            task.Source = ".";
            task.Destination = Path.Combine(Path.GetTempPath(), "DeimosTest");
            task.IsOverwrite = true;
            await task.ExecuteAsync();
        }

        [TestMethod]
        public async Task TestRunCommand() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            var task = new RunCommand(state, new CommandBuilderFactory(), this._loggerFactory.CreateLogger<RunCommand>());
            task.Path = @"c:\Windows\System32\cmd.exe";
            task.Arguments = "/c @(call)";
            task.SucccessExitCodes = [ 1 ];
            await task.ExecuteAsync();
        }

        [TestMethod]
        public async Task TestCustomiseUnattend() {
            var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_Core_x64.xml");
            Assert.IsTrue(File.Exists(file));
            var state = new State(this._loggerFactory.CreateLogger<State>());
            var task = new CustomiseUnattend(state, this._loggerFactory.CreateLogger<CustomiseUnattend>());
            task.Path = file;
            task.OutputPath = "TestCustomiseUnattend.xml";
            task.Customisations = [
                new XmlValueCustomisation(this._loggerFactory.CreateLogger<XmlValueCustomisation>()) {
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

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
