// <copyright file="IVdsVolume.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// 
    /// </summary>
    [ComImport]
    [Guid("88306bb2-e71f-478c-86a2-79da200a0f11")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsVolume {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Returns property details of the current volume.
        /// </summary>
        /// <param name="volumeProperties"></param>
        void GetProperties(out VDS_VOLUME_PROP volumeProperties);

        /// <summary>
        /// Retrieves the pack to which the volume is a member.
        /// </summary>
        /// <param name="pack"></param>
        void GetPack(out IVdsPack pack);

        /// <summary>
        /// Returns an object that enumerates the plexes of the volume.
        /// </summary>
        /// <param name="plexes"></param>
        void QueryPlexes(out IEnumVdsObject plexes);

        /// <summary>
        /// Expands the size of the current volume by adding disk extents to
        /// each member of each plex.
        /// </summary>
        /// <param name="inputDiskArray"></param>
        /// <param name="numberOfDisks"></param>
        /// <param name="vdsAsync"></param>
        void Extend(VDS_INPUT_DISK[] inputDiskArray,
            int numberOfDisks,
            out IVdsAsync vdsAsync);

        /// <summary>
        /// Reduces the size of the volume and all plexes, and returns the
        /// released extents to free space.
        /// </summary>
        /// <param name="numberOfBytesToRemove"></param>
        /// <param name="vdsAsync"></param>
        void Shrink(ulong numberOfBytesToRemove, out IVdsAsync vdsAsync);

        /// <summary>
        /// Adds a volume as a plex to the current volume.
        /// </summary>
        /// <param name="volumeID"></param>
        /// <param name="vdsAsync"></param>
        void AddPlex(Guid volumeID, out IVdsAsync vdsAsync);

        /// <summary>
        /// Removes a specified plex from the current volume..
        /// </summary>
        /// <param name="plexID"></param>
        /// <param name="vdsAsync"></param>
        void BreakPlex(Guid plexID, out IVdsAsync vdsAsync);

        /// <summary>
        /// Removes one or more specified plexes from the current volume,
        /// releasing the extents.
        /// </summary>
        /// <param name="plexID"></param>
        /// <param name="vdsAsync"></param>
        void RemovePlex(Guid plexID, out IVdsAsync vdsAsync);

        /// <summary>
        /// Deletes the volume and all plexes, releasing the extents.
        /// </summary>
        /// <param name="force"></param>
        void Delete(bool force);

        /// <summary>
        /// Sets the volume flags.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="revertOnClose"></param>
        void SetFlags(VDS_VOLUME_FLAG flags, bool revertOnClose);

        /// <summary>
        /// Clears the volume flags.
        /// </summary>
        /// <param name="flags"></param>
        void ClearFlags(uint flags);
    }
}
