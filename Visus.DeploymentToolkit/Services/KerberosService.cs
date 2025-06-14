// <copyright file="KerberosService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Kerberos.NET.Crypto;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static Visus.DeploymentToolkit.Services.NetApi;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    ///Implementation of <see cref="IKerberosService"/>.
    /// </summary>
    internal sealed class KerberosService(ILogger<KerberosService> logger) {

        public void CreateKeyTable() {
        }

        /// <inheritdoc />
        public void JoinDomain(string domain,
                string? account,
                string? password,
                JoinOptions options = JoinOptions.JoinDomain,
                string? organisationalUnit = null) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                this._logger.LogTrace("Joining local machine to domain "
                    + "{Domain} in {OU} using the account {Account} and "
                    + "options {Options}. A password {Password}.", domain,
                    account, organisationalUnit, options,
                    (password is null) ? "was not given" : "was given");
                var status = NetJoinDomain(null,
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

        #region Private fields
        private readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        #endregion
    }
}
