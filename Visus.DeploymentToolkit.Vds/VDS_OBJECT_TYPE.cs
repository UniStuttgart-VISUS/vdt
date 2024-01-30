// <copyright file="VDS_OBJECT_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid types of a VDS object.
    /// </summary>
    public enum VDS_OBJECT_TYPE : uint {
        UNKNOWN = 0,
        PROVIDER = 1,
        PACK = 10,
        VOLUME = 11,
        VOLUME_PLEX = 12,
        DISK = 13,
        SUB_SYSTEM = 30,
        CONTROLLER = 31,
        DRIVE = 32,
        LUN = 33,
        LUN_PLEX = 34,
        PORT = 35,
        PORTAL = 36,
        TARGET = 37,
        PORTAL_GROUP = 38,
        STORAGE_POOL = 39,
        HBAPORT = 90,
        INIT_ADAPTER = 91,
        INIT_PORTAL = 92,
        ASYNC = 100,
        ENUM = 101,
        VDISK = 200,
        OPEN_VDISK = 201
    }
}
