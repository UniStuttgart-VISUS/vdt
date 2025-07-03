// <copyright file="DomainTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Kerberos.NET.Crypto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DirectoryAuthentication;
using Visus.DirectoryAuthentication.Configuration;
using Visus.Ldap.Configuration;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class DomainTest {

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public async Task TestChangeMachinePassword() {
            if (this._secrets.CanTestDomain) {
                var services = this.CreateServiceProvider();

                var krbOptions = Options.Create(new DomainOptions());
                var ldap = services.GetRequiredService<ILdapConnectionService>();
                var ldapOptions = services.GetRequiredService<IOptions<LdapOptions>>();
                var logger = services.GetRequiredService<ILogger<DomainService>>();

                var domain = new DomainService(krbOptions, ldap, ldapOptions, logger);

                await domain.SetMachinePasswordAsync(this._secrets.TestMachine!,
                    "horst",
                    this._secrets.DomainAdmin,
                    this._secrets.DomainPassword!);

                await domain.SetMachinePasswordAsync(this._secrets.TestMachine!,
                    "hugo",
                    null,
                    "horst");
            } else {
                Assert.Inconclusive("This test requires domain information.");
            }
        }

        [TestMethod]
        public async Task TestDiscover() {
            if (!string.IsNullOrWhiteSpace(this._secrets.Domain)) {
                var services = this.CreateDeploymentServiceProvider();

                var krbOptions = Options.Create(new DomainOptions());
                var ldap = services.GetRequiredService<ILdapConnectionService>();
                var ldapOptions = services.GetRequiredService<IOptions<LdapOptions>>();
                var logger = services.GetRequiredService<ILogger<DomainService>>();

                var domain = new DomainService(krbOptions, ldap, ldapOptions, logger);
                var info = await domain.DiscoverAsync(this._secrets.Domain!);
                Assert.IsNotNull(info);
                Assert.IsNotNull(info.Name);
                Assert.IsNotNull(info.DomainController);
            } else {
                Assert.Inconclusive("This test requires domain information.");
            }
        }

        [TestMethod]
        public async Task CreateKeytab() {
            if (this._secrets.CanTestDomain) {
                var services = this.CreateServiceProvider();

                var krbOptions = Options.Create(new DomainOptions());
                var ldap = services.GetRequiredService<ILdapConnectionService>();
                var ldapOptions = services.GetRequiredService<IOptions<LdapOptions>>();
                var logger = services.GetRequiredService<ILogger<DomainService>>();

                var domain = new DomainService(krbOptions, ldap, ldapOptions, logger);

                var keytab = await domain.CreateKeyTableAsync(this._secrets.TestMachine!,
                    "horst",
                    this._secrets.DomainController!,
                    this._secrets.DomainAdmin!,
                    this._secrets.DomainPassword!,
                    IDomainService.DefaultEncryptionTypes);
                Assert.IsNotNull(keytab);

            } else {
                Assert.Inconclusive("This test requires domain information.");
            }
        }

        private IServiceProvider CreateDeploymentServiceProvider() => new ServiceCollection()
            .AddLogging(o => {
                o.AddDebug();
                o.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureLdap()
            .AddDeploymentServices()
            .BuildServiceProvider();

        private IServiceProvider CreateServiceProvider() => new ServiceCollection()
            .AddLogging(o => {
                o.AddDebug();
                o.SetMinimumLevel(LogLevel.Trace);
            })
            .AddLdapAuthentication(o => {
                o.Schema = "Active Directory";
                o.Servers = [ this._secrets.DomainController! ];
                o.IsNoCertificateCheck = true;
                o.IsRecursiveGroupMembership = false;
                o.User = this._secrets.DomainAdmin;
                o.Password = this._secrets.DomainPassword;
                o.SearchBases = new Dictionary<string, SearchScope>() {
                    { this._secrets.SearchBase!, SearchScope.Subtree }
                };
                o.TransportSecurity = TransportSecurity.Ssl;
                o.Port = 636;
            }, validateOnStart: false)
            .BuildServiceProvider();

        private readonly TestSecrets _secrets = TestSecrets.Load();

    }
}
