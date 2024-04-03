// <copyright file="VDS_VOLUME_PROP2.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a volume object. This structure is identical
    /// to the <see cref="VDS_VOLUME_PROP"/>  structure, except that it also
    /// includes the volume GUIDs.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VDS_VOLUME_PROP2 {
        public Guid ID;
        public VDS_VOLUME_TYPE Type;
        public VDS_VOLUME_STATUS Status;
        public VDS_HEALTH Health;
        public VDS_TRANSITION_STATE TransitionState;
        public ulong Size;
        public VDS_VOLUME_FLAG Flags;
        public VDS_FILE_SYSTEM_TYPE RecommendedFileSystemType;
        public int UniqueIDLength;
        public string Name;
        public IntPtr UniqueID;
    }
}
