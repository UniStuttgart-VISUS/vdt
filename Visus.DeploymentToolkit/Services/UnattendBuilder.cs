// <copyright file="UnattendBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Options;
using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Extensions;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the <see cref="IUnattendBuilder"/> interface.
    /// </summary>
    /// <param name="options"></param>
    internal sealed class UnattendBuilder(
            IOptions<UnattendBuilderOptions> options) : IUnattendBuilder {

        /// <inheritdoc />
        public XElement MakeComponent(string name, string? architecture) {
            ArgumentNullException.ThrowIfNull(name);
            architecture ??= RuntimeInformation.ProcessArchitecture.GetFolder();

            return new XElement(this.MakeName("component"),
                new XAttribute("name", name),
                new XAttribute("architecture", architecture),
                new XAttribute("publicKeyToken", this._options.PublicKeyToken),
                new XAttribute("versionScope", this._options.VersionScope),
                new XAttribute("language", this._options.Language));
        }

        /// <inheritdoc />
        public XName MakeName(string localName) => XName.Get(
            localName, this._options.Namespace);

        #region Private fields
        private readonly UnattendBuilderOptions _options = options?.Value
            ?? throw new ArgumentNullException(nameof(options));
        #endregion
    }
}
