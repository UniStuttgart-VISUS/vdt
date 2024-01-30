// <copyright file="VDS_INTERCONNECT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the address data of a physical interconnect.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_INTERCONNECT {

        /// <summary>
        /// The interconnect address type enumerated by 
        /// <see cref="VDS_INTERCONNECT_ADDRESS_TYPE"/>.
        /// </summary>
        VDS_INTERCONNECT_ADDRESS_TYPE AddressType;

        /// <summary>
        /// The size of the interconnect address data for the LUN port
        /// (<see cref="Port"/>), in bytes.
        /// </summary>
        uint SizeOfPort;

        /// <summary>
        /// The interconnect address data for the LUN port.
        /// </summary>
        byte[] Port; // TODO

        /// <summary>
        /// The size of the interconnect address data for the LUN
        /// (<see cref="Address"/>), in bytes.
        /// </summary>
        uint SizeOfAddress;

        /// <summary>
        /// The interconnect address data for the LUN.
        /// </summary>
        byte[] Address; // TODO
    }
}
