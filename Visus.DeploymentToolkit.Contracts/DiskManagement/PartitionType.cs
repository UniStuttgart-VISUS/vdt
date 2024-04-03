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
    public sealed class PartitionType {

        #region Public class properties
        /// <summary>
        /// Gets all well-known partition types.
        /// </summary>
        public static IEnumerable<PartitionType> All => _all.Value;
        #endregion

        /// <summary>
        /// Gets all partition types with matching GPT GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static IEnumerable<PartitionType> FromGpt(Guid guid)
            => All.Where(t => t.Guid == guid);

        /// <summary>
        /// Gets all partition types with matching MBR ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<PartitionType> FromMbr(byte id)
            => All.Where(t => t.ID == RemoveMsftHidden(id));

        #region Known partition types
        public static readonly PartitionType Fat12 = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x01, "FAT12");

        public static readonly PartitionType Fat16LessThan32Mb = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x04, "FAT16 (< 32MB)");

        public static readonly PartitionType Fat16 = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x06, "FAT16");

        public static readonly PartitionType Ntfs = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x07, "NTFS");

        public static readonly PartitionType Fat32 = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x0b, "FAT32");

        public static readonly PartitionType Fat32Lba = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x0c, "FAT32 LBA");

        public static readonly PartitionType Fat16Lba = new(
            "EBD0A0A2-B9E5-4433-87C0-68B6B72699C7", 0x0e, "FAT16 LBA");

        public static readonly PartitionType WindowsEfi = new(
            "E3C9E316-0B5C-4DB8-817D-F92DF00215AE", 0x0c, "Microsoft reserved");

        public static readonly PartitionType WindowsRe = new(
            "DE94BBA4-06D1-4D40-A16A-BFD50179D6AC", 0x27, "Windows RE");

        public static readonly PartitionType WindowsLdmData = new(
            "AF9B60A0-1431-4F62-BC68-3311714A69AD", 0x42, "Windows LDM data");

        public static readonly PartitionType WindowsLdmMetadata = new(
            "5808C8AA-7E8F-42E0-85D2-E1E90434CFB3", 0x42, "Windows LDM metadata");

        public static readonly PartitionType WindowsStorageSpaces = new(
            "E75CAF8F-F680-4CEE-AFA3-B001E56EFC2D", 0x42, "Windows Storage Spaces");

        public static readonly PartitionType LinuxSwap = new(
            "0657FD6D-A4AB-43C4-84E5-0933C84B4F4F", 0x82, "Linux swap");

        public static readonly PartitionType LinuxFileSystem = new(
            "0FC63DAF-8483-4772-8E79-3D69D8477DE4", 0x82, "Linux file system");

        public static readonly PartitionType LinuxReserved = new(
            "8DA63339-0007-60C0-C436-083AC8230908", 0x83, "Linux reserved");

        public static readonly PartitionType Lvm = new(
            "E6D6D379-F507-44C2-A23C-238F2A3DF928", 0x8e, "Linux LVM");

        public static readonly PartitionType EfiSystem = new(
            "C12A7328-F81F-11D2-BA4B-00A0C93EC93B", 0xef, "EFI system partition");

        public static readonly PartitionType EfiMbr = new(
            "024DEE41-33E7-11D3-9D69-0008C781F39F", 0xef, "MBR partition scheme");

        public static readonly PartitionType EfiBoot = new(
            "21686148-6449-6E6F-744E-656564454649", 0xef, "BIOS boot partition");
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the GPT GUID of the partition type.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets the MBR ID of the partition type.
        /// </summary>
        public byte ID { get; }

        /// <summary>
        /// Gets the human readable name of the partition type.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Public methods
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
        private PartitionType(string guid, byte mbr, string name) {
            this.Guid = Guid.Parse(guid);
            this.ID = mbr;
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
