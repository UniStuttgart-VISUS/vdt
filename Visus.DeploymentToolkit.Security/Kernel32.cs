// <copyright file="Kernel32.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// Holds functions imported from advapi32.dll.
    /// </summary>
    public static class Kernel32 {

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(nint handle);

    }
}
