// <copyright file="Extensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Security.Principal;


namespace Visus.DeploymentToolkit.Test {

    /// <summary>
    /// Extension methods used for testing.
    /// </summary>
    internal static class Extensions {

        public static bool IsAdministrator(this WindowsIdentity identity) {
            if (identity == null) {
                return false;
            }

            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}
