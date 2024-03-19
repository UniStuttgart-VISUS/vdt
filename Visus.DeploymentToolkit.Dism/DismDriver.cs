// <copyright file="DismCapabilityInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes the architecture and hardware that the driver supports.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismDriver {

        /// <summary>
        /// The manufacturer name of the driver.
        /// </summary>
        public string ManufacturerName;

        /// <summary>
        /// A hardware description of the driver.
        /// </summary>
        public string HardwareDescription;

        /// <summary>
        /// The hardware ID of the driver.
        /// </summary>
        public string HardwareID;

        /// <summary>
        /// The architecture of the driver.
        /// </summary>
        public uint Architecture;

        /// <summary>
        /// The service name of the driver.
        /// </summary>
        public string ServiceName;

        /// <summary>
        /// The compatible IDs of the driver.
        /// </summary>
        public string CompatibleIDs;

        /// <summary>
        /// The exclude IDs of the driver.
        /// </summary>
        public string ExcludeIDs;
    }
}
