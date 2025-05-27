// <copyright file="UnattendCustomisationTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.SystemInformation;
using Visus.DeploymentToolkit.Unattend;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for implementations of <see cref="ICustomisation"/>.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"TestData\Unattend_Core_x64.xml")]
    [DeploymentItem(@"TestData\Unattend_PE_x64.xml")]
    [DeploymentItem(@"TestData\Unattend_PE_x64_damaged.xml")]
    public sealed class UnattendCustomisationTest {

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public void TestInputLayout() {
            Assert.AreEqual("0407:00000407",InputProfiles.ForCulture(new CultureInfo("de-DE")));
        }

        [TestMethod]
        public void TestDescripionFactory() {
            var customisation = new XmlAttributeCustomisation(this._loggerFactory.CreateLogger<XmlAttributeCustomisation>()) {
                Path = "/unattend:unattend/unattend:settings",
                Name = "pass",
                Value = "horst",
                IsRequired = true
            };

            var desc = CustomisationDescription.Create(customisation);
            Assert.IsNotNull(desc);
            Assert.AreEqual(customisation.GetType().FullName, desc.Type);
            Assert.AreEqual("/unattend:unattend/unattend:settings", desc.Parameters[nameof(customisation.Path)]);
            Assert.AreEqual("pass", desc.Parameters[nameof(customisation.Name)]);
            Assert.AreEqual("horst", desc.Parameters[nameof(customisation.Value)]);
            Assert.AreEqual(true, desc.Parameters[nameof(customisation.IsRequired)]);
        }

        [TestMethod]
        public void TestXmlAttributeCustomisation() {
            var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_Core_x64.xml");
            Assert.IsTrue(File.Exists(file));

            using var reader = new XmlTextReader(file);
            var doc = XDocument.Load(reader);
            Assert.IsNotNull(doc);

            {
                var customisation = new XmlAttributeCustomisation(this._loggerFactory.CreateLogger<XmlAttributeCustomisation>()) {
                    Path = "/unattend:unattend/unattend:settings",
                    Name = "pass",
                    Value = "horst",
                    IsRequired = true
                };
                customisation.Apply(doc);
            }

            {
                var settings = doc.DescendantNodes().Where(n => n is XElement e && e.Name.LocalName == "settings").Select(n => (XElement) n);
                Assert.IsNotNull(settings);
                Assert.IsTrue(settings.Any());

                foreach (var s in settings) {
                    Assert.IsTrue(s.HasAttributes);
                    Assert.IsNotNull(s.Attribute("pass"));
                    Assert.AreEqual("horst", s.Attribute("pass")!.Value);
                }
            }

            Assert.Throws<ArgumentException>(() => {
                var customisation = new XmlAttributeCustomisation(this._loggerFactory.CreateLogger<XmlAttributeCustomisation>()) {
                    Path = "/unattend:unattend/unattend:hugo3000",
                    Name = "pass",
                    Value = "horst",
                    IsRequired = true
                };
                customisation.Apply(doc);
            });
        }

        [TestMethod]
        public void TestXmlValueCustomisation() {
            var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_Core_x64.xml");
            Assert.IsTrue(File.Exists(file));

            using var reader = new XmlTextReader(file);
            var doc = XDocument.Load(reader);
            Assert.IsNotNull(doc);

            {
                var customisation = new XmlValueCustomisation(this._loggerFactory.CreateLogger<XmlValueCustomisation>()) {
                    Path = "//unattend:component[contains(@name, 'Microsoft-Windows-International-Core-WinPE')]/unattend:SetupUILanguage/unattend:UILanguage",
                    Value = "de-DE",
                    IsRequired = true
                };
                customisation.Apply(doc);
            }

            {
                var nodes = doc.DescendantNodes().Where(n => n is XElement e && e.Name.LocalName == "UILanguage").Select(n => (XElement) n);
                Assert.IsNotNull(nodes);
                Assert.IsTrue(nodes.Any());
                Assert.IsTrue(nodes.Any(n => n.Value == "de-DE"));
            }

            Assert.Throws<ArgumentException>(() => {
                var customisation = new XmlValueCustomisation(this._loggerFactory.CreateLogger<XmlValueCustomisation>()) {
                    Path = "//unattend:component[contains(@name, 'saddam')]/unattend:SetupUILanguage/unattend:UILanguage",
                    Value = "de-DE",
                    IsRequired = true
                };
                customisation.Apply(doc);
            });
        }

        [TestMethod]
        public void TestLocalisationCustomisation() {
            var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_Core_x64.xml");
            Assert.IsTrue(File.Exists(file));

            var doc = XDocument.Load(file, LoadOptions.None);
            Assert.IsNotNull(doc);

            {
                var builder = new UnattendBuilder(Options.Create<UnattendBuilderOptions>(new()));
                var logger = this._loggerFactory.CreateLogger<LocalisationCustomisation>();
                var customisation = new LocalisationCustomisation(builder, logger) {
                    InputLocale = new CultureInfo("de-DE"),
                    InputProfile = "0407:00000407",
                    UserInterfaceLanguage = new CultureInfo("de-DE"),
                    SetupLocale = new CultureInfo("de-DE"),
                    SystemLocale = new CultureInfo("de-DE"),
                    UserLocale = new CultureInfo("de-DE")
                };
                customisation.Apply(doc);
            }
        }

        [TestMethod]
        public void TestRunCommandCustomisation() {
            var cmd = @"C:\Windows\System32\cmd.exe /c echo Hello World!";
            var desc = "Test command";

            {
                var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_PE_x64.xml");
                Assert.IsTrue(File.Exists(file));

                var doc = XDocument.Load(file, LoadOptions.None);
                Assert.IsNotNull(doc);

                var builder = new UnattendBuilder(Options.Create<UnattendBuilderOptions>(new()));
                var logger = this._loggerFactory.CreateLogger<RunCommandCustomisation>();
                var customisation = new RunCommandCustomisation(builder, logger) {
                    Description = desc,
                    Order = 1,
                    Path = cmd
                };
                customisation.Apply(doc);

                var nodes = doc.DescendantNodes()
                    .Where(n => n is XElement e && e.Name.LocalName == "RunSynchronousCommand")
                    .Select(n => (XElement) n);
                Assert.IsNotNull(nodes);
                Assert.IsTrue(nodes.Any());

                foreach (var n in nodes) {
                    var d = n.Descendant("Description");
                    Assert.IsNotNull(d);
                    Assert.AreEqual(desc, d.Value);

                    var o = n.Descendant("Order");
                    Assert.IsNotNull(o);
                    Assert.AreEqual("1", o.Value);

                    var p = n.Descendant("Path");
                    Assert.IsNotNull(p);
                    Assert.AreEqual(cmd, p.Value);
                }
            }

            {
                var file = Path.Combine(this.TestContext.DeploymentDirectory!, "Unattend_PE_x64_damaged.xml");
                Assert.IsTrue(File.Exists(file));

                var doc = XDocument.Load(file, LoadOptions.None);
                Assert.IsNotNull(doc);

                var builder = new UnattendBuilder(Options.Create<UnattendBuilderOptions>(new()));
                var logger = this._loggerFactory.CreateLogger<RunCommandCustomisation>();
                var customisation = new RunCommandCustomisation(builder, logger) {
                    Description = desc,
                    Order = 1,
                    Path = cmd
                };
                customisation.Apply(doc);

                var nodes = doc.DescendantNodes()
                    .Where(n => n is XElement e && e.Name.LocalName == "RunSynchronousCommand")
                    .Select(n => (XElement) n);
                Assert.IsNotNull(nodes);
                Assert.IsTrue(nodes.Any());

                foreach (var n in nodes) {
                    var d = n.Descendant("Description");
                    Assert.IsNotNull(d);
                    Assert.AreEqual(desc, d.Value);

                    var o = n.Descendant("Order");
                    Assert.IsNotNull(o);
                    Assert.AreEqual("1", o.Value);

                    var p = n.Descendant("Path");
                    Assert.IsNotNull(p);
                    Assert.AreEqual(cmd, p.Value);
                }
            }


        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }

}
