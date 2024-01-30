// <copyright file="Api.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Provides P/Invoke for DISM API
    /// (https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/dism/dism-api-reference?view=windows-11)
    /// </summary>
    internal static class Api {

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismAddDriver(IntPtr session,
            string driverPath, bool forceUnsigned);

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismApplyUnattend(IntPtr session,
            string nattendFile, bool ingleSession);

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismCloseSession(IntPtr session);

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismInitialize(DismLogLevel logLevel,
          string? logFilePath, string? scratchDirectory);

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismMountImage(string imageFilePath,
            string mountPath, uint imageIndex, string? imageName,
            DiskImageIdentifier imageIdentifier, uint flags, IntPtr cancelEvent,
            DismProgressCallback progress, IntPtr userData);

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismOpenSession(string imagePath,
            string? windowsDirectory, string? systemDrive, out IntPtr session);

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismShutdown();

        [DllImport("DismAPI.dll", PreserveSig = false)]
        internal extern static void DismUnmountImage(string mountPath,
            uint flags, IntPtr cancelEvent, DismProgressCallback progress,
            IntPtr userData);
    }
}
