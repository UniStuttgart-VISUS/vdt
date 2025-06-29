﻿// <copyright file="IEnumVdsObject.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines methods for creating and managing virtual disks.
    /// </summary>
    [ComImport]
    [Guid("b481498c-8354-45f9-84a0-0bdd2832a91f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SupportedOSPlatform("windows")]
    public interface IVdsVdProvider {

        /// <summary>
        /// Returns a list of all virtual disks that are managed by the provider.
        /// </summary>
        /// <param name="enumerator">Receives the enumerator for the virtual
        /// disks.</param>
        void QueryVDisks(out IEnumVdsObject enumerator);

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void CreateVDisk();

        [Obsolete("This method is not correctly mapped, because we did not yet "
            + "need it. However, the method still needs to be there fore the "
            + "interface to work. Callers must fix the sigature before using "
            + "this method or the call will most likely crash the application.")]
        void AddVDisk();

        void GetDiskFromVDisk(IVdsVDisk pVDisk, out IVdsDisk ppDisk);

        void GetVDiskFromDisk(IVdsDisk pDisk, out IVdsVDisk ppVDisk);
    }
}
