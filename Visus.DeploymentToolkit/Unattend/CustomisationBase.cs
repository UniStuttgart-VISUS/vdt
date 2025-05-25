// <copyright file="CustomisationBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// The base class for customisation steps, which provides several methods
    /// facilitating the modification of an unattend.xml file.
    /// </summary>
    /// <param name="logger">The logger used by the step to record errors and
    /// progress messages.</param>
    public abstract class CustomisationBase(ILogger logger) : ICustomisation {

        #region Public methods
        /// <inheritdoc />
        public abstract void Apply(XDocument unattend);
        #endregion

        #region Protected methods
        /// <summary>
        /// Creates an <see cref="IXmlNamespaceResolver"/> for the specified
        /// <see cref="XDocument"/>.
        /// </summary>
        /// <param name="document">The XML document for which the namespace
        /// resolver is created, which must have a default namespace</param>
        /// <param name="defaultNamespace">The prefix to associate with the
        /// default namespace of the document.</param>
        /// <returns>An <see cref="IXmlNamespaceResolver"/> that resolves
        /// namespaces using the default namespace of the specified document.
        /// </returns>
        protected static IXmlNamespaceResolver GetResolver(XDocument document,
                string defaultNamespace = "unattend") {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(defaultNamespace);
            var retval = new XmlNamespaceManager(new NameTable());
            retval.AddNamespace(defaultNamespace,
                document.Root!.GetDefaultNamespace().NamespaceName);
            return retval;
        }

        /// <summary>
        /// Gets all elements matching the given XPath expression.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected IEnumerable<XElement> GetElements(XDocument document,
                string path) {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(path);
            this._logger.LogTrace("Selecting elements to modify via XPath "
                + "expression \"{Path}\".", path);
            var resolver = new XmlNamespaceManager(new NameTable());
            resolver.AddNamespace("unattend",
                document.Root!.GetDefaultNamespace().NamespaceName);
            return document.XPathSelectElements(path, GetResolver(document));
        }

        /// <summary>
        /// Gets all elements matching the given XPath expression and throws if
        /// nothing did match.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected IEnumerable<XElement> GetRequiredElements(XDocument element,
                string path) {
            var retval = this.GetElements(element, path);

            if (retval?.Any() != true) {
                this._logger.LogError("The XML element at \"{Path}\" is "
                        + "expected to exist but does not.", path);
                throw new InvalidOperationException(string.Format(
                    Errors.InexistentXPath, path));
            }

            return retval;
        }
        #endregion

        #region Protected fields
        /// <summary>
        /// The logger used by the step to record errors and progress messages.
        /// </summary>
        protected readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        #endregion
    }
}
