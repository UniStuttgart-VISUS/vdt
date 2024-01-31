// <copyright file="VDS_FILE_SYSTEM_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid types for a file system.
    /// </summary>
    public enum VDS_FILE_SYSTEM_TYPE : uint {
        UNKNOWN = 0,
        RAW,
        FAT,
        FAT32,
        NTFS,
        CDFS,
        UDF,
        EXFAT,
        CSVFS,
        REFS
    }
}
