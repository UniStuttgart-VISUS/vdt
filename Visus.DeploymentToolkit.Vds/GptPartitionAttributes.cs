// <copyright file="MbrPartitionParameters.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Possible attributes of a GPT partition as described on
    /// https://learn.microsoft.com/en-us/windows/win32/api/vds/ns-vds-create_partition_parameters
    /// </summary>
    [Flags]
    public enum GptPartitionAttributes : ulong {

        /// <summary>
        /// If this attribute is set, the partition is required by a computer
        /// to function properly.
        /// </summary>
        PlatformRequied = 0x0000000000000001,

        /// <summary>
        /// If this attribute is set, the partition does not receive a drive
        /// letter by default when the disk is moved to another computer or when
        /// the disk is seen for the first time by a computer.
        /// </summary>
        NoDriveLetter = 0x8000000000000000,

        /// <summary>
        /// If this attribute is set, the partition is not detected by the Mount
        /// Manager.
        /// </summary>
        Hidden = 0x4000000000000000,

        /// <summary>
        /// If this attribute is set, the partition is a shadow copy of another
        /// partition.
        /// </summary>
        ShadowCopy = 0x2000000000000000,

        /// <summary>
        /// If this attribute is set, the partition is read-only. All requests
        /// to write to the partition will fail.
        /// </summary>
        ReadOnly = 0x1000000000000000
    }
}
