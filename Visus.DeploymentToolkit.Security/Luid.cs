// <copyright file="Luid.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// Describes a local identifier for an adapter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Luid {

        /// <summary>
        /// Specifies a <c>DWORD</c> that contains the unsigned lower numbers of
        /// the ID.
        /// </summary>
        public uint LowPart;

        /// <summary>
        /// Specifies a <c>DWORD</c> that contains the high lower numbers of
        /// the ID.
        /// </summary>
        public int HighPart;
    }
}
