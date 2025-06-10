// <copyright file="WmiDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// A <see cref="IDisk"/> as enumerated by the <see cref="VdsService"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class WmiDisk : IDisk, IDisposable {

        #region Public casts
        /// <summary>
        /// Converts a <see cref="WmiDisk"/> to the underlying WMI object.
        /// </summary>
        /// <param name="disk"></param>
        public static explicit operator ManagementObject(
            WmiDisk disk) => disk?._disk!;
        #endregion

        #region Public constants
        /// <summary>
        /// The name of the management class used to represent a disk.
        /// </summary>
        public const string Class = "MSFT_Disk";
        #endregion

        #region Public properties
        /// <inheritdoc />
        public StorageBusType BusType { get; }

        /// <inheritdoc />
        public DiskFlags Flags { get; }

        /// <inheritdoc />
        public string FriendlyName {
            get {
                ObjectDisposedException.ThrowIf(this._disk is null, this);
                return (string) this._disk["FriendlyName"];
            }
        }

        /// <inheritdoc />
        public Guid ID { get; }

        /// <inheritdoc />
        public string Path {
            get {
                ObjectDisposedException.ThrowIf(this._disk is null, this);
                return (string) this._disk["Path"];
            }
        }

        /// <inheritdoc />
        public IEnumerable<IPartition> Partitions => this._partitions.Value;

        /// <inheritdoc />
        public PartitionStyle PartitionStyle { get; }

        /// <inheritdoc />
        public uint SectorSize {
            get {
                ObjectDisposedException.ThrowIf(this._disk is null, this);
                return (uint) this._disk["LogicalSectorSize"];
            }
        }

        /// <inheritdoc />
        public ulong Size {
            get {
                ObjectDisposedException.ThrowIf(this._disk is null, this);
                return (ulong) this._disk["Size"];
            }
        }

        /// <inheritdoc />
        public IEnumerable<(IVolume, IPartition)> VolumePartitions
            => this._volumePartitions.Value.Cast<(IVolume, IPartition)>();

        /// <inheritdoc />
        public IEnumerable<IVolume> Volumes => this._volumes.Value;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets all partitions on the disk without relying on cached data.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WmiPartition> GetPartitions() {
            ObjectDisposedException.ThrowIf(this._disk is null, this);
            var id = (string) this._disk["ObjectId"];
            var retval = this._disk.Scope.QueryObjects("ASSOCIATORS OF "
                + $@"{{{Class}.ObjectId=""{id.EscapeWql()}""}} "
                + "WHERE AssocClass = MSFT_DiskToPartition")
                .Select(p => new WmiPartition(p, this.PartitionStyle));
            return retval;
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.FriendlyName} ({this.ID})";
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="disk">The WMI object representing the disk.</param>
        /// <param name="flags"></param>
        internal WmiDisk(ManagementObject disk, DiskFlags flags = DiskFlags.None) {
            this._disk = disk ?? throw new ArgumentNullException(nameof(disk));
            if (!this._disk.ClassPath.ClassName.EqualsIgnoreCase(Class)) {
                throw new ArgumentException(string.Format(
                    Errors.UnexpectedManagementClass,
                    Class,
                    this._disk.ClassPath.ClassName));
            }

            var busType = (ushort) this._disk["BusType"];
            this.BusType = (StorageBusType) busType;

            try {
                this.ID = Guid.Parse((string) this._disk["UniqueId"]);
            } catch {
                this.ID = Guid.Empty;
            }

            var loc = this._disk["Location"];
            var pa = this._disk["Path"];

            var style = (ushort) this._disk["PartitionStyle"];
            this.PartitionStyle = (PartitionStyle) style;

            this._partitions = new(this.GetPartitions);

            this._volumePartitions = new(() => {
               ObjectDisposedException.ThrowIf(this._disk is null, this);
                var id = (string) this._disk["ObjectId"];
                var partitions = this._disk.Scope.QueryObjects("ASSOCIATORS OF "
                    + $@"{{{Class}.ObjectId=""{id.EscapeWql()}""}} "
                    + "WHERE AssocClass = MSFT_DiskToPartition");

                var volumes = partitions.Select(p => {
                    var id = (string) p["ObjectId"];
                    var v = this._disk.Scope.QueryObjects("ASSOCIATORS "
                        + $@"OF {{{WmiPartition.Class}.ObjectId="
                        + $@"""{id.EscapeWql()}""}} "
                        + "WHERE AssocClass = MSFT_PartitionToVolume")
                        .SingleOrDefault();
                    if (v is null) {
                        p.Dispose();
                        return (null!, null!);
                    } else {
                        var volume = new WmiVolume(v);
                        var partition = new WmiPartition(p, this.PartitionStyle);
                        return (volume, partition);
                    }
                });

                return from v in volumes
                       where v.volume is not null
                       select (v.volume, v.partition);
            });

            this._volumes = new(() => {
                ObjectDisposedException.ThrowIf(this._disk is null, this);
                return this.Partitions.Select(p => {
                    var obj = (ManagementObject) (WmiPartition) p;
                    var id = (string) obj["ObjectId"];
                    var volume = this._disk.Scope.QueryObjects("ASSOCIATORS "
                        + $@"OF {{{WmiPartition.Class}.ObjectId="
                        + $@"""{id.EscapeWql()}""}} "
                        + "WHERE AssocClass = MSFT_PartitionToVolume")
                        .SingleOrDefault();
                    return (volume is not null) ? new WmiVolume(volume) : null;
                }).Where(v => v != null).Cast<WmiVolume>();
            });

            // Determine the properties of the disk.
            this.Flags = flags;
            if ((bool) this._disk["IsReadOnly"]) {
                this.Flags |= DiskFlags.ReadOnly;
            }

            if ((bool) this._disk["IsOffline"]) {
                this.Flags |= DiskFlags.Offline;
            }

            // For some really weird reason, I sometimes got an array instead of
            // the documented ushort, so we check for that.
            var status = this._disk["OperationalStatus"] switch {
                ushort[] a when (a.Length > 0) => a[0],
                ushort s => s,
                _ => 0
            };

            switch (status) {
                case 0xD013: // Offline
                    this.Flags |= DiskFlags.Uninitialised;
                    break;
            }
        }
        #endregion

        #region Private methods
        private void Dispose(bool disposing) {
            if (disposing && (this._disk is not null)) {
                this._disk?.Dispose();
                this._disk = null!;

                if (this._partitions.IsValueCreated) {
                    foreach (var p in this._partitions.Value) {
                        p.Dispose();
                    }
                }

                if (this._volumePartitions.IsValueCreated) {
                    foreach (var vp in this._volumePartitions.Value) {
                        vp.Item1.Dispose();
                        vp.Item2.Dispose();
                    }
                }

                if (this._volumes.IsValueCreated) {
                    foreach (var v in this._volumes.Value) {
                        v.Dispose();
                    }
                }
            }
        }
        #endregion

        #region Private fields
        private ManagementObject _disk;
        private readonly Lazy<IEnumerable<WmiPartition>> _partitions;
        private readonly Lazy<IEnumerable<(WmiVolume, WmiPartition)>>
            _volumePartitions;
        private readonly Lazy<IEnumerable<WmiVolume>> _volumes;
        #endregion
    }
}
