// <copyright file="IKerberosService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Kerberos.NET.Crypto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static Visus.DeploymentToolkit.Services.NetApi;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service for managing Kerberos stuff like joinging the domain or
    /// creating key tables.
    /// </summary>
    public interface IKerberosService {

        /// <summary>
        /// Joins the computer to a workgroup or domain.
        /// </summary>
        /// <param name="domain">A pointer to a constant null-terminated
        /// character string that specifies the name of the domain or workgroup
        /// to join.</param>
        /// <param name="account">A pointer to a constant null-terminated
        /// character string that specifies the account name to use when
        /// connecting to the domain controller. The string must specify either
        /// a domain NetBIOS name and user account (for example, REDMOND\user)
        /// or the user principal name (UPN) of the user in the form of an
        /// Internet-style login name (&quot;user@remond.com&quot;). If this
        /// parameter is <see langword="null"/>, the caller's context is used.
        /// </param>
        /// <param name="password">If the <paramref name="account"/>  parameter
        /// specifies an account name, this parameter must point to the
        /// password to use when connecting to the domain controller. Otherwise,
        /// this parameter must be <see langword="null"/>. You can specify a
        /// local machine account password rather than a user password for
        /// unsecured joins.</param>
        /// <param name="options">A set of bit flags defining the join options.
        /// </param>
        /// <param name="organisationalUnit">Optionally specifies the pointer to
        /// a constant null-terminated character string that contains the
        /// RFC-1779 format name of the organisational unit (OU) for the
        /// computer account. If you specify this parameter, the string must
        /// contain a full path lik OU=testOU,DC=domain,DC=Domain,DC=com.
        /// Otherwise, this parameter must be <see langword="null"/>.</param>
        void JoinDomain(string domain, string? account, string password,
            JoinOptions options, string? organisationalUnit = null);
    }
}
