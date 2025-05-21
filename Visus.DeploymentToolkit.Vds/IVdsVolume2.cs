// <copyright file="IVdsVolume2.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    [ComImport]
    [Guid("72AE6713-DCBB-4a03-B36B-371F6AC6B53D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsVolume2 {

        void GetProperties2(out VDS_VOLUME_PROP2 volumeProperties);
    }
}
