// <copyright file="FirmwareType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.SystemInformation {

    /// <summary>
    /// Lists the supported firmware types of a system.
    /// </summary>
    public enum FirmwareType {

        /// <summary>
        /// The system is using a BIOD and MBR disks.
        /// </summary>
        Bios,

        /// <summary>
        /// The system is using a UEFI and GPT disks.
        /// </summary>
        Uefi
    }
}
