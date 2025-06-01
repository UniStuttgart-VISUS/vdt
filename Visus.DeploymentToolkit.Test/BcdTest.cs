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
        public void TestWellKnownBcdObjects() {
            Assert.AreEqual(Guid.Parse("{0CE4991B-E6B3-4B16-B23C-5E0D9250E5D9}"), WellKnownBcdObject.EmsSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{1AFA9C49-16AB-4A5C-4A90-212802DA9460}"), WellKnownBcdObject.ResumeLoaderSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{1CAE1EB7-A0DF-4D4D-9851-4860E34EF535}"), WellKnownBcdObject.DefaultBootEntry.GetGuid());
            Assert.AreEqual(Guid.Parse("{313E8EED-7098-4586-A9BF-309C61F8D449}"), WellKnownBcdObject.KernelDebuggerSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{4636856E-540F-4170-A130-A84776F4C654}"), WellKnownBcdObject.DebuggerSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{466F5A88-0AF2-4F76-9038-095B170DC21C}"), WellKnownBcdObject.WindowsLegacyNtldr.GetGuid());
            Assert.AreEqual(Guid.Parse("{5189B25C-5558-4BF2-BCA4-289B11BD29E2}"), WellKnownBcdObject.BadMemoryGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{6EFB52BF-1766-41DB-A6B3-0EE5EFF72BD7}"), WellKnownBcdObject.BootLoaderSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{7254A080-1510-4E85-AC0F-E7FB3D444736}"), WellKnownBcdObject.WindowsSetupEfi.GetGuid());
            Assert.AreEqual(Guid.Parse("{7EA2E1AC-2E61-4728-AAA3-896D9D0A9F0E}"), WellKnownBcdObject.GlobalSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{7FF607E0-4395-11DB-B0DE-0800200C9A66}"), WellKnownBcdObject.HypervisorSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{9DEA862C-5CDD-4E70-ACC1-F32B344D4795}"), WellKnownBcdObject.WindowsBootmgr.GetGuid());
            Assert.AreEqual(Guid.Parse("{A1943BBC-EA85-487C-97C7-C9EDE908A38A}"), WellKnownBcdObject.WindowsOsTargetTemplatePcat.GetGuid());
            Assert.AreEqual(Guid.Parse("{A5A30FA2-3D06-4E9F-B5F4-A01DF9D1FCBA}"), WellKnownBcdObject.FirmwareBootmgr.GetGuid());
            Assert.AreEqual(Guid.Parse("{AE5534E0-A924-466C-B836-758539A3EE3A}"), WellKnownBcdObject.WindowsSetupRamdiskOptions.GetGuid());
            Assert.AreEqual(Guid.Parse("{B012B84D-C47C-4ED5-B722-C0C42163E569}"), WellKnownBcdObject.WindowsOsTargetTemplateEfi.GetGuid());
            Assert.AreEqual(Guid.Parse("{B2721D73-1DB4-4C62-BF78-C548A880142D}"), WellKnownBcdObject.WindowsMemoryTester.GetGuid());
            Assert.AreEqual(Guid.Parse("{CBD971BF-B7B8-4885-951A-FA03044F5D71}"), WellKnownBcdObject.WindowsSetupPcat.GetGuid());
            Assert.AreEqual(Guid.Parse("{FA926493-6F1C-4193-A414-58F0B2456D1E}"), WellKnownBcdObject.CurrentBootEntry.GetGuid());

            Assert.IsTrue(WellKnownBcdObject.EmsSettingsGroup.GetNames().Contains("{emssettings}"));
            Assert.IsTrue(WellKnownBcdObject.ResumeLoaderSettingsGroup.GetNames().Contains("{resumeloadersettings}"));
            Assert.IsTrue(WellKnownBcdObject.DefaultBootEntry.GetNames().Contains("{default}"));
            Assert.IsTrue(WellKnownBcdObject.KernelDebuggerSettingsGroup.GetNames().Contains("{kerneldbgsettings}"));
            Assert.IsTrue(WellKnownBcdObject.DebuggerSettingsGroup.GetNames().Contains("{dbgsettings}"));
            Assert.IsTrue(WellKnownBcdObject.DebuggerSettingsGroup.GetNames().Contains("{eventsettings}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsLegacyNtldr.GetNames().Contains("{legacy}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsLegacyNtldr.GetNames().Contains("{ntldr}"));
            Assert.IsTrue(WellKnownBcdObject.BadMemoryGroup.GetNames().Contains("{badmemory}"));
            Assert.IsTrue(WellKnownBcdObject.BootLoaderSettingsGroup.GetNames().Contains("{bootloadersettings}"));
            Assert.IsTrue(WellKnownBcdObject.GlobalSettingsGroup.GetNames().Contains("{globalsettings}"));
            Assert.IsTrue(WellKnownBcdObject.HypervisorSettingsGroup.GetNames().Contains("{hypervisorsettings}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsBootmgr.GetNames().Contains("{bootmgr}"));
            Assert.IsTrue(WellKnownBcdObject.FirmwareBootmgr.GetNames().Contains("{fwbootmgr}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsSetupRamdiskOptions.GetNames().Contains("{ramdiskoptions}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsMemoryTester.GetNames().Contains("{memdiag}"));
            Assert.IsTrue(WellKnownBcdObject.CurrentBootEntry.GetNames().Contains("{current}"));
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
