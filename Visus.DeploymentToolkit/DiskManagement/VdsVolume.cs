// <copyright file="VdsVolume.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Implementation of <see cref="IVolume"/> based on the VDS.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    [SupportedOSPlatform("windows")]
    internal sealed class VdsVolume : IVolume {

        #region Public properties
        /// <inheritdoc />
        public FileSystem FileSystem => (FileSystem) _fileSystem.Type;

        /// <inheritdoc />
        public string? Label => _fileSystem.Label;

        /// <inheritdoc />
        public IEnumerable<string> Mounts => _mounts.Value;

        /// <inheritdoc />
        public string Name => _properties.Name;

        /// <inheritdoc />
        public ulong Size => _properties.Size;
        #endregion

        #region Internal contructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="volume"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal VdsVolume(IVdsVolume volume) {
            this._volume = volume
                ?? throw new ArgumentNullException(nameof(volume));

            if (this._volume is IVdsVolumeMF mf) {
                mf.GetFileSystemProperties(out _fileSystem);
            }

            this._mounts = new(() => {
                if (this._volume is IVdsVolumeMF mf) {
                    mf.QueryAccessPaths(out var retval, out var _);
                    return retval;
                } else {
                    return Enumerable.Empty<string>();
                }
            });

            this._volume.GetProperties(out _properties);
        }
        #endregion

        #region Private fields
        private readonly VDS_FILE_SYSTEM_PROP _fileSystem;
        private readonly Lazy<IEnumerable<string>> _mounts;
        private readonly VDS_VOLUME_PROP _properties;
        private readonly IVdsVolume _volume;
        #endregion
    }
}
