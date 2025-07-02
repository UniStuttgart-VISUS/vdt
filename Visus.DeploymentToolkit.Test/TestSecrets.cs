// <copyright file="TestSecrets.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Holds confidential data required for testing.
    /// </summary>
    internal sealed class TestSecrets {

        public static TestSecrets Load() {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<TestSecrets>()
                .Build();
            var retval = new TestSecrets();
            configuration.Bind(retval);
            return retval;
        }

        public bool CanTestDomain
            => !string.IsNullOrWhiteSpace(this.Domain)
            && !string.IsNullOrWhiteSpace(this.DomainAdmin)
            && !string.IsNullOrWhiteSpace(this.DomainPassword)
            && !string.IsNullOrWhiteSpace(this.DomainController)
            && !string.IsNullOrWhiteSpace(this.SearchBase)
            && !string.IsNullOrWhiteSpace(this.TestMachine);

        public bool CanTestNetworkShare
            => !string.IsNullOrWhiteSpace(this.NetworkShare)
            && !string.IsNullOrWhiteSpace(this.NetworkUser)
            && !string.IsNullOrWhiteSpace(this.NetworkPassword);

        public string? Domain { get; set; }
        public string? DomainAdmin { get; set; }
        public string? DomainController { get; set; }
        public string? DomainPassword { get; set; }
        public string? NetworkShare { get; set; }
        public string? NetworkPassword { get; set; }
        public string? NetworkUser { get; set; }
        public string? SearchBase { get; set; }
        public string? TestMachine { get; set; }
    }
}
