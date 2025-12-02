// <copyright file="InfParserTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.InfParser;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Test of the INF file parser.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"TestData\amdpsp.inf")]
    public sealed class InfParserTest {

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public void TestParseFile() {
            var file = Path.Combine(this.TestContext.DeploymentDirectory!, "amdpsp.inf");
            var content = File.ReadAllText(file);
            var sections = Parser.ParseFile(file);

            Assert.IsTrue(sections.ContainsKey("Version"), "Have [Version]");
            Assert.IsTrue(sections.ContainsKey("DestinationDirs"), "Have [[DestinationDirs]]");
            Assert.IsTrue(sections.ContainsKey("SourceDisksNames"), "Have [SourceDisksNames]");
            Assert.IsTrue(sections.ContainsKey("SourceDisksFiles"), "Have [SourceDisksFiles]");
            Assert.IsTrue(sections.ContainsKey("Manufacturer"), "Have [Manufacturer]");
            Assert.IsTrue(sections.ContainsKey("AMDMfg.NTamd64"), "Have [AMDMfg.NTamd64]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_10"), "Have [amdpsp_Device_10]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_20"), "Have [amdpsp_Device_20]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_30"), "Have [amdpsp_Device_30]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_100"), "Have [amdpsp_Device_100]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_110"), "Have [amdpsp_Device_110]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_120"), "Have [amdpsp_Device_120]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_AddReg"), "Have [amdpsp_Device_AddReg]");
            Assert.IsTrue(sections.ContainsKey("AMDPSP.PCI"), "Have [AMDPSP.PCI]");
            Assert.IsTrue(sections.ContainsKey("AMDPSP.AMDTEE64"), "Have [AMDPSP.AMDTEE64]");
            Assert.IsTrue(sections.ContainsKey("AMDPSP.AMDTEE32"), "Have [AMDPSP.AMDTEE32]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_10.Services"), "Have [amdpsp_Device_10.Services]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_20.Services"), "Have [amdpsp_Device_20.Services]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_30.Services"), "Have [amdpsp_Device_30.Services]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_100.Services"), "Have [amdpsp_Device_100.Services]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_110.Services"), "Have [amdpsp_Device_110.Services]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_120.Services"), "Have [amdpsp_Device_120.Services]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Service_Inst"), "Have [amdpsp_Service_Inst]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_100.HW"), "Have [amdpsp_Device_100.HW]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_110.HW"), "Have [amdpsp_Device_110.HW]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_HW_AddReg"), "Have [amdpsp_Device_HW_AddReg]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_100.Wdf"), "Have [amdpsp_Device_100.Wdf]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_Device_110.Wdf"), "Have [amdpsp_Device_110.Wdf]");
            Assert.IsTrue(sections.ContainsKey("amdpsp_wdfsect"), "Have [amdpsp_wdfsect]");
            Assert.IsTrue(sections.ContainsKey("Strings"), "Have [Strings]");

            var version = sections["Version"] as KeyValueSection;
            Assert.IsNotNull(version, "[Version] is KeyValueSection");
            var driverVersion = version["DriverVer"] as DriverVersion;
            Assert.IsNotNull(driverVersion, "DriverVer is available");
            Assert.AreEqual(new DateTime(2021, 6, 11), driverVersion.Date);
            Assert.AreEqual(5, driverVersion.Major);
            Assert.AreEqual(17, driverVersion.Minor);
            Assert.AreEqual(0, driverVersion.Build);
            Assert.AreEqual(0, driverVersion.Revision);
        }
    }
}
