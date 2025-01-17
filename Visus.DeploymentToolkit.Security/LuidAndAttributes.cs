// <copyright file="LuidAndAttributes.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// Represents a locally unique identifier (<see cref="Luid"/>) and its
    /// attributes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LuidAndAttributes {

        /// <summary>
        /// Specifies an <see cref="Luid"/> value.
        /// </summary>
        public Luid Luid;

        /// <summary>
        /// Specifies attributes of the <see cref="Luid"/>. This value contains
        /// up to 32 one-bit flags. Its meaning is dependent on the definition
        /// and use of the <see cref="Luid"/>.
        /// </summary>
        public uint Attributes;
    }
}
