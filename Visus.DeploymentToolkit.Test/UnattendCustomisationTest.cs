// <copyright file="UnattendCustomisationTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Unattend;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Tests for implementations of <see cref="ICustomisation"/>.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"TestData\Unattend_Core_x64.xml")]
    public sealed class UnattendCustomisationTest {

        public TestContext TestContext { get; set; }

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

            Assert.Throws<InvalidOperationException>(() => {
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

            Assert.Throws<InvalidOperationException>(() => {
                var customisation = new XmlValueCustomisation(this._loggerFactory.CreateLogger<XmlValueCustomisation>()) {
                    Path = "//unattend:component[contains(@name, 'saddam')]/unattend:SetupUILanguage/unattend:UILanguage",
                    Value = "de-DE",
                    IsRequired = true
                };
                customisation.Apply(doc);
            });
        }

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());
    }

}
