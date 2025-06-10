// <copyright file="WmiPartition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Management;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Implementation of <see cref="IPartition"/> for WMI.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class WmiPartition : IPartition, IDisposable {

        #region Public casts
        /// <summary>
        /// Converts a <see cref="WmiPartition"/> to the underlying WMI object.
        /// </summary>
        /// <param name="partition"></param>
        public static explicit operator ManagementBaseObject(
            WmiPartition partition) => partition?._partition!;
        #endregion

        #region Public constants
        /// <summary>
        /// The name of the management class used to represent a partition.
        /// </summary>
        public const string Class = "MSFT_Partition";
        #endregion

        #region Public properties
        /// <inheritdoc />
        public PartitionFlags Flags { get; }

        /// <inheritdoc />
        public uint Index {
            get {
                ObjectDisposedException.ThrowIf(this._partition is null, this);
                return (uint) this._partition["PartitionNumber"];
            }
        }

        /// <inheritdoc />
        public string? Name {
            get {
                ObjectDisposedException.ThrowIf(this._partition is null, this);
                return (this.Style == PartitionStyle.Gpt)
                    ? this._partition["Guid"] as string
                    : null;
            }
        }

        /// <inheritdoc />
        public ulong Offset {
            get {
                ObjectDisposedException.ThrowIf(this._partition is null, this);
                return (ulong) this._partition["Offset"];
            }
        }

        /// <inheritdoc />
        public ulong Size {
            get {
                ObjectDisposedException.ThrowIf(this._partition is null, this);
                return (ulong) this._partition["Size"];
            }
        }

        /// <inheritdoc />
        public PartitionStyle Style { get; }

        /// <inheritdoc />
        public PartitionType Type { get; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="partition">The management object representing the
        /// partition.</param>
        /// <param name="style">The partition style of the disk the partition
        /// belongs to.</param>
        internal WmiPartition(ManagementBaseObject partition,
                PartitionStyle style) {
            this._partition = partition
                ?? throw new ArgumentNullException(nameof(partition));
            if (!this._partition.ClassPath.ClassName.EqualsIgnoreCase(Class)) {
                throw new ArgumentException(string.Format(
                    Errors.UnexpectedManagementClass,
                    Class,
                    this._partition.ClassPath.ClassName));
            }

            this.Style = style;
            this.Type = this.Style switch {
                PartitionStyle.Gpt => new(Guid.Parse((string) this._partition["GptType"])),
                PartitionStyle.Mbr => new((byte) (ushort) this._partition["MbrType"]),
                _ => throw new ArgumentException()
            };

            if ((bool) this._partition["IsActive"]) {
                this.Flags |= PartitionFlags.Active;
            }

            if ((bool) this._partition["IsBoot"]) {
                this.Flags |= PartitionFlags.Boot;
            }

            if ((bool) this._partition["IsSystem"]) {
                this.Flags |= PartitionFlags.System;
            }
        }
        #endregion

        #region Private methods
        private void Dispose(bool disposing) {
            if (disposing) {
                this._partition?.Dispose();
                this._partition = null!;
            }
        }
        #endregion

        #region Private fields
        private ManagementBaseObject _partition;
        #endregion
    }
}
