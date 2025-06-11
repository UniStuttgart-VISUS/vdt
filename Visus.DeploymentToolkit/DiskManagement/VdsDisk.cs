// <copyright file="VdsDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// A <see cref="IDisk"/> as enumerated by the <see cref="VdsService"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class VdsDisk : IDisk {

        #region Public properties
        /// <summary>
        /// Gets the underlying advanced disk interface that allows the
        /// <see cref="VdsService"/> perform changes to the disk.
        /// </summary>
        public IVdsAdvancedDisk AdvancedDisk => (IVdsAdvancedDisk) this._disk;

        /// <summary>
        /// Gets the underlying advanced disk interface that allows for changing
        /// the type of a partition.
        /// </summary>
        public IVdsAdvancedDisk2 AdvancedDisk2
            => (IVdsAdvancedDisk2) this._disk;

        /// <inheritdoc />
        public StorageBusType BusType
            => (StorageBusType) this.Properties.BusType;

        /// <summary>
        /// Gets the underlying disk interface that allows the
        /// <see cref="VdsService"/> perform changes to the disk.
        /// </summary>
        public IVdsDisk Disk => this._disk;

        /// <inheritdoc />
        public DiskFlags Flags { get; }

        /// <inheritdoc />
        public string FriendlyName => this.Properties.FriendlyName;

        /// <inheritdoc />
        public Guid ID => Properties.Id;

        /// <inheritdoc />
        public string Path => this.Properties.DevicePath;

        /// <inheritdoc />
        public IEnumerable<IPartition> Partitions => this._partitions.Value;

        /// <inheritdoc />
        public PartitionStyle PartitionStyle
            => (PartitionStyle) this.Properties.PartitionStyle;

        /// <summary>
        /// Gets the disk properties.
        /// </summary>
        public VDS_DISK_PROP Properties { get; }

        /// <inheritdoc />
        public uint SectorSize => this.Properties.BytesPerSector;

        /// <inheritdoc />
        public ulong Size => this.Properties.Size;

        /// <inheritdoc />
        public IEnumerable<(IVolume, IPartition)> VolumePartitions {
            get {
                var partitions = this._partitions.Value.ToArray();
                foreach (var v in this._volumes.Value) {
                    var sdn = v.StorageDeviceNumber;
                    if (sdn.PartitionNumber < partitions.Length) {
                        yield return (v, partitions[sdn.PartitionNumber]);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IVolume> Volumes => this._volumes.Value;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void AssignDriveLetter(ulong offset, char letter) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                throw new InvalidOperationException(Errors.NoAdvancedDisk);
            }

            disk.AssignDriveLetter(offset, letter);
        }

        /// <inheritdoc />
        public void DeleteDriveLetter(ulong offset, char letter) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                throw new InvalidOperationException(Errors.NoAdvancedDisk);
            }

            disk.DeleteDriveLetter(offset, letter);
        }

        /// <inheritdoc />
        public Task<VDS_ASYNC_OUTPUT> FormatPartitionAsync(ulong offset,
                VDS_FILE_SYSTEM_TYPE type,
                string label,
                uint unitAllocationSize,
                FormatFlags flags,
                CancellationToken cancellationToken) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                throw new InvalidOperationException(Errors.NoAdvancedDisk);
            }

            bool force = ((flags & FormatFlags.Force) != 0);
            bool quick = ((flags & FormatFlags.Quick) != 0);
            bool compress = ((flags & FormatFlags.EnableCompression) != 0);

            cancellationToken.ThrowIfCancellationRequested();
            disk.FormatPartition(offset,
                type,
                label,
                unitAllocationSize,
                force,
                quick,
                compress,
                out var async);
            return async.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public char? GetDriveLetter(ulong offset) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                return null;
            }

            try {
                disk.GetDriveLetter(offset, out var retval);
                return retval;
            } catch {
                return null;
            }
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.FriendlyName} ({this.ID})";
        #endregion

        #region Internal constructors
        internal VdsDisk(IVdsDisk disk, DiskFlags flags = DiskFlags.None) {
            this._disk = disk ?? throw new ArgumentNullException(nameof(disk));
            this._disk.GetProperties(out var props);
            this.Properties = props;
            this._partitions = new(this.GetPartitions);
            this._volumes = new(this.GetVolumes);

            // Determine the properties of the disk.
            this.Flags = flags;
            if (this.Properties.Flags.HasFlag(VDS_DISK_FLAG.READ_ONLY)) {
                this.Flags |= DiskFlags.ReadOnly;
            }
            if (this.Properties.Flags.HasFlag(VDS_DISK_FLAG.CURRENT_READ_ONLY)) {
                this.Flags |= DiskFlags.ReadOnly;
            }
            if (this.Properties.Flags.HasFlag(VDS_DISK_FLAG.AUDIO_CD)) {
                this.Flags |= DiskFlags.ReadOnly;
                //this.Flags |= DiskFlags.Removable;
            }
            //if (this._properties.Flags.HasFlag(VDS_DISK_FLAG.STYLE_CONVERTIBLE)) {
            //    this.Flags |= DiskFlags.StyleConvertible;
            //}
            try {
                this._disk.GetPack(out var pack);
            } catch {
                // If we cannot get the pack, we assume the disk is uninitalised.
                this.Flags |= DiskFlags.Uninitialised;
            }
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Queries the partitions on the disk without accessing the cache.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<VdsPartition> GetPartitions() {
            if (this._disk is IVdsAdvancedDisk disk) {
                try {
                    disk.QueryPartitions(out var props, out var _);
                    return props.Select(p => new VdsPartition(this, p));
                } catch { /* We can ignore this for unit'd disks. */ }
            }
            return Enumerable.Empty<VdsPartition>();
        }

        /// <summary>
        /// Queries the volumes on the disk without accessing the cache.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<VdsVolume> GetVolumes() {
            this._disk.GetPack(out var pack);
            pack.QueryVolumes(out var enumerator);
            return enumerator.Enumerate<IVdsVolume>()
                .Select(v => new VdsVolume(v));
        }
        #endregion

        #region Private fields
        private readonly IVdsDisk _disk;
        private readonly Lazy<IEnumerable<VdsPartition>> _partitions;
        private readonly Lazy<IEnumerable<VdsVolume>> _volumes;
        #endregion
    }
}
