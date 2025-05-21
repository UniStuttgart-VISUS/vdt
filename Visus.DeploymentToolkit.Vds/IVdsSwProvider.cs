// <copyright file="IVdsSwProvider.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Provides methods to perform operations specific to the software
    /// provider.
    /// </summary>
    [ComImport]
    [Guid("9aa58360-ce33-4f92-b658-ed24b14425b8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsSwProvider {

        /// <summary>
        /// Returns an enumeration object that contains all packs managed by
        /// the software provider.
        /// </summary>
        /// <param name="packs"></param>
        void QueryPacks(out IEnumVdsObject packs);

        /// <summary>
        /// Creates a pack object.
        /// </summary>
        /// <param name="pack"></param>
        void CreatePack(out IVdsPack pack);
    }
}
