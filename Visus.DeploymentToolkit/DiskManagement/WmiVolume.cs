// <copyright file="WmiVolume.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Implementation of <see cref="IVolume"/> based on the WMI.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    [SupportedOSPlatform("windows")]
    internal sealed class WmiVolume : IVolume, IDisposable {

        #region Public casts
        /// <summary>
        /// Converts a <see cref="WmiVolume"/> to the underlying WMI object.
        /// </summary>
        /// <param name="volume">The volume to be converted.</param>
        public static explicit operator ManagementBaseObject(
            WmiVolume volume) => volume?._volume!;
        #endregion

        #region Public constants
        /// <summary>
        /// The name of the management class used to represent a volume.
        /// </summary>
        public const string Class = "MSFT_Volume";
        #endregion

        #region Public properties
        /// <inheritdoc />
        public FileSystem FileSystem { get; } = FileSystem.Unknown;

        /// <inheritdoc />
        public string? Label => (string) this._volume["FileSystemLabel"];

        /// <inheritdoc />
        public IEnumerable<string> Mounts => this._mounts.Value;

        /// <inheritdoc />
        public string Name => (string) this._volume["Path"];

        /// <inheritdoc />
        public ulong Size => (ulong) this._volume["Size"];
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Internal contructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="volume"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal WmiVolume(ManagementBaseObject volume) {
            this._volume = volume
                ?? throw new ArgumentNullException(nameof(volume));
            if (!this._volume.ClassPath.ClassName.EqualsIgnoreCase(Class)) {
                throw new ArgumentException(string.Format(
                    Errors.UnexpectedManagementClass,
                    Class,
                    this._volume.ClassPath.ClassName));
            }

            this.FileSystem = ((ushort) this._volume["FileSystemType"]).FromWmi();

            this._mounts = new Lazy<IEnumerable<string>>(() => {
                ObjectDisposedException.ThrowIf(this._volume is null, this);
                var id = (string) this._volume["ObjectId"];
                var partition = this._volume.QueryObjects("ASSOCIATORS OF "
                    + $@"{{{Class}.ObjectId=""{id.EscapeWql()}""}} "
                    + "WHERE AssocClass = MSFT_PartitionToVolume").Single();
                return partition["AccessPaths"] as string[]
                    ?? Array.Empty<string>();
            });
        }
        #endregion

        #region Private methods
        private void Dispose(bool disposing) {
            if (disposing) {
                this._volume?.Dispose();
                this._volume = null!;
            }
        }
        #endregion

        #region Private fields
        private readonly Lazy<IEnumerable<string>> _mounts;
        private ManagementBaseObject _volume;
        #endregion
    }
}
