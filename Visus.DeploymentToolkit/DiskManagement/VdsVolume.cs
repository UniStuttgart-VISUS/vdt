// <copyright file="VdsVolume.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Properties;
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

        /// <summary>
        /// Gets the device paths of the volume.
        /// </summary>
        public IEnumerable<string> Paths { get; } = [ ];

        /// <inheritdoc />
        public ulong Size => _properties.Size;

        /// <summary>
        /// Gets the <see cref="STORAGE_DEVICE_NUMBER"/> for the volume, which
        /// allows for identifying the physical disk and partition that
        /// contains the volume.
        /// </summary>
        public STORAGE_DEVICE_NUMBER StorageDeviceNumber
            => _storageDeviceNumber.Value;
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

            if (this._volume is IVdsVolumeMF3 mf3) {
                this.Paths = mf3.QueryVolumeGuidPathNames().ToArray();
            }

            if (!this.Paths.Any()) {
                // If we could not get the GUID via IVdsVolumeMF3, try to
                // convert the mount point the device path like in
                // https://www.codeproject.com/Articles/13839/How-to-Prepare-a-USB-Drive-for-Safe-Removal
                this.Paths = this.Mounts.Select(m => m.Insert(0, @"\\.\"));
            }

            this._storageDeviceNumber = new(() => {
                if (!this.Paths.Any()) {
                    throw new InvalidOperationException(Errors.NoVolumePath);
                }
                return GetStorageDeviceNumber(this.Paths.First());
            });
        }
        #endregion

        #region Private methods
        private static IEnumerable<DISK_EXTENT> GetExtents(string path,
                int maxExtents = 16) {
            Debug.Assert(path is not null);

            // As per https://stackoverflow.com/questions/33615577/how-to-enumerate-the-volumes-on-a-phyiscal-drive-using-windows-api,
            // the path of the volume must not end with a separator.
            using (var f = new FileStream(
                    path.TrimEnd(Path.DirectorySeparatorChar),
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite)) {
                // Allocate a buffer for the VOLUME_DISK_EXTENTS structure and
                // additional 'maxExtents' DISK_EXTENT structures after that.
                var cnt = Marshal.SizeOf<VOLUME_DISK_EXTENTS>()
                    + maxExtents * Marshal.SizeOf<DISK_EXTENT>();
                var buf = Marshal.AllocHGlobal(cnt);

                try {
                    // TODO: this does not work with ERROR_INVALID_FUNCTION. No idea why ...
                    Kernel32.DeviceIoControl(f.SafeFileHandle,
                        Kernel32.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                        nint.Zero, 0,
                        buf, (uint) cnt);

                    var de = Marshal.PtrToStructure<VOLUME_DISK_EXTENTS>(buf);
                    yield return de.Extents;

                    var o = buf + Marshal.SizeOf<VOLUME_DISK_EXTENTS>();
                    for (int i = 1; i < de.NumberOfDiskExtents; ++i) {
                        var e = Marshal.PtrToStructure<DISK_EXTENT>(o);
                        yield return e;
                        o += Marshal.SizeOf<DISK_EXTENT>();
                    }

                } finally {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="STORAGE_DEVICE_NUMBER"/> for the given volume,
        /// which is "[t]he magic link between storage volumes and their disk"
        /// according to
        /// https://www.codeproject.com/Articles/13839/How-to-Prepare-a-USB-Drive-for-Safe-Removal
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static STORAGE_DEVICE_NUMBER GetStorageDeviceNumber(
                string path) {
            Debug.Assert(path is not null);
            using (var f = new FileStream(
                    path.TrimEnd(Path.DirectorySeparatorChar),
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite)) {
                STORAGE_DEVICE_NUMBER retval = new();
                Kernel32.DeviceIoControl(f.SafeFileHandle,
                    Kernel32.IOCTL_STORAGE_GET_DEVICE_NUMBER,
                    out retval);
                return retval;
            }
        }
        #endregion

        #region Private fields
        private readonly VDS_FILE_SYSTEM_PROP _fileSystem;
        private readonly Lazy<IEnumerable<string>> _mounts;
        private readonly VDS_VOLUME_PROP _properties;
        private readonly Lazy<STORAGE_DEVICE_NUMBER> _storageDeviceNumber;
        private readonly IVdsVolume _volume;
        #endregion
    }
}
