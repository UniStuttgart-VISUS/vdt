// <copyright file="IVdsAsync.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Manages asynchronous operations. Methods that initiate asynchronous
    /// operations return a pointer to an <see cref="IVdsAsync"/> interface,
    /// allowing the caller to optionally cancel, wait for, or query the
    /// status of the asynchronous operation.
    /// </summary>
    [ComImport]
    [Guid("d5d23b6d-5a55-4492-9889-397a3c2d2dbc")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsAsync {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Cancels the asynchronous operation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Returns when the asynchronous operation has either finished
        /// successfully or failed.
        /// </summary>
        /// <param name="hr"></param>
        /// <param name="async"></param>
        void Wait(out int hr, out VDS_ASYNC_OUTPUT async);

        /// <summary>
        /// Returns when the asynchronous operation is in progress, or has either
        /// finished successfully or failed.
        /// </summary>
        /// <param name="hr"></param>
        /// <param name="percentCompleted"></param>
        void QueryStatus(out int hr, out uint percentCompleted);
    }
}
