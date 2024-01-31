// <copyright file="VDS_ASYNC_OUTPUT_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of operations that objects can process.
    /// </summary>
    public enum VDS_ASYNC_OUTPUT_TYPE : uint {
        UNKNOWN = 0,
        CREATEVOLUME = 1,
        EXTENDVOLUME = 2,
        SHRINKVOLUME = 3,
        ADDVOLUMEPLEX = 4,
        BREAKVOLUMEPLEX = 5,
        REMOVEVOLUMEPLEX = 6,
        REPAIRVOLUMEPLEX = 7,
        RECOVERPACK = 8,
        REPLACEDISK = 9,
        CREATEPARTITION = 10,
        CLEAN = 11,
        CREATELUN = 50,
        ADDLUNPLEX = 52,
        REMOVELUNPLEX = 53,
        EXTENDLUN = 54,
        SHRINKLUN = 55,
        RECOVERLUN = 56,
        LOGINTOTARGET = 60,
        LOGOUTFROMTARGET = 61,
        CREATETARGET = 62,
        CREATEPORTALGROUP = 63,
        DELETETARGET = 64,
        ADDPORTAL = 65,
        REMOVEPORTAL = 66,
        DELETEPORTALGROUP = 67,
        FORMAT = 101,
        CREATE_VDISK = 200,
        ATTACH_VDISK = 201,
        COMPACT_VDISK = 202,
        MERGE_VDISK = 203,
        EXPAND_VDISK = 204
    }
}
