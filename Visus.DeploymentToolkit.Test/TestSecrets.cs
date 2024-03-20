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

        public string NetworkShare { get; set; }
        public string NetworkPassword { get; set; }
        public string NetworkUser { get; set; }
    }
}
