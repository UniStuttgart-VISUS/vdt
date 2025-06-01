// <copyright file="BcdTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Visus.DeploymentToolkit.Bcd;
using Visus.DeploymentToolkit.Security;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class BcdTest {

        [TestMethod]
        public void TestBcdObjectType() {
            // Test against constants from https://www.geoffchappell.com/notes/windows/boot/bcd/objects.htm
            Assert.AreEqual(0xF0000000u, (uint) BcdObjectType.ObjectTypeMask);
            Assert.AreEqual(0x00F00000u, (uint) BcdObjectType.ImageTypeMask);
            Assert.AreEqual(0x00F00000u, (uint) BcdObjectType.InheritableMask);
            
            Assert.AreEqual(0x000FFFFFu, (uint) BcdObjectType.ApplicationTypeMask);
            Assert.AreEqual(0x10100001u, (uint) BcdObjectType.FirmwareBootManager);
            Assert.AreEqual(0x10100002u, (uint) BcdObjectType.BootManager);
            Assert.AreEqual(0x10200003u, (uint) BcdObjectType.OperatingSystemLoader);
            Assert.AreEqual(0x10200004u, (uint) BcdObjectType.Resume);
            Assert.AreEqual(0x10200005u, (uint) BcdObjectType.MemoryDiagnostic);
            Assert.AreEqual(0x10300006u, (uint) BcdObjectType.NtLoader);
            Assert.AreEqual(0x10400008u, (uint) BcdObjectType.Bootsector);
            Assert.AreEqual(0x10400009u, (uint) BcdObjectType.Startup);
            Assert.AreEqual(0x1020000Au, (uint) BcdObjectType.BootApp);

            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.Inherit);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.BadMemory);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.DebugSettings);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.EmsSettings);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.GlobalSettings);
            Assert.AreEqual(0x20200001u, (uint) BcdObjectType.InheritFirmwareBootManager);
            Assert.AreEqual(0x20200002u, (uint) BcdObjectType.InheritBootManager);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.InheritOperatingSystemLoader);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.BootLoaderSettings);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.HypervisorSettings);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.KernelDebuggerSettings);
            Assert.AreEqual(0x20200004u, (uint) BcdObjectType.InheritResume);
            Assert.AreEqual(0x20200004u, (uint) BcdObjectType.ResumeLoaderSettings);
            Assert.AreEqual(0x20200005u, (uint) BcdObjectType.InheritMemoryDiagnostic);
            Assert.AreEqual(0x20200006u, (uint) BcdObjectType.InheritNtLoader);
            Assert.AreEqual(0x20200007u, (uint) BcdObjectType.InheritSetupLoader);
            Assert.AreEqual(0x20200008u, (uint) BcdObjectType.InheritBootsector);
            Assert.AreEqual(0x20200009u, (uint) BcdObjectType.InheritStartup);
            Assert.AreEqual(0x20300000u, (uint) BcdObjectType.InheritDevice);
        }


        [TestMethod]
        public void TestCreateRegistryBcdStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                path = $"{Path.GetPathRoot(path)}{Path.GetFileName(Path.GetTempFileName())}";

                using (var seBackup = new TokenPrivilege("SeBackupPrivilege"))
                using (var seRestore = new TokenPrivilege("SeRestorePrivilege")) {
                    try {
                        using var bcd = RegistryBcdStore.Create(path);
                        Assert.IsNotNull(bcd);
                    } finally {
                        File.Delete(path);
                    }
                }
            }
        }

        [TestMethod]
        public void TestCreateBcdStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var wmi = new ManagementService(GetLogger<ManagementService>());
                var boot = new BootService(wmi, GetLogger<BootService>());

                var path = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                path = $"{Path.GetPathRoot(path)}{Path.GetFileName(Path.GetTempFileName())}";

                var store = boot.CreateBcdStore(path);
                Assert.IsNotNull(store);
                Assert.IsTrue(File.Exists(path));

                try {
                    var opened = boot.OpenBcdStore(path);
                    Assert.IsNotNull(opened);
                } finally {
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void TestOpenSystemStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var wmi = new ManagementService(GetLogger<ManagementService>());
                var boot = new BootService(wmi, GetLogger<BootService>());

                var store = boot.OpenBcdStore(null);
                Assert.IsNotNull(store);

                var p = store.GetMethodParameters("EnumerateObjects");
                p["Type"] = 0x10200003;
                var r = store.InvokeMethod("EnumerateObjects", p, null);
                var retval = r["ReturnValue"];
                var objs = r["Objects"];
            }
        }

        private static ILogger<T> GetLogger<T>() => Loggers.CreateLogger<T>();
        private static readonly ILoggerFactory Loggers = LoggerFactory.Create(static l => l.AddDebug());
    }
}
