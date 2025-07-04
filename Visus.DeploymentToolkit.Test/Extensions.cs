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

        /// <summary>
        /// Answer whether the specified identity is an administrator.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsAdministrator(this WindowsIdentity that) {
            if (that == null) {
                return false;
            }

            var principal = new WindowsPrincipal(that);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}
