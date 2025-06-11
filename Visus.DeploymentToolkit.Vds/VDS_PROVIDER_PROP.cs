// <copyright file="VDS_PROVIDER_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a provider object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VDS_PROVIDER_PROP {

        /// <summary>
        /// The GUID of the provider object.
        /// </summary>
        public Guid ID;

        /// <summary>
        /// A string representing the name of the provider.
        /// </summary>
        public string Name;

        /// <summary>
        /// The version-specific GUID of the provider.
        /// </summary>
        public Guid VersionID;

        /// <summary>
        /// A string representing the version of the provider.
        /// </summary>
        public string Version;

        /// <summary>
        /// The provider types enumerated by <see cref="VDS_PROVIDER_TYPE">.
        /// </summary>
        public VDS_PROVIDER_TYPE Type;

        /// <summary>
        /// The provider flags enumerated by <see cref="VDS_PROVIDER_FLAG">.
        /// </summary>
        public VDS_PROVIDER_FLAG Flags;

        /// <summary>
        /// The size of a stripe to be used across multiple disks managed by a
        /// software provider.
        /// </summary>
        /// <remarks>
        /// A stripe size must be a power of two. Each bit in the 32-bit integer
        /// represents a size, in bytes. For example, if the nth bit is set,
        /// then VDS supports stripe size of 2^n. Thus, bits 0 through 31 can
        /// specify 2^0 through 2^31. The basic provider sets this value to
        /// zero. The dynamic stripe size can be any power of 2 ranging from 512
        /// to 1MB.
        /// </remarks>
        public uint StripeSizeFlags;

        /// <summary>
        /// The rebuild priority used by software providers to specify the
        /// regeneration order when a mirrored or striped with parity (RAID-5)
        /// volume requires rebuilding.
        /// </summary>
        /// <remarks>
        /// Priority levels are 0 (lowest priority) to 15 (highest priority).
        /// VDS propagates the priority to all new volumes created by the
        /// provider. Thus, all volumes managed by a provider have the same
        /// rebuild priority. This member does not apply to the basic provider
        /// and is zero for the dynamic provider.
        /// </remarks>
        public short RebuildPriority;
    }
}
