// <copyright file="VDS_DISK_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid flags for a disk object.
    /// </summary>
    [Flags]
    public enum VDS_DISK_FLAG : uint {

        /// <summary>
        /// The media in a CDROM or DVD drive is an audio CD.
        /// </summary>
        AUDIO_CD = 0x1,

        /// <summary>
        /// The disk is reserved for use only as hot spare.
        /// </summary>
        HOTSPARE = 0x2,

        /// <summary>
        /// This flag is reserved for future use. Do not use.
        /// </summary>
        RESERVE_CAPABLE = 0x4,

        /// <summary>
        /// The disk is masked.
        /// </summary>
        MASKED = 0x8,

        /// <summary>
        /// The partition style on disk can be converted between MBR and GPT.
        /// </summary>
        STYLE_CONVERTIBLE = 0x10,

        /// <summary>
        /// The disk is clustered.
        /// </summary>
        CLUSTERED = 0x20,

        /// <summary>
        /// This flag indicates that the disk's read-only attribute, which is
        /// maintained by the Windows operating system, is set. This attribute
        /// can be set by using the <see cref="IVdsDisk.SetFlags"/> method
        /// and cleared by using the <see cref="IVdsDisk.ClearFlags"/>
        /// method. This flag and the corresponding attribute do not necessarily
        /// reflect the actual read-only state of the disk, which is indicated
        /// by the <see cref="CURRENT_READ_ONLY"/> flag.
        /// </summary>
        READ_ONLY = 0x40,

        /// <summary>
        /// The disk hosts the current system volume. If the disk is dynamic and
        /// the volume is a mirror, the flag is set on the disk that holds the
        /// plex that was used as the system volume at startup.
        /// </summary>
        SYSTEM_DISK = 0x80,

        /// <summary>
        /// The disk hosts the current boot volume.
        /// </summary>
        BOOT_DISK = 0x100,

        /// <summary>
        /// The disk contains a pagefile.
        /// </summary>
        PAGEFILE_DISK = 0x200,

        /// <summary>
        /// The disk contains the hibernation volume.
        /// </summary>
        HIBERNATIONFILE_DISK = 0x400,

        /// <summary>
        /// The disk contains the crashdump volume.
        /// </summary>
        CRASHDUMP_DISK = 0x800,

        /// <summary>
        /// The disk is visible to the computer at startup. For GPT, this flag
        /// is set for all disks. For MBR, it is set only for disks that are
        /// visible to the computer's BIOS firmware. (This is generally the
        /// first 12 disks that are connected to the computer and visible to
        /// the BIOS at startup.)
        /// </summary>
        HAS_ARC_PATH = 0x1000,

        /// <summary>
        /// The disk is a dynamic disk.
        /// </summary>
        DYNAMIC = 0x2000,

        /// <summary>
        /// This flag is set on the hard disk from which the computer is
        /// configured to start. On computers that use the BIOS firmware, this
        /// is the first hard disk that the firmware detects when the computer
        /// starts up (device 80H, or 81H if 80H is assigned to a USB flash
        /// device). If the user plugs a USB flash device into the computer
        /// before startup, this may cause device 80H to be assigned to the USB
        /// device and may cause 81H to be assigned the first hard disk detected
        /// by the firmware. Note that in that case, this flag is not set on the
        /// USB flash device. On computers that use the Extended Firmware
        /// Interface (EFI), this flag is set on the disk that contains the EFI
        /// System Partition (ESP) that was used to start the computer. Note
        /// that if none of the disks contain an ESP, or if there are multiple
        /// ESPs, this flag is not set on any of the disks.
        /// </summary>
        BOOT_FROM_DISK = 0x4000,

        /// <summary>
        /// This flag indicates that the disk is in a read-only state. If it is
        /// not set, the disk is read/write. Unlike the <see cref="READ_ONLY"/>
        /// flag, which is used to change the disk's read-only attribute
        /// maintained by the Windows operating system, this flag reflects the
        /// actual disk state. This flag cannot be set by using the
        /// <see cref="IVdsDisk.SetFlags"/> method or cleared by using the 
        /// <see cref="IVdsDisk.ClearFlags"/> method. The disk will be in a
        /// read-only state if its read-only attribute is set. However, a disk
        /// can be in a read-only state even if its read-only attribute is not
        /// set, if the underlying hardware is read-only. For example, if the
        /// LUN is in read-only state, or if the disk is a virtual hard disk
        /// that resides on a volume that is read-only, the underlying hardware
        /// is read-only, and therefore the disk is in a read-only.
        /// </summary>
        CURRENT_READ_ONLY = 0x8000,

        REFS_NOT_SUPPORTED = 0x10000
    }
}
