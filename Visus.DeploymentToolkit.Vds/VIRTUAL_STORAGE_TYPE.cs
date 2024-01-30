// <copyright file="VIRTUAL_STORAGE_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Contains the type and provider (vendor) of the virtual storage device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VIRTUAL_STORAGE_TYPE {
        /// <summary>
        /// Device type identifier.
        /// </summary>
        uint DeviceId;

        /// <summary>
        /// Vendor-unique identifier.
        /// </summary>
        Guid VendorId;
    }
}
