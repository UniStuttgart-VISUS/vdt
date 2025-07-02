// <copyright file="DomainOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Text;
using Visus.DeploymentToolkit.Extensions;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides the ability to configure the behaviour of the
    /// <see cref="IDomainService"/>.
    /// </summary>
    public sealed class DomainOptions {

        #region Public constants
        /// <summary>
        /// The suggested name of the configuration section used for these
        /// options.
        /// </summary>
        public const string SectionName = "Domain";
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the LDAP attribute holding the account
        /// name.
        /// </summary>
        public string AccountAttribute { get; set; } = "sAMAccountName";

        /// <summary>
        /// Gets or sets whether the account search should append the dollar
        /// sign used for machine accounts in Active Directory.
        /// </summary>
        public bool AppendDollarSign { get; set; } = true;

        /// <summary>
        /// Gets or sets the LDAP attribute of the root DSE that holds the name
        /// of the default naming context.
        /// </summary>
        public string DefaultNamingContextAttribute {
            get;
            set;
        } = "defaultNamingContext";

        /// <summary>
        /// Gets or sets the LDAP attribute of the root DSE that holds the DNS
        /// name of the domain controller used by the connection.
        /// </summary>
        public string DomainControllerAttribute {
            get;
            set;
        } = "dnsHostName";

        /// <summary>
        /// Gets or set the name of the LDAP attribute that holds the Kerberos
        /// key version number (KVNO).
        /// </summary>
        public string KvnoAttribute { get; set; } = "msDS-KeyVersionNumber";

        /// <summary>
        /// Gets or sets the LDAP filter used to find a machine account.
        /// </summary>
        public string MachineFilter {
            get;
            set;
        } = "(&(objectClass=computer)(sAMAccountName={0}))";

        /// <summary>
        /// Gets or sets the name of the LDAP attribute where the password is
        /// being stored.
        /// </summary>
        public string PasswordAttribute { get; set; } = "unicodePwd";

        /// <summary>
        /// Gets or sets the LDAP attribute where the SPNs are stored in Active
        /// Directory.
        /// </summary>
        public string ServicePrincipalNameAttribute {
            get;
            set;
        } = "servicePrincipalName";
        #endregion

        #region Internal methods
        /// <summary>
        /// Encodes the given password for an LDAP request.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        internal byte[] EncodePassword(string password) {
            ArgumentNullException.ThrowIfNull(password);
            if ("unicodePwd".EqualsIgnoreCase(this.PasswordAttribute)) {
                // Cf. https://learn.microsoft.com/en-us/troubleshoot/windows-server/active-directory/change-windows-active-directory-user-password
                return Encoding.Unicode.GetBytes($@"""{password}""");
            } else {
                return Encoding.UTF8.GetBytes(password);
            }
        }
        #endregion
    }
}
