// <copyright file="VDS_STORAGE_DEVICE_ID_DESCRIPTOR.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines one or more storage identifiers for a storage device
    /// (typically an instance, as opposed to a class, of device).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_STORAGE_DEVICE_ID_DESCRIPTOR {

        /// <summary>
        /// The version of this structure.
        /// </summary>
        uint Version;

        /// <summary>
        /// The number of identifiers specified in <see cref="Identifiers"/>.
        /// </summary>
        uint NumberOfIdentifiers;

        /// <summary>
        /// The <see cref="VDS_STORAGE_IDENTIFIER"/>s.
        /// </summary>
        VDS_STORAGE_IDENTIFIER[] Identifiers;   // TODO
    }
}
