// <copyright file="VDS_PROVIDER_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid flags for a provider object.
    /// </summary>
    [Flags]
    public enum VDS_PROVIDER_FLAG : uint {

        /// <summary>
        /// The provider is a dynamic provider. If this flag is set
        /// for the provider of a disk, the disk is dynamic.
        /// </summary>
        Dynamic = 0x1,

        /// <summary>
        /// The operating system supplies this hardware provider to
        /// manage an internal hardware controller.
        /// </summary>
        InternalHardwareProvider = 0x2,

        /// <summary>
        /// The provider supports single-disk packs only. Typically,
        /// the basic provider sets this flag to simulate a pack with
        /// one disk.
        /// </summary>
        OneDiskOnlyPerPack = 0x4,

        /// <summary>
        /// The provider is a dynamic provider that supports online status for
        /// only one pack at a time.
        /// </summary>
        OnePackOnlineOnly = 0x8,

        /// <summary>
        /// All volumes managed by this provider must have contiguous space.
        /// This flag applies to basic providers only.
        /// </summary>
        VolumeSpaceMustBeContiguous = 0x10,

        SupportDynamic = 0x80000000,

        SupportFaultTolerant = 0x40000000,

        SupportDynamic1394 = 0x20000000,

        SupportMirror = 0x20,

        SupportRaid5 = 0x40,
    }
}
