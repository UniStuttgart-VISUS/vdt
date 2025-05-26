// <copyright file="CustomisationBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Xml;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Extensions;


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
        /// Gets a namespace resolver for the default namespace of the given
        /// XML documment, which is mapped to the prefix &quot;unattend&quot;
        /// and &quot;u&quot;.
        /// </summary>
        /// <param name="unattend"></param>
        /// <returns></returns>
        protected static IXmlNamespaceResolver GetResolver(XDocument unattend)
            => unattend.GetResolver("unattend", "u");
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
