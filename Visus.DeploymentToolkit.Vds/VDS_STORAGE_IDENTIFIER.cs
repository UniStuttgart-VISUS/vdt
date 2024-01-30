// <copyright file="VDS_STORAGE_IDENTIFIER.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines a storage device using a particular code set and type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_STORAGE_IDENTIFIER {

        /// <summary>
        /// The encoding type of <see cref="Identifier"/> enumerated by 
        /// <see cref="VDS_STORAGE_IDENTIFIER_CODE_SET"/>.
        /// </summary>
        public VDS_STORAGE_IDENTIFIER_CODE_SET CodeSet;

        /// <summary>
        /// The type of <see cref="Identifier"/> enumerated by 
        /// <see cref="VDS_STORAGE_IDENTIFIER_TYPE"/>.
        /// </summary>
        public VDS_STORAGE_IDENTIFIER_TYPE Type;

        /// <summary>
        /// The size of the <see cref="Identifier"/> array, in bytes.
        /// </summary>
        public uint SizeOfIdentifier;

        /// <summary>
        /// The identifier data.
        /// </summary>
        byte[] Identifier;// TODO
    }
}
