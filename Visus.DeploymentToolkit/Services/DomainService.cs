// <copyright file="DomainService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Kerberos.NET.Crypto;
using Kerberos.NET.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Compliance;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DirectoryAuthentication;
using Visus.DirectoryAuthentication.Configuration;
using Visus.DirectoryAuthentication.Extensions;
using Visus.Ldap.Configuration;
using static Visus.DeploymentToolkit.Services.NetApi;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    ///Implementation of <see cref="IDomainService"/>.
    /// </summary>
    internal sealed class DomainService(
            IOptions<DomainOptions> krbOptions,
            ILdapConnectionService ldap,
            IOptions<LdapOptions> ldapOptions,
            ILogger<DomainService> logger)
            : IDomainService {

        #region Public methods
        /// <inheritdoc />
        public Task<DomainInfo> DiscoverAsync(string domain) {
            ArgumentNullException.ThrowIfNull(domain);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var info = DsGetDcName(null,
                    domain,
                    null,
                    null,
                    GetDcNameFlags.WritableRequired
                    | GetDcNameFlags.ReturnDnsName);
                return Task.FromResult(new DomainInfo(info));
            } else {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public async Task<KeyTable> CreateKeyTableAsync(string machine,
                string machinePassword,
                string domainController,
                string? user,
                string? password,
                IEnumerable<EncryptionType> encryptionTypes) {
            ArgumentNullException.ThrowIfNull(machine);
            ArgumentNullException.ThrowIfNull(domainController);

            this._logger.LogTrace("Connecting to LDAP server {Server} to "
                + "obtain the service principal names for {Machine}.",
                domainController, machine);
            using var connection = await this.ConnectAsync(domainController,
                user, password);

            var account = await this.GetMachine(connection, machine, connection);
            this._logger.LogTrace("Found machine account {DistinguishedName}.",
                account.DistinguishedName);
            machine = account
                .Attributes[this._krbOptions.AccountAttribute]
                .GetValues<string>()
                .First();
            this._logger.LogTrace("Account name is {Machine}.", machine);

            var spnAtt = this._krbOptions.ServicePrincipalNameAttribute;
            var principals = account.Attributes[spnAtt]?.GetValues<string>()
                ?? Enumerable.Empty<string>();
            this._logger.LogTrace("Found {Count} service principal names "
                + "for machine {Machine}: {Spns}.", principals.Count(),
                machine, string.Join(", ", principals));

            var kvnoAtt = this._krbOptions.KvnoAttribute;
            var kvnoValue = account.Attributes[kvnoAtt]
                ?.GetValues<string>()
                .FirstOrDefault()
                ?? throw new ArgumentException(string.Format(
                    Errors.KvnoNotFound, machine, kvnoAtt));
            var kvno = int.Parse(kvnoValue, CultureInfo.InvariantCulture);
            this._logger.LogTrace("Found key version number {Kvno} for machine "
                + "{Machine}.", kvno, machine);

            var realm = GetDomain(account).ToUpperInvariant();
            this._logger.LogTrace("Using Kerberos realm {Realm}.", realm);

            var spns = principals
                .Select(s => PrincipalName.FromKrbPrincipalName(
                    KrbPrincipalName.FromString(s), realm))
                .Append(PrincipalName.FromKrbPrincipalName(
                    KrbPrincipalName.FromString(machine,
                    PrincipalNameType.NT_PRINCIPAL), realm));

            var keys = new KerberosKey[spns.Count() * encryptionTypes.Count()];
            this._logger.LogTrace("Creating {Count} Kerberos keys for "
                + "{Machine} .", keys.Length, machine);
            {
                int i = 0;
                foreach (var p in spns) {
                    foreach (var e in encryptionTypes) {
                        keys[i++] = new(machinePassword,
                            p,
                            etype: e,
                            kvno: kvno);
                    }
                }
            }

            return new KeyTable(keys);
        }

        /// <inheritdoc />
        public void JoinDomain(string? server,
                string domain,
                string? account,
                [SensitiveData] string? password,
                JoinOptions options = JoinOptions.JoinDomain,
                string? organisationalUnit = null) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                this._logger.LogTrace("Joining {Machine} to domain "
                    + "{Domain} in {OU} using the account {Account} and "
                    + "options {Options}. A password {Password}.",
                    server ?? "the local machine",
                    domain,
                    account,
                    organisationalUnit,
                    options,
                    (password is null) ? "was not given" : "was given");
                var status = NetJoinDomain(server,
                    domain,
                    organisationalUnit,
                    account,
                    password,
                    options);
                if (status != 0) {
                    this._logger.LogTrace("NetJoinDomain failed with status "
                        + "{Status}.", status);
                    throw new Win32Exception(status);
                }

            } else {
                throw new NotImplementedException("This method is only "
                    + "implemented for Windows.");
            }
        }

        /// <inheritdoc />
        public async Task SetMachinePasswordAsync(
                string machine,
                [SensitiveData] string machinePassword,
                string? user,
                [SensitiveData] string userPassword) {
            ArgumentNullException.ThrowIfNull(machine);
            ArgumentNullException.ThrowIfNull(machinePassword);
            ArgumentNullException.ThrowIfNull(userPassword);

            var (domain, machineAccount) = machine.SplitAccount();
            if ((domain is null) || (machineAccount is null)) {
                throw new ArgumentException(
                    string.Format(Errors.InvalidMachineAccount, machine),
                    nameof(machine));
            }
            machine = machineAccount;

            var domainInfo = await this.DiscoverAsync(domain);
            this._logger.LogTrace("Discovered domain controller {Server} for "
                + "domain {Domain}.", domainInfo.DomainController,
                domainInfo.Name);

            this._logger.LogTrace("Connecting to LDAP server {Server} as "
                + "{User} to change the machine password of {Machine} in "
                + "{Domain}.", domainInfo.DomainController,
                user ?? "current user", machine, domain);
            using var connection = await this.ConnectAsync(
                domainInfo.DomainController,
                user,
                userPassword);

            var account = await this.GetMachine(connection, machine, connection);
            this._logger.LogTrace("Found machine account {DistinguishedName}.",
                account.DistinguishedName);

            var req = new ModifyRequest(account.DistinguishedName);

            if (string.IsNullOrWhiteSpace(user)) {
                this._logger.LogTrace("Changing password of machine account "
                    + "using the previous one.");
                // Cf. https://learn.microsoft.com/en-us/troubleshoot/windows-server/active-directory/change-windows-active-directory-user-password
                var add = new DirectoryAttributeModification {
                    Operation = DirectoryAttributeOperation.Add,
                    Name = this._krbOptions.PasswordAttribute,
                };
                add.Add(this._krbOptions.EncodePassword(machinePassword));

                var del = new DirectoryAttributeModification {
                    Operation = DirectoryAttributeOperation.Delete,
                    Name = this._krbOptions.PasswordAttribute,
                };
                del.Add(this._krbOptions.EncodePassword(userPassword));

                req.Modifications.Add(add);
                req.Modifications.Add(del);

            } else {
                this._logger.LogTrace("Setting a new machine password using "
                    + "administrative permissions.");
                // Cf. https://learn.microsoft.com/en-us/troubleshoot/windows-server/active-directory/change-windows-active-directory-user-password
                var mod = new DirectoryAttributeModification {
                    Operation = DirectoryAttributeOperation.Replace,
                    Name = this._krbOptions.PasswordAttribute,
                };
                mod.Add(this._krbOptions.EncodePassword(machinePassword));
                req.Modifications.Add(mod);
            }

            await connection.LdapConnection.SendRequestAsync(req);
        }
        #endregion

        #region Nested class Connection
        /// <summary>
        /// A disposable wrapper around an LDAP connection and its options.
        /// </summary>
        private sealed class Connection : IDisposable {

            /// <summary>
            /// Implicit conversion to <see cref="LdapConnection"/>.
            /// </summary>
            /// <param name="c"></param>
            public static implicit operator LdapConnection(Connection c) {
                return c.LdapConnection;
            }

            /// <summary>
            /// Implicit conversion to <see cref="LdapOptions"/>.
            /// </summary>
            /// <param name="c"></param>
            public static implicit operator LdapOptions(Connection c) {
                return c.LdapOptions;
            }

            /// <summary>
            /// Initialises a new instance.
            /// </summary>
            /// <param name="connection"></param>
            /// <param name="options"></param>
            /// <param name="rootDse"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public Connection(LdapConnection connection,
                    LdapOptions options,
                    SearchResultEntry rootDse) {
                this.LdapConnection = connection
                    ?? throw new ArgumentNullException(nameof(connection));
                this.LdapOptions = options
                    ?? throw new ArgumentNullException(nameof(options));
                this.RootDse = rootDse
                    ?? throw new ArgumentNullException(nameof(rootDse));
            }

            /// <inheritdoc />
            public void Dispose() => this.LdapConnection.Dispose();

            /// <summary>
            /// Gets the LDAP connection that has been established.
            /// </summary>
            public LdapConnection LdapConnection { get; }

            /// <summary>
            /// Gets the options that have been used 
            /// </summary>
            public LdapOptions LdapOptions { get; }

            /// <summary>
            /// Gets the Root DSE entry that has been obtained when connecting
            /// to the LDAP server.
            /// </summary>
            public SearchResultEntry RootDse { get; }
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the attributes to be loaded when searching machines.
        /// </summary>
        private string[] Attributes => [
            this._krbOptions.AccountAttribute,
            this._krbOptions.KvnoAttribute,
            this._krbOptions.ServicePrincipalNameAttribute
        ];
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the domain name of the DN of the given
        /// <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string GetDomain(SearchResultEntry entry,
                StringComparison comparison
                = StringComparison.OrdinalIgnoreCase) {
            Debug.Assert(entry is not null);
            var dn = entry.DistinguishedName.AsSpan();
            var sb = new StringBuilder();

            for (var i = 0; i < dn.Length; ) {
                var end = dn.Slice(i).IndexOf(',');
                if (end < 0) {
                    end = dn.Length - i;
                }

                var part = dn.Slice(i, end);
                if (part.StartsWith("DC=", comparison)) {
                    if (sb.Length > 0) {
                        sb.Append('.');
                    }

                    sb.Append(part.Slice(3).Trim());
                }

                i += end + 1;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Connects to the specified LDAP <paramref name="server"/> using the
        /// given credentials or as the current user if no credentials are
        /// provided.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<Connection> ConnectAsync(
                string server,
                string? user,
                string? password) {
            Debug.Assert(!string.IsNullOrWhiteSpace(server));

            switch (this._ldapOptions.TransportSecurity) {
                case TransportSecurity.Ssl:
                case TransportSecurity.StartTls:
                    break;

                case TransportSecurity.None:
                default:
                    throw new ArgumentException(Errors.NoTransportSecurity);
            }

            // Create a clone of the LDAP options, because we want to modify
            // some stuff for use in this service.
            var options = new LdapOptions();
            foreach (var p in options.GetType().GetProperties()) {
                if (p.SetMethod is not null) {
                    var value = p.GetValue(this._ldapOptions);
                    p.SetValue(options, value);
                }
            }

            // This is unnecessary as we do not work with groups.
            options.IsRecursiveGroupMembership = false;

            // If we have a specific user, use this one instead of the
            // configured one.
            if (user is not null) {
                options.User = user;
                options.Password = password;
            }

            this._logger.LogTrace("Using caller-provided LDAP server {Server}.",
                server);
            options.Servers = [ server ];

            this._logger.LogTrace("Connecting to LDAP server as {User}.",
                options.User ?? "current user");
            var connection = this._ldap.Connect(options);

            this._logger.LogTrace("Obtaining root DSE from LDAP server.");
            var rootDse = await connection.GetRootDseAsync([
                    this._krbOptions.DefaultNamingContextAttribute,
                    this._krbOptions.DomainControllerAttribute
                ]) ?? throw new ArgumentException(Errors.NoRootDse);

            // If we connected successfully, remember the DC we used.
            this._logger.LogTrace("Forcing the LDAP server to be always "
                + "{Server}.", server);
                this._ldapOptions.Servers = [ server ];

            // We always use the default naming context to search for computers.
            var b = rootDse
                .Attributes[this._krbOptions.DefaultNamingContextAttribute]
                .GetValues<string>()
                .FirstOrDefault()
                ?? throw new ArgumentException(Errors.NoDefaultNamingContext);
            this._logger.LogTrace("Setting search base to default naming "
                + "context {SearchBase}.", b);
            this._ldapOptions.SearchBases = new Dictionary<string, SearchScope> {
                { b, SearchScope.Subtree }
            };

            return new(connection, options, rootDse);
        }

        /// <summary>
        /// Searches a machine by its account name.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="account"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task<SearchResultEntry?> GetByAccount(
                LdapConnection connection,
                string account,
                LdapOptions options) {
            Debug.Assert(connection is not null);
            Debug.Assert(account is not null);
            Debug.Assert(options is not null);

            {
                var idx = account.IndexOf('.');
                if (idx >= 0) {
                    this._logger.LogTrace("Removing domain suffix from machine "
                        + "account {Account}.", account);
                    account = account.Substring(0, idx);
                }
            }

            if (this._krbOptions.AppendDollarSign && !account.EndsWith('$')) {
                account += '$';
            }

            this._logger.LogTrace("Searching for machine account {Account}.",
                account);
            return (await connection.SearchAsync(
                string.Format(this._krbOptions.MachineFilter, account),
                this.Attributes,
                options)).SingleOrDefault();
        }

        /// <summary>
        /// Searches a machine account by its distinguished name (DN).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dn"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<SearchResultEntry?> GetByDn(
                LdapConnection connection, string dn, LdapOptions options) {
            Debug.Assert(connection is not null);
            Debug.Assert(options is not null);
            var att = options.Mapping!.DistinguishedNameAttribute;
            this._logger.LogTrace("Searching for {DistinguishedName}.", dn);
            return (await connection.SearchAsync($"({att}={dn})",
                this.Attributes,
                options)).SingleOrDefault();
        }

        /// <summary>
        /// Gets a machine by its account name or distinguished name.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<SearchResultEntry> GetMachine(
                LdapConnection connection,
                string name,
                LdapOptions options)
            => await this.GetByAccount(connection, name, options)
                ?? await this.GetByDn(connection, name, options)
                ?? throw new ArgumentException(string.Format(
                    Errors.MachineAccountNotFound, name), nameof(name));
        #endregion

        #region Private fields
        private readonly DomainOptions _krbOptions = krbOptions?.Value
            ?? throw new ArgumentNullException(nameof(krbOptions));
        private readonly ILdapConnectionService _ldap = ldap
            ?? throw new ArgumentNullException(nameof(_ldap));
        private readonly LdapOptions _ldapOptions = ldapOptions?.Value
            ?? throw new ArgumentNullException(nameof(ldapOptions));
        private readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        #endregion
    }
}
