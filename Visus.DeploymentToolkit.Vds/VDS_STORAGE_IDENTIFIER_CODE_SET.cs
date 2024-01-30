// <copyright file="VDS_STORAGE_IDENTIFIER.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of the valid code sets (encodings) of a storage
    /// identifier.
    /// </summary>
    public enum VDS_STORAGE_IDENTIFIER_CODE_SET {

        /// <summary>
        /// This value is reserved.
        /// </summary>
        Reserved = 0,

        /// <summary>
        /// The storage identifier is encoded as binary data.
        /// </summary>
        Binary = 1,

        /// <summary>
        /// The storage identifier is encoded as ASCII data.
        /// </summary>
        Ascii = 2,

        /// <summary>
        /// The storage identifier is encoded as UTF-8.
        /// </summary>
        Utf8 = 3
    }
}
