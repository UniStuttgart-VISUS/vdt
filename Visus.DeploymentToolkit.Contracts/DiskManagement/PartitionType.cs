// <copyright file="ParitionType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Represents a type of partition.
    /// </summary>
    public sealed class PartitionType : IEquatable<PartitionType> {

        #region Public class properties
        /// <summary>
        /// Gets all well-known partition types defined as fields of this class.
        /// </summary>
        public static IEnumerable<PartitionType> All => _all.Value;
        #endregion

        #region Public class methods
        /// <summary>
        /// Gets all partition types with matching GPT GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static IEnumerable<PartitionType> FromGpt(Guid guid)
            => All.Where(t => t.Gpt == guid);

        /// <summary>
        /// Gets all partition types with matching MBR ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<PartitionType> FromMbr(byte id)
            => All.Where(t => t.Mbr == RemoveMsftHidden(id));
        #endregion

        #region Known partition types
        /// <summary>
        /// FAT 12.
        /// </summary>
        public static readonly PartitionType Fat12 = new(0x01, "FAT12");

        /// <summary>
        /// FAT16 < 32 MB.
        /// </summary>
        public static readonly PartitionType Fat16LessThan32Mb
            = new(0x04, "FAT16");

        /// <summary>
        /// FAT16.
        /// </summary>
        public static readonly PartitionType Fat16 = new(0x06, "FAT16");

        /// <summary>
        /// NTFS or HPFS.
        /// </summary>
        public static readonly PartitionType Ntfs = new(0x07, "NTFS");

        /// <summary>
        /// FAT32.
        /// </summary>
        public static readonly PartitionType Fat32 = new(0x0b, "FAT32");

        /// <summary>
        /// FAT32 LBA.
        /// </summary>
        public static readonly PartitionType Fat32Lba = new(0x0c, "FAT32 LBA");

        /// <summary>
        /// FAT16 LBA.
        /// </summary>
        public static readonly PartitionType Fat16Lba = new(0x0e, "FAT16 LBA");

        /// <summary>
        /// Microsoft basic data (FAT and NTFS on GPT).
        /// </summary>
        public static readonly PartitionType MicrosoftBasicData = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", "Microsoft basic data");

        /// <summary>
        /// Microsoft reserved parittion.
        /// </summary>
        public static readonly PartitionType WindowsEfi = new(
            "E3C9E316-0B5C-4DB8-817D-F92DF00215AE", 0x0c, "Microsoft reserved");

        /// <summary>
        /// Windows Recovery Environment.
        /// </summary>
        public static readonly PartitionType WindowsRe = new(
            "DE94BBA4-06D1-4D40-A16A-BFD50179D6AC", 0x27, "Windows RE");

        /// <summary>
        /// Data partition of the Windows Logical Disk Manager.
        /// </summary>
        public static readonly PartitionType WindowsLdmData = new(
            "AF9B60A0-1431-4F62-BC68-3311714A69AD", "Windows LDM data");

        /// <summary>
        /// Meta-data partition of the Windows Logical Disk Manager.
        /// </summary>
        public static readonly PartitionType WindowsLdmMetaData = new(
            "5808C8AA-7E8F-42E0-85D2-E1E90434CFB3", "Windows LDM meta data");

        /// <summary>
        /// Windows Storage Spaces (Windows 8 and later).
        /// </summary>
        public static readonly PartitionType WindowsStorageSpaces = new(
            "E75CAF8F-F680-4CEE-AFA3-B001E56EFC2D", "Windows Storage Spaces");

        /// <summary>
        /// Windows software RAID on MBR disks.
        /// </summary>
        public static readonly PartitionType Windows= new(0x42, "Windows");

        /// <summary>
        /// Linux swap partition.
        /// </summary>
        public static readonly PartitionType LinuxSwap = new(
            "0657FD6D-A4AB-43C4-84E5-0933C84B4F4F", 0x82, "Linux swap");

        /// <summary>
        /// Linux native partition.
        /// </summary>
        public static readonly PartitionType LinuxFileSystem = new(
            "0FC63DAF-8483-4772-8E79-3D69D8477DE4", "Linux file system");

        /// <summary>
        /// Linux hybrid MBR.
        /// </summary>
        public static readonly PartitionType LinuxReserved = new(
            "8DA63339-0007-60C0-C436-083AC8230908", "Linux reserved");

        /// <summary>
        /// Linux MBR.
        /// </summary>
        public static readonly PartitionType Linux = new(0x83, "Linux");

        /// <summary>
        /// Linux Logical Volume Manager.
        /// </summary>
        public static readonly PartitionType Lvm = new(
            "E6D6D379-F507-44C2-A23C-238F2A3DF928", 0x8e, "Linux LVM");

        /// <summary>
        /// The EFI system partition (EPS) formatted as FAT containing the EFI
        /// boot firmware.
        /// </summary>
        public static readonly PartitionType EfiSystem = new(
            "C12A7328-F81F-11D2-BA4B-00A0C93EC93B", "EFI system partition");

        /// <summary>
        /// A partition holding a MBR partition table for virtualisation
        /// purposes.
        /// </summary>
        public static readonly PartitionType EfiMbr = new(
            "024DEE41-33E7-11D3-9D69-0008C781F39F", "MBR partition scheme");

        /// <summary>
        /// The EFI boot partition used by GRUB to start on BIOS-based PCs.
        /// </summary>
        public static readonly PartitionType EfiBoot = new(
            "21686148-6449-6E6F-744E-656564454649", "BIOS boot partition");

        /// <summary>
        /// An EFI parition on MBR disks.
        /// </summary>
        public static readonly PartitionType Efi = new(0xef, "EFI");
        #endregion

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="guid">The GUID of the GPT partition type.</param>
        /// <param name="name">The human readable name of the partition type. If
        /// this is <c>null</c>, the constructor will look up the name from the
        /// well-known partition types or use the GUID as final fallback.</param>
        public PartitionType(Guid guid, string? name = null) {
            this.Gpt = guid;
            this.Name = name
                ?? FromGpt(guid).FirstOrDefault()?.Name
                ?? guid.ToString("B");
        }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="id">The MBR partition ID.</param>
        /// <param name="name">The human readable name of the partition type. If
        /// this is <c>null</c>, the constructor will look up the name from the
        /// well-known partition types or use the partition ID as final
        /// fallback.</param>
        public PartitionType(byte id, string? name = null) {
            this.Mbr = id;
            this.Name = name
                ?? FromMbr(id).FirstOrDefault()?.Name
                ?? id.ToString("X2");
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the GPT GUID of the partition type.
        /// </summary>
        /// <remarks>
        /// This property might be <c>null</c> for MBR-only partition types.
        /// </remarks>
        public Guid? Gpt { get; }

        /// <summary>
        /// Answer whether the partition type is for GUID partition tables.
        /// </summary>
        public bool IsGpt => this.Gpt != null;

        /// <summary>
        /// Answer whether the partition type is for Master Boot Record
        /// partition tables.
        /// </summary>
        public bool IsMbr => this.Mbr != null;

        /// <summary>
        /// Gets the MBR ID of the partition type.
        /// </summary>
        /// <remarks>
        /// This property might be <c>null</c> for GPT-only partition types.
        /// </remarks>
        public byte? Mbr { get; }

        /// <summary>
        /// Gets the human readable name of the partition type.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override bool Equals(object? obj)
            => this.Equals(obj as PartitionType);

        /// <inheritdoc />
        public bool Equals(PartitionType? other) {
            if (other == null) {
                return false;
            }

            if (object.ReferenceEquals(this, other)) {
                return true;
            }

            return ((this.Gpt == other.Gpt) && (this.Mbr == other.Mbr));
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return typeof(PartitionType).GetHashCode()
                ^ this.Gpt?.GetHashCode() ?? 0
                ^ this.Mbr?.GetHashCode() ?? 0;
        }

        /// <inheritdoc />
        public override string ToString() => this.Name;
        #endregion

        #region Private class methods
        /// <summary>
        /// Remove the hidden flag from Microsoft's MBR IDs.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static byte RemoveMsftHidden(byte id) {
            return id switch {
                0x11 => 0x01,
                0x14 => 0x04,
                0x16 => 0x06,
                0x17 => 0x07,
                0x1b => 0x0b,
                0x1c => 0x0c,
                0x1e => 0x0e,
                _ => id
            };
        }
        #endregion

        #region Private constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="guid">The string representation of the GPT partition
        /// type.</param>
        /// <param name="name">The human-readable name of the partition type.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/>
        /// is <c>null</c>.</exception>
        private PartitionType(string guid, string name) {
            this.Gpt = Guid.Parse(guid);
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="guid">The string representation of the GPT partition
        /// type.</param>
        /// <param name="id">The MBR ID.</param>
        /// <param name="name">The human-readable name of the partition type.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/>
        /// is <c>null</c>.</exception>
        private PartitionType(string guid, byte id, string name) {
            this.Gpt = Guid.Parse(guid);
            this.Mbr = id;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        #endregion

        #region Private fields
        private static readonly Lazy<IEnumerable<PartitionType>> _all = new(() => {
            var flags = BindingFlags.Public | BindingFlags.Static;
            return from f in typeof(PartitionType).GetFields(flags)
                   where f.FieldType == typeof(PartitionType)
                   let v = f.GetValue(null) as PartitionType
                   where v != null
                   select v;
        });
        #endregion
    }
}
