// <copyright file="VdsDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
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
    internal sealed class VdsDisk : IAdvancedDisk {

        #region Public properties
        /// <inheritdoc />
        public StorageBusType BusType
            => (StorageBusType) this._properties.BusType;

        /// <inheritdoc />
        public DiskFlags Flags { get; }

        /// <inheritdoc />
        public string FriendlyName => this._properties.FriendlyName;

        /// <inheritdoc />
        public Guid ID => _properties.Id;

        /// <inheritdoc />
        public IEnumerable<IPartition> Partitions => this._partitions.Value;

        /// <inheritdoc />
        public PartitionStyle PartitionStyle
            => (PartitionStyle) this._properties.PartitionStyle;

        /// <inheritdoc />
        public uint SectorSize => this._properties.BytesPerSector;

        /// <inheritdoc />
        public ulong Size => this._properties.Size;

        /// <inheritdoc />
        public IEnumerable<Tuple<IVolume, IPartition>> VolumePartitions {
            get {
                var partitions = this._partitions.Value.ToArray();
                foreach (var v in this._volumes.Value) {
                    var sdn = v.StorageDeviceNumber;
                    if (sdn.PartitionNumber < partitions.Length) {
                        yield return new(v, partitions[sdn.PartitionNumber]);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IVolume> Volumes => _volumes.Value;
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
        public Task<VDS_ASYNC_OUTPUT> CleanAsync(CleanFlags flags,
                CancellationToken cancellationToken) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                throw new InvalidOperationException(Errors.NoAdvancedDisk);
            }

            var force = ((flags & CleanFlags.Force) != 0);
            var forceOem = ((flags & CleanFlags.ForceOem) != 0);
            var fullClean = ((flags & CleanFlags.FullClean) != 0);

            cancellationToken.ThrowIfCancellationRequested();
            disk.Clean(force, forceOem, fullClean, out var async);

            return async.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task ConvertAsync(PartitionStyle style) {
            switch (style) {
                case PartitionStyle.Gpt:
                    this._disk.ConvertStyle(VDS_PARTITION_STYLE.GPT);
                    return Task.CompletedTask;

                case PartitionStyle.Mbr:
                    this._disk.ConvertStyle(VDS_PARTITION_STYLE.MBR);
                    return Task.CompletedTask;

                default:
                    throw new ArgumentException();
            }
        }

        /// <inheritdoc />
        public Task<VDS_ASYNC_OUTPUT> CreatePartitionAsync(ulong offset,
                ulong size,
                MbrPartitionParameters parameters,
                CancellationToken cancellationToken) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                throw new InvalidOperationException(Errors.NoAdvancedDisk);
            }

            CREATE_PARTITION_PARAMETERS cpp = new() {
                Style = VDS_PARTITION_STYLE.MBR,
                MbrPartInfo = parameters
            };

            cancellationToken.ThrowIfCancellationRequested();
            disk.CreatePartition(offset, size, ref cpp, out var async);
            return async.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<VDS_ASYNC_OUTPUT> CreatePartitionAsync(ulong offset,
                ulong size,
                VDS_PARTITION_INFO_GPT parameters,
                CancellationToken cancellationToken) {
            if (this._disk is not IVdsAdvancedDisk disk) {
                throw new InvalidOperationException(Errors.NoAdvancedDisk);
            }

            CREATE_PARTITION_PARAMETERS cpp = new() {
                Style = VDS_PARTITION_STYLE.GPT,
                GptPartInfo = parameters
            };

            cancellationToken.ThrowIfCancellationRequested();
            disk.CreatePartition(offset, size, ref cpp, out var async);
            return async.WaitAsync(cancellationToken);
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
            this._disk.GetProperties(out _properties);
            this._partitions = new(() => {
                if (this._disk is IVdsAdvancedDisk disk) {
                    try {
                        disk.QueryPartitions(out var props, out var _);
                        return props.Select(p => new VdsPartition(p));
                    } catch { /* We can ignore this for unit'd disks. */ }
                }

                return Enumerable.Empty<VdsPartition>();
            });
            this._volumes = new(() => {
                disk.GetPack(out var pack);
                pack.QueryVolumes(out var enumerator);
                return enumerator.Enumerate<IVdsVolume>()
                    .Select(v => new VdsVolume(v));
            });


            this.Flags = flags;
            if (this._properties.Flags.HasFlag(VDS_DISK_FLAG.READ_ONLY)) {
                this.Flags |= DiskFlags.ReadOnly;
            }
            if (this._properties.Flags.HasFlag(VDS_DISK_FLAG.CURRENT_READ_ONLY)) {
                this.Flags |= DiskFlags.ReadOnly;
            }
            if (this._properties.Flags.HasFlag(VDS_DISK_FLAG.AUDIO_CD)) {
                this.Flags |= DiskFlags.ReadOnly;
                this.Flags |= DiskFlags.Removable;
            }
            if (this._properties.Flags.HasFlag(VDS_DISK_FLAG.STYLE_CONVERTIBLE)) {
                this.Flags |= DiskFlags.StyleConvertible;
            }
        }
        #endregion

        #region Private fields
        private readonly IVdsDisk _disk;
        private readonly Lazy<IEnumerable<VdsPartition>> _partitions;
        private readonly VDS_DISK_PROP _properties;
        private readonly Lazy<IEnumerable<VdsVolume>> _volumes;
        #endregion
    }
}
