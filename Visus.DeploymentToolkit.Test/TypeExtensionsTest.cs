// <copyright file="TypeExtensionsTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Extensions;


namespace Visus.DeploymentToolkit.Test {


    /// <summary>
    /// Tests for extension methods in <see cref="TypeExtensions"/>.
    /// </summary>
    [TestClass]
    public sealed class TypeExtensionsTest {

        [TestMethod]
        public void IsBasicJson() {
            Assert.IsTrue(typeof(int).IsBasicJson());
            Assert.IsTrue(typeof(float).IsBasicJson());
            Assert.IsTrue(typeof(double).IsBasicJson());
            Assert.IsTrue(typeof(ushort).IsBasicJson());
            Assert.IsTrue(typeof(bool).IsBasicJson());
            Assert.IsTrue(typeof(string).IsBasicJson());
            Assert.IsFalse(typeof(object).IsBasicJson());
            Assert.IsFalse(typeof(Dictionary<string, string>).IsBasicJson());
            Assert.IsFalse(typeof(string[]).IsBasicJson());
        }

        [TestMethod]
        public void IsEnumerable() {
            Assert.IsFalse(typeof(int).IsEnumerable());
            Assert.IsTrue(typeof(IEnumerable<int>).IsEnumerable());
            Assert.IsTrue(typeof(IEnumerable<object>).IsEnumerable());
            Assert.IsTrue(typeof(IEnumerable<int>).IsEnumerable(TypeExtensions.IsBasicJson));
            Assert.IsFalse(typeof(IEnumerable<object>).IsEnumerable(TypeExtensions.IsBasicJson));
        }

        [TestMethod]
        public void IsNumeric() {
            Assert.IsTrue(typeof(int).IsNumeric());
            Assert.IsTrue(typeof(float).IsNumeric());
            Assert.IsTrue(typeof(byte).IsNumeric());
            Assert.IsFalse(typeof(string).IsNumeric());
        }
    }
}
