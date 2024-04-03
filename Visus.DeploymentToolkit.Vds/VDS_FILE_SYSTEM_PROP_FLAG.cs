// <copyright file="VDS_FILE_SYSTEM_PROP_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the details of file-system compression.
    /// </summary>
    [Flags]
    public enum VDS_FILE_SYSTEM_PROP_FLAG : uint {

        /// <summary>
        /// If set, the file system supports file compression.
        /// </summary>
        COMPRESSED = 0x01
    }
}
