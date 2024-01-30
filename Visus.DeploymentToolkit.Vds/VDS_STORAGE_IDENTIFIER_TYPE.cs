// <copyright file="VDS_STORAGE_IDENTIFIER_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid types for a storage identifier.
    /// </summary>
    public enum VDS_STORAGE_IDENTIFIER_TYPE {

        /// <summary>
        /// The storage identifier type is vendor specific.
        /// </summary>
        VendorSpecific = 0,

        /// <summary>
        /// The storage identifier is the same as the vendor identifier.
        /// </summary>
        VendorId = 1,

        /// <summary>
        /// The storage identifier type follows the IEEE 64-bit Extended
        /// Unique Identifier (EUI-64) standard.
        /// </summary>
        EUI64 = 2,

        /// <summary>
        /// The storage identifier type follows the Fibre Channel Physical and
        /// Signalling Interface (FC-PH) naming.
        /// </summary>
        FCPHName = 3,

        /// <summary>
        /// The storage identifier type is dependent on the port.
        /// </summary>
        PortRelative = 4,

        TargetPortGroup = 5,

        LogicalUnitGroup = 6,

        MD5LogicalUnitIdentifier = 7,

        ScsiNameString = 8
    }
}
