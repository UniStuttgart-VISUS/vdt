// <copyright file="VDS_PACK_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a pack object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_PACK_PROP {

        /// <summary>
        /// The GUID of the pack object.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// A string representing the pack name. Packs managed by the basic
        /// provider have no name.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Name;

        /// <summary>
        /// The pack status enumerated by <see cref="VDS_PACK_STATUS"/>.
        /// </summary>
        public VDS_PACK_STATUS Status;

        /// <summary>
        /// The pack flags enumerated by <see cref="VDS_PACK_FLAG"/>.
        /// </summary>
        public VDS_PACK_FLAG Flags;
    }
}
