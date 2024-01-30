// <copyright file="VDS_INTERCONNECT_ADDRESS_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of the valid address types of a physical interconnect.
    /// </summary>
    public enum VDS_INTERCONNECT_ADDRESS_TYPE : uint {

        /// <summary>
        /// This value is reserved.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// The address type is FCFS.
        /// </summary>
        FCFS = 1,

        /// <summary>
        /// The address type is FCPH.
        /// </summary>
        FCPH = 2,

        /// <summary>
        /// The address type is FCPH3.
        /// </summary>
        FCPH3 = 3,

        /// <summary>
        /// The address type is MAC.
        /// </summary>
        MAC = 4,

        /// <summary>
        /// The address type is SCSI.
        /// </summary>
        SCSI = 5
    }
}
