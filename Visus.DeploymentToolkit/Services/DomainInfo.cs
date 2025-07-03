// <copyright file="DomainInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides some information about an Active Directory domain that might
    /// be useful for joining a machine.
    /// </summary>
    public sealed class DomainInfo {

        #region Public properties
        /// <summary>
        /// Gets the name of one of the domain controllers.
        /// </summary>
        public string DomainController { get; }

        /// <summary>
        /// Gets the DNS name of the forest.
        /// </summary>
        public string Forest { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> of the domain.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// Gets the name of the domain.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the domain with the preferred domain controller for
        /// use with <see cref="NetApi.NetJoinDomain"/>.
        /// </summary>
        public string NameWithDomainController
            => $@"{this.Name}\{this.DomainController}";

        /// <summary>
        /// Gets the name of the site where the domain controller resides.
        /// </summary>
        public string Site { get; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override string ToString() => this.Name ?? base.ToString()!;
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="info"></param>
        [SupportedOSPlatform("windows")]
        internal DomainInfo(NetApi.DOMAIN_CONTROLLER_INFO info) {
            this.DomainController = info.DomainControllerName.TrimStart('\\');
            this.Forest = info.DnsForestName;
            this.ID = info.DomainGuid;
            this.Name = info.DomainName;
            this.Site = info.DcSiteName;
        }
        #endregion
    }
}
