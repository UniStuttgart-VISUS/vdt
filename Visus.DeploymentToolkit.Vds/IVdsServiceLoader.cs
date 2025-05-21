// <copyright file="IVdsServiceLoader.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Entry point to all VDS objects.
    /// </summary>
    [ComImport]
    [Guid("e0393303-90d4-4a97-ab71-e9b671ee2729")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsServiceLoader {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Launches VDS on the specified computer and returns a pointer to the
        /// service object.
        /// </summary>
        /// <param name="machineName">This parameter must be set to <c>null</c>.
        /// </param>
        /// <param name="service">Receives the <see cref="IVdsService"/>
        /// interface.</param>
        void LoadService(
            [In, MarshalAs(UnmanagedType.LPWStr)] string? machineName,
            out IVdsService service);
    }
}