// <copyright file="IVdsProvider.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Provides access to the metadata of a provider.
    /// </summary>
    [ComImport]
    [Guid("10c5e575-7984-4e81-a56b-431f5f92ae42")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsProvider {

        /// <summary>
        /// Returns the properties of a provider.
        /// </summary>
        /// <param name="properties"></param>
        void GetProperties(out VDS_PROVIDER_PROP properties);
    }
}
