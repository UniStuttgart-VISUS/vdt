// <copyright file="BcdTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class BcdTest {

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
