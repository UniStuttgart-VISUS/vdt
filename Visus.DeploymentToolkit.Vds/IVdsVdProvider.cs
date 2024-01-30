// <copyright file="IEnumVdsObject.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines methods for creating and managing virtual disks.
    /// </summary>
    [ComImport]
    [Guid("b481498c-8354-45f9-84a0-0bdd2832a91f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsVdProvider {
        void QueryVDisks(out IEnumVdsObject ppEnum);
        [Obsolete("This is not correctly mapped")]
        void CreateVDisk();
        [Obsolete("This is not correctly mapped")]
        void AddVDisk();
        void GetDiskFromVDisk(IVdsVDisk pVDisk, out IVdsDisk ppDisk);
        void GetVDiskFromDisk(IVdsDisk pDisk, out IVdsVDisk ppVDisk);
    }
}
