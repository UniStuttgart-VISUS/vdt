// <copyright file="TokenPrivileges.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// Contains information about a set of privileges for an access token.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TokenPrivileges {

        /// <summary>
        /// This must be set to the number of entries in the Privileges array.
        /// </summary>
        /// <remarks>
        /// This must be one! I don't know right now how we could marshal
        /// anything else here.
        /// </remarks>
        public int PrivilegeCount;

        /// <summary>
        /// An array of <see cref="PrivilegeCount"/> elements of type
        /// <see cref="LuidAndAttributes"/>.
        /// </summary>
        public LuidAndAttributes Privileges;
    }
}
