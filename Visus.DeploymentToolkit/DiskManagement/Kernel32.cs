// <copyright file="Kernel32.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Provides P/Invokes for making certain system calls.
    /// </summary>
    internal static class Kernel32 {

        #region Public constants
        /// <summary>
        /// The FSCTL_IS_VOLUME_DIRTY control code determines whether the
        /// specified volume is dirty.
        /// </summary>
        public const uint FSCTL_IS_VOLUME_DIRTY
            = (((0x00000009) << 16) | ((0) << 14) | ((30) << 2) | (0));

        /// <summary>
        /// The I/O control code for retrieving the disk extents of a volume.
        /// </summary>
        public const uint IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS
            = (0x00000056 << 16);

        /// <summary>
        /// Retrieves the device type, device number, and, for a partitionable
        /// device, the partition number of a device.
        /// </summary>
        public const uint IOCTL_STORAGE_GET_DEVICE_NUMBER
            = (((0x0000002d) << 16) | ((0) << 14) | ((0x0420) << 2) | (0));

        /// <summary>
        /// Retrieves information for each entry in the partition tables for a
        /// disk.
        /// </summary>
        public const uint IOCTL_DISK_GET_DRIVE_LAYOUT
            = (((0x00000007) << 16) | ((0x01) << 14) | ((0x03) << 2) | (0));

        /// <summary>
        /// Partitions a disk as specified by drive layout and partition
        /// information data.
        /// </summary>
        public const uint IOCTL_DISK_SET_DRIVE_LAYOUT
            = (((0x00000007) << 16) | ((0x01 | (0x02)) << 14) | (0x04 << 2) | (0));
        #endregion

        /// <summary>
        /// Sends a control code directly to a specified device driver, allowing
        /// for custom I/O operations.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the Windows API
        /// <c>DeviceIoControl</c> function. It allows communication with device
        /// drivers for custom operations that are not covered by standard
        /// system calls.</remarks>
        /// <param name="device">A handle to the device on which the operation
        /// is to be performed. This handle must be obtained using a function such
        /// as <c>CreateFile</c>.</param>
        /// <param name="ioControlCode">The control code for the operation. This
        /// value identifies the specific operation to be performed.</param>
        /// <param name="inBuffer">A pointer to the input buffer that contains
        /// the data required for the operation. Can be <see langword="null"/> if
        /// no input data is required.</param>
        /// <param name="inBufferSize">The size, in bytes, of the input buffer
        /// pointed to by <paramref name="inBuffer"/>.</param>
        /// <param name="outBuffer">A reference to a pointer that will receive
        /// the output data from the operation. Can be <see langword="null"/>
        /// if no output data is expected.</param>
        /// <param name="outBufferSize">The size, in bytes, of the output buffer
        /// pointed to by <paramref name="outBuffer"/>.</param>
        /// <param name="bytesReturned">When the method returns, contains the
        /// number of bytes returned in the output buffer. This parameter is set
        /// to zero if no output data is returned.</param>
        /// <param name="overlapped">A pointer to an <c>OVERLAPPED</c> structure
        /// for asynchronous operations. Can be <see langword="null"/> for
        /// synchronous operations.</param>
        /// <returns><see langword="true"/> if the operation succeeds; otherwise,
        /// <see langword="false"/>. Call <see cref="Marshal.GetLastWin32Error"/>
        /// to retrieve extended error information if the method fails.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(nint device,
            uint ioControlCode,
            nint inBuffer,
            uint inBufferSize,
            nint outBuffer,
            uint outBufferSize,
            out uint bytesReturned,
            nint overlapped);

        /// <summary>
        /// Sends a control code directly to a specified device driver, allowing
        /// for custom I/O operations.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the Windows API
        /// <c>DeviceIoControl</c> function. It allows communication with device
        /// drivers for custom operations that are not covered by standard
        /// system calls.</remarks>
        /// <param name="device">A handle to the device on which the operation
        /// is to be performed. This handle must be obtained using a function such
        /// as <c>CreateFile</c>.</param>
        /// <param name="ioControlCode">The control code for the operation. This
        /// value identifies the specific operation to be performed.</param>
        /// <param name="inBuffer">A pointer to the input buffer that contains
        /// the data required for the operation. Can be <see langword="null"/> if
        /// no input data is required.</param>
        /// <param name="inBufferSize">The size, in bytes, of the input buffer
        /// pointed to by <paramref name="inBuffer"/>.</param>
        /// <param name="outBuffer">A reference to a pointer that will receive
        /// the output data from the operation. Can be <see langword="null"/>
        /// if no output data is expected.</param>
        /// <param name="outBufferSize">The size, in bytes, of the output buffer
        /// pointed to by <paramref name="outBuffer"/>.</param>
        /// <returns>The number of bytes returned to
        /// <paramref name="outBuffer"/>.</returns>
        /// <exception cref="Win32Exception">If the I/O control failed.
        /// </exception>
        public static uint DeviceIoControl(SafeFileHandle device,
                uint ioControlCode,
                nint inBuffer,
                uint inBufferSize,
                nint outBuffer,
                uint outBufferSize) {
            if (!DeviceIoControl(device.DangerousGetHandle(),
                    ioControlCode,
                    inBuffer,
                    inBufferSize,
                    outBuffer,
                    outBufferSize,
                    out var retval,
                    nint.Zero)) {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return retval;
        }

        /// <summary>
        /// Sends a control code directly to a specified device driver, allowing
        /// for custom I/O operations.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output structure returned
        /// by the IOCTL.</typeparam>
        /// <param name="device">A handle to the device on which the operation
        /// is to be performed. This handle must be obtained using a function such
        /// as <c>CreateFile</c>.</param>
        /// <param name="ioControlCode">The control code for the operation. This
        /// value identifies the specific operation to be performed.</param>
        /// <param name="output">This structure will be filled by the IOCTL.
        /// </param>
        public static void DeviceIoControl<TOutput>(SafeFileHandle device,
                uint ioControlCode,
                out TOutput output)
                where TOutput : struct {
            var cnt = Marshal.SizeOf<TOutput>();
            var buf = Marshal.AllocHGlobal(cnt);

            try {
                DeviceIoControl(device,
                    ioControlCode,
                    nint.Zero,
                    0,
                    buf,
                    (uint) cnt);
                output = Marshal.PtrToStructure<TOutput>(buf);
            } finally {
                Marshal.FreeHGlobal(buf);
            }
        }
    }
}
