// <copyright file="TaskDescriptionTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for <see cref="ITaskDescription"/>.
    /// </summary>
    [TestClass]
    public sealed class TaskDescriptionTest {

        [TestMethod]
        public void TestCopyFiles() {
            var state = new State(CreateLogger<State>());
            var dir = new DirectoryService(CreateLogger<DirectoryService>());
            var copy = new CopyService(dir, CreateLogger<CopyService>());
            var task = new CopyFiles(state, copy, CreateLogger<CopyFiles>());

            var desc = TaskDescription.FromTask(task) as ITaskDescription;
            Assert.IsNotNull(desc);
            Assert.AreEqual("Visus.DeploymentToolkit.Tasks.CopyFiles", desc.Task);

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(CopyFiles.Destination)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsTrue(p.IsRequired);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(CopyFiles.IsOverwrite)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var d = p.Sources.Where(s => s.Type == ParameterSourceType.Default).SingleOrDefault();
                Assert.IsNotNull(d);
                Assert.AreEqual(false, d.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(CopyFiles.IsRecursive)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var d = p.Sources.Where(s => s.Type == ParameterSourceType.Default).SingleOrDefault();
                Assert.IsNotNull(d);
                Assert.AreEqual(true, d.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(CopyFiles.IsRequired)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(CopyFiles.Source)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsTrue(p.IsRequired);
            }
        }


        [TestMethod]
        public void TestMountDeploymentShare() {
            var state = new State(CreateLogger<State>());
            var mount = new MountNetworkShare(state, CreateLogger<MountNetworkShare>());
            var sec = new SessionSecurityService(state, CreateLogger<SessionSecurityService>());
            var console = new ConsoleInputService();
            var drives = new DriveInfoService();
            var task = new MountDeploymentShare(state, mount, sec, console, drives, CreateLogger<MountNetworkShare>());

            var desc = TaskDescription.FromTask(task) as ITaskDescription;
            Assert.IsNotNull(desc);
            Assert.AreEqual("Visus.DeploymentToolkit.Tasks.MountDeploymentShare", desc.Task);

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.DeploymentShare)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsTrue(p.IsRequired);
                var s = p.Sources.Where(s => s.Type == ParameterSourceType.State).SingleOrDefault();
                Assert.IsNotNull(s);
                Assert.AreEqual(WellKnownStates.DeploymentShare, s.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.Domain)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var s = p.Sources.Where(s => s.Type == ParameterSourceType.State).SingleOrDefault();
                Assert.IsNotNull(s);
                Assert.AreEqual(WellKnownStates.DeploymentShareDomain, s.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.Interactive)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var d = p.Sources.Where(s => s.Type == ParameterSourceType.Default).SingleOrDefault();
                Assert.IsNotNull(d);
                Assert.AreEqual(true, d.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.MountPoint)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var s = p.Sources.Where(s => s.Type == ParameterSourceType.State).SingleOrDefault();
                Assert.IsNotNull(s);
                Assert.AreEqual(WellKnownStates.DeploymentDirectory, s.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.Password)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.PreserveConnection)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var d = p.Sources.Where(s => s.Type == ParameterSourceType.Default).SingleOrDefault();
                Assert.IsNotNull(d);
                Assert.AreEqual(false, d.Source);
            }

            {
                var p = desc.DeclaredParameters.Where(p => p.Name == nameof(MountDeploymentShare.User)).SingleOrDefault();
                Assert.IsNotNull(p);
                Assert.IsFalse(p.IsRequired);
                var s = p.Sources.Where(s => s.Type == ParameterSourceType.State).SingleOrDefault();
                Assert.IsNotNull(s);
                Assert.AreEqual(WellKnownStates.DeploymentShareUser, s.Source);
            }
        }

        [TestMethod]
        public void TestFactoryFromType() {
            var desc = TaskDescriptionFactory.FromType<CopyFiles>();
            Assert.IsNotNull(desc);
            Assert.AreEqual("Visus.DeploymentToolkit.Tasks.CopyFiles", desc.Task);
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.Destination)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.IsOverwrite)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.IsRecursive)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.IsRequired)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.Source)));
        }

        [TestMethod]
        public void TestFactoryFromTypeString() {
            var desc = TaskDescriptionFactory.FromType("Visus.DeploymentToolkit.Tasks.CopyFiles");
            Assert.IsNotNull(desc);
            Assert.AreEqual("Visus.DeploymentToolkit.Tasks.CopyFiles", desc.Task);
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.Destination)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.IsOverwrite)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.IsRecursive)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.IsRequired)));
            Assert.IsTrue(desc.DeclaredParameters.Any(p => p.Name == nameof(CopyFiles.Source)));
        }

        [TestMethod]
        public void TestFactoryFromAbstract() {
            Assert.Throws<ArgumentException>(TaskDescriptionFactory.FromType<TaskBase>);
        }

        [TestMethod]
        public void TestFactoryFromNonTask() {
            Assert.Throws<ArgumentException>(() => TaskDescriptionFactory.FromType(typeof(int)));
        }

        private static ILogger<T> CreateLogger<T>() => Loggers.CreateLogger<T>();
        private static readonly ILoggerFactory Loggers = LoggerFactory.Create(static l => l.AddDebug());

    }
}
