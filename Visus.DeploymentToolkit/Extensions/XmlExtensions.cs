// <copyright file="XmlExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides XML-related extension methods.
    /// </summary>
    public static class XmlExtensions {

        /// <summary>
        /// Gets the first decendant with the specified name.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement? Descendant(this XElement that, string name) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(name);
            return that.Descendants().FirstOrDefault(
                e => (e.Name.LocalName == name));
        }

        /// <summary>
        /// Creates an <see cref="IXmlNamespaceResolver"/> for the default
        /// namespaces of the specified element.
        /// </summary>
        /// <param name="that">The XML element for which the namespace
        /// resolver is created, which must have a default namespace</param>
        /// <param name="prefixes">The prefix to associate with the
        /// default namespace of the document This parameter defaults to
        /// &quot;unattend&quot;.</param>
        /// <returns>An <see cref="IXmlNamespaceResolver"/> that resolves
        /// namespaces using the default namespace of the specified default
        /// namespace.</returns>
        public static IXmlNamespaceResolver GetResolver(
                this XElement that,
                params string[] prefixes) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(prefixes);
            var namespaceName = that.GetDefaultNamespace().NamespaceName;
            var retval = new XmlNamespaceManager(new NameTable());

            foreach (var p in  prefixes) {
                retval.AddNamespace(p, namespaceName);
            }

            return retval;
        }

        /// <summary>
        /// Gets an XPath expression for the specified <see cref="XElement"/>.
        /// </summary>
        /// <param name="that">The element to get the XPath for.</param>
        /// <param name="resolver">An optional namespace resolver that will
        /// provide the prefixes for the path.</param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(that))]
        public static string? GetXPath(this XElement that,
                IXmlNamespaceResolver? resolver) {
            if (that is null) {
                return null;
            }

            var sb = new StringBuilder(that.Name.LocalName);

            var ns = resolver?.LookupPrefix(that.Name.NamespaceName);
            if (ns is not null) {
                sb.Insert(0, ':');
                sb.Insert(0, ns);
            }

            var parent = that.Parent;
            while (parent is not null) {
                sb.Insert(0, '/');
                sb.Insert(0, parent.Name.LocalName);

                ns = resolver?.LookupPrefix(that.Name.NamespaceName);
                if (ns is not null) {
                    sb.Insert(0, ':');
                    sb.Insert(0, ns);
                }

                parent = parent.Parent;
            }

            sb.Insert(0, '/');

            return sb.ToString();
        }

        /// <summary>
        /// Creates an <see cref="IXmlNamespaceResolver"/> for the specified
        /// <see cref="XDocument"/>.
        /// </summary>
        /// <param name="that">The XML document for which the namespace
        /// resolver is created, which must have a default namespace</param>
        /// <param name="prefixes">The prefix to associate with the
        /// default namespace of the document This parameter defaults to
        /// &quot;unattend&quot;.</param>
        /// <returns>An <see cref="IXmlNamespaceResolver"/> that resolves
        /// namespaces using the default namespace of the specified document.
        /// </returns>
        public static IXmlNamespaceResolver GetResolver(
                this XDocument that,
                params string[] prefixes)
            => that.Root!.GetResolver(prefixes);

        /// <summary>
        /// Selects a collection of <see cref="XElement"/> objects that match
        /// the specified XPath expression and makes sure that the selection
        /// is not empty.
        /// </summary>
        /// <param name="node">The node to evaluate the XPath expression
        /// against.</param>
        /// <param name="expression">The XPath expression used to select
        /// elements. This cannot be <c>null</c> or empty.</param>
        /// <param name="resolver">An <see cref="IXmlNamespaceResolver"/> used
        /// to resolve namespace prefixes in the XPath expression. This can
        /// be <c>null</c> if no namespaces are used.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> containing the
        /// elements that match the specified XPath expression.</returns>
        /// <exception cref="ArgumentException">If no elements matching
        /// the XPath expression are found.</exception>
        public static IEnumerable<XElement> XPathSelectRequiredElements(
                this XNode node,
                string expression,
                IXmlNamespaceResolver? resolver) {
            var retval = node.XPathSelectElements(expression, resolver);

            if (retval?.Any() != true) {
                throw new ArgumentException(string.Format(
                    Errors.InexistentXPath, expression));
            }

            return retval;
        }

        /// <summary>
        /// Selects a single <see cref="XElement" /> matching the given XPath
        /// expression or throws.
        /// </summary>
        /// <param name="node">The node to evaluate the XPath expression
        /// against.</param>
        /// <param name="expression">The XPath expression used to select
        /// the element. This cannot be <c>null</c> or empty.</param>
        /// <param name="resolver">An <see cref="IXmlNamespaceResolver"/> used
        /// to resolve namespace prefixes in the XPath expression. This can
        /// be <c>null</c> if no namespaces are used.</param>
        /// <returns>The <see cref="IEnumerable{XElement}"/> matching the
        /// specified XPath expression.</returns>
        public static XElement XPathSelectSingleElement(
                this XNode node,
                string expression,
                IXmlNamespaceResolver? resolver)
            => node.XPathSelectRequiredElements(expression, resolver).Single();
    }
}
