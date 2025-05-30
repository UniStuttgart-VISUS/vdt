// <copyright file="TaskSequenceStoreTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for the task sequence store.
    /// </summary>
    [TestClass]
    public sealed class TaskSequenceStoreTest {

        [TestMethod]
        public void TestSerialiseCopyFiles() {
            var state = new State(CreateLogger<State>());
            var dir = new DirectoryService(CreateLogger<DirectoryService>());
            var copy = new CopyService(dir, CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, CreateLogger<CopyFiles>());
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

        [TestMethod]
        public async Task TestSerialiseWinPeSequence() {
            var services = new ServiceCollection();
            services.AddState();
            services.Configure<TaskSequenceStoreOptions>(o => {
                o.Path = Directory.GetCurrentDirectory();
            });
            services.AddLogging(s => s.AddDebug());
            services.AddDeploymentServices();
            var provider = services.BuildServiceProvider();

            var state = provider.GetRequiredService<IState>();
            Assert.IsNotNull(state);

            var task = provider.GetRequiredService<SelectWindowsPeSequence>();
            await task.ExecuteAsync();
            Assert.IsNotNull(state.TaskSequence);

            var sequence = state.TaskSequence as ITaskSequence;
            Assert.IsNotNull(sequence);

            var path = Path.GetTempFileName();
            await sequence.SaveAsync(path, "TEST", "Test Sequence", "Blah");

            var desc = await TaskSequenceDescription.ParseAsync(path);
            Assert.IsNotNull(desc);

            var builder = provider.GetRequiredService<ITaskSequenceBuilder>();
            Assert.IsNotNull(builder);

            var restored = builder.FromDescription(desc);
            Assert.IsNotNull(restored);
        }

        private static ILogger<T> CreateLogger<T>() where T : class  => LoggerFactory.CreateLogger<T>();
        private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(l => l.AddDebug());
    }
}
