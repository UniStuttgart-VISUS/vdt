// <copyright file="IDomainService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Kerberos.NET.Crypto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Compliance;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service for managing Kerberos stuff like joinging the domain or
    /// creating key tables.
    /// </summary>
    public interface IDomainService {

        /// <summary>
        /// The default encryption types used when creating a keytab.
        /// </summary>
        public static readonly EncryptionType[] DefaultEncryptionTypes = [
            EncryptionType.AES256_CTS_HMAC_SHA1_96,
            EncryptionType.AES128_CTS_HMAC_SHA1_96,
        ];

        /// <summary>
        /// Creates a Kerberos key table at <paramref name="path"/> for the
        /// given <paramref name="machine"/> and
        /// <paramref name="machinePassword"/>.
        /// </summary>
        /// <param name="machine">The account name or distinguished name of the
        /// machine.</param>
        /// <param name="machinePassword">The machine password to be embedded
        /// in the keytab.</param>
        /// <param name="domainController">The name of the domain controller
        /// used to obtain the service principal names of
        /// <paramref name="machine"/> from.</param>
        /// <param name="user">The name of a user who can connect to the
        /// <paramref name="domainController"/>.</param>
        /// <param name="password">The password of the
        /// <paramref name="user"/>.</param>
        /// <param name="encryptionTypes">The encryption types used for the keys
        /// in the keytab.</param>
        /// <returns>A task for the keytab that is being generated.</returns>
        /// <exception cref="System.ArgumentException">If
        /// <paramref name="path"/> is <see langword="null"/> or empty.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="machine"/> is <see langword="null"/>, or if
        /// <paramref name="machinePassword"/> is <see langword="null"/>.
        /// </exception>
        Task<KeyTable> CreateKeyTableAsync(string machine,
            string machinePassword,
            string domainController,
            string? user,
            string? password,
            IEnumerable<EncryptionType> encryptionTypes);

        /// <summary>
        /// Tries to find a domain for the given DNS or NetBIOS name.
        /// </summary>
        /// <param name="domain">The FQDN or NetBIOS name of the domain to
        /// search.</param>
        /// <returns>An information object for the domain.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="domain"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.InvalidOperationException">If this method
        /// is called on any platform but Windows.</exception>
        Task<DomainInfo> DiscoverAsync(string domain);

        /// <summary>
        /// Joins the computer to a workgroup or domain.
        /// </summary>
        /// <param name="server">The DNS or NetBIOS name of the computer on
        /// which to execute the domain join operation. If this parameter is
        /// <see langword="null"/>, the local computer is used. Note that this
        /// parameter can also be used to rename a machine as part of a join
        /// if <see cref="JoinOptions.JoinWithNewName"/> is set.</param>
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
        void JoinDomain(string? server,
            string domain,
            string? account,
            [SensitiveData] string? password,
            JoinOptions options,
            string? organisationalUnit = null);

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
        void JoinDomain(string domain,
                string? account,
                [SensitiveData] string? password,
                JoinOptions options,
                string? organisationalUnit = null)
            => this.JoinDomain(null, domain, account, password, options,
                organisationalUnit);

        /// <summary>
        /// Sets a new machine password for the given
        /// <paramref name="machine"/>.
        /// </summary>
        /// <remarks>
        /// <para>This method uses LDAP to change the password of the given
        /// machine account. The password can either be reset by providing a
        /// user that has sufficient privileges to change the password of
        /// other accounts, or changed by providing the current machine
        /// password instead.</para>
        /// <para>Please note that using this method will break the trust
        /// between a machine and the domain controller. Therefore, it should
        /// only be used to configure pre-staged accounts that are subsequently
        /// joined with <see cref="JoinDomain"/>.</para>
        /// </remarks>
        /// <param name="machine">The name of the machine, which can either be
        /// the machine account or the distinguished name of the directory
        /// entry.</param>
        /// <param name="machinePassword">The new machine password.</param>
        /// <param name="user">The name of a user account to perform the change.
        /// This can be <see langword="null"/> iff the current machine password
        /// is provided as <paramref name="userPassword"/>. If a user is
        /// provided, the account must have sufficient privileges to change
        /// other account's passwords. This is typically a domain administrator.
        /// </param>
        /// <param name="userPassword">The password of the
        /// <paramref name="user"/>, or the current machine password if the
        /// <paramref name="user"/> is <see langword="null"/>.</param>
        /// <returns></returns>
        Task SetMachinePasswordAsync(
            string machine, [SensitiveData] string machinePassword,
            string? user, [SensitiveData] string userPassword);
    }
}
