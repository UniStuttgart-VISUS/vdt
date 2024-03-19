// <copyright file="DismString.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// DISM API functions that return strings wrap the heap allocated PCWSTR
    /// in a DismString structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismString {

        /// <summary>
        /// A null-terminated Unicode string.
        /// </summary>
        string Value;
    }

}
