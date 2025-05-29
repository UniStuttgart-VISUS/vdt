// <copyright file="TaskSequenceStoreTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Unattend;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for the task sequence store.
    /// </summary>
    [TestClass]
    public sealed class TaskSequenceStoreTest {


        [TestMethod]
        public void TestSerialiseCopyFiles() {
            var state = new State(this._loggerFactory.CreateLogger<State>());
            var dir = new DirectoryService(this._loggerFactory.CreateLogger<DirectoryService>());
            var copy = new CopyService(dir, this._loggerFactory.CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, this._loggerFactory.CreateLogger<CopyFiles>());
            task.Source = ".";
            task.Destination = Path.Combine(Path.GetTempPath(), "DeimosTest2");
            task.IsRecursive = false;
            task.IsOverwrite = true;

            var desc = TaskDescription.FromTask(task);
            Assert.IsNotNull(desc);
            Assert.AreEqual(task.GetType().FullName, desc.Task);
            Assert.AreEqual(task.Source, desc.Parameters[nameof(task.Source)]);
            Assert.AreEqual(task.Destination, desc.Parameters[nameof(task.Destination)]);
            Assert.AreEqual(task.IsRecursive, desc.Parameters[nameof(task.IsRecursive)]);
            Assert.AreEqual(task.IsOverwrite, desc.Parameters[nameof(task.IsOverwrite)]);
            Assert.AreEqual(task.IsRequired, desc.Parameters[nameof(task.IsRequired)]);
            Assert.AreEqual(task.IsCritical, desc.Parameters[nameof(task.IsCritical)]);

            var file = Path.GetTempFileName();
            using (var f = File.OpenWrite(file)) {
                JsonSerializer.Serialize(f, desc);
            }

            using (var f = File.OpenRead(file)) {
                var restored = JsonSerializer.Deserialize<TaskDescription>(File.OpenRead(file));
                Assert.IsNotNull(restored);
                Assert.AreEqual(desc.Task, restored.Task);
                Assert.AreEqual(desc.Parameters[nameof(task.Source)], desc.Parameters[nameof(task.Source)]);
                Assert.AreEqual(desc.Parameters[nameof(task.Destination)], desc.Parameters[nameof(task.Destination)]);
                Assert.AreEqual(desc.Parameters[nameof(task.IsRecursive)], desc.Parameters[nameof(task.IsRecursive)]);
                Assert.AreEqual(desc.Parameters[nameof(task.IsOverwrite)], desc.Parameters[nameof(task.IsOverwrite)]);
                Assert.AreEqual(desc.Parameters[nameof(task.IsRequired)], desc.Parameters[nameof(task.IsRequired)]);
                Assert.AreEqual(desc.Parameters[nameof(task.IsCritical)], desc.Parameters[nameof(task.IsCritical)]);
            }
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }
}
