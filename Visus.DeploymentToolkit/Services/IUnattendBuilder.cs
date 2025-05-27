// <copyright file="IUnattendBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Xml.Linq;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a builder class that allows for creating components
    /// for the unattend.xml file.
    /// </summary>
    public interface IUnattendBuilder {

        /// <summary>
        /// Makes a new XML element for the component with the specified
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The local name of the component.</param>
        /// <param name="architecture">The architecture to use or <c>null</c>
        /// for the architecture of the calling process.</param>
        /// <returns></returns>
        XElement MakeComponent(string name, string? architecture);

        /// <summary>
        /// Makes an XML name in the namespace of unattend.xml.
        /// </summary>
        /// <param name="localName">The local name of the element.</param>
        /// <returns>The qualified name of the element.</returns>
        XName MakeName(string localName);
    }
}
