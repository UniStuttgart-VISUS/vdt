// <copyright file="Functions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Provides the entry points of the DISM API.
    /// </summary>
    internal static class Native {

        /// <summary>
        /// The name of the DISM DLL.
        /// </summary>
        public const string LibraryName = "DismAPI.dll";

        /// <summary>
        /// Adds a capability to an image.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="name"></param>
        /// <param name="limitAccess"></param>
        /// <param name="sourcePaths"></param>
        /// <param name="sourcePathCount"></param>
        /// <param name="cancelEvent"></param>
        /// <param name="progress"></param>
        /// <param name="userData"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismAddCapability(
            uint session,
            string name,
            bool limitAccess,
            string[] sourcePaths,
            uint sourcePathCount,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        /// <summary>
        /// Adds a third party driver (.inf) to an offline Windows image.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="driverPath"></param>
        /// <param name="forceUnsigned"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismAddDriver(
            uint session,
            string driverPath,
            bool forceUnsigned);

        /// <summary>
        /// Adds a single .cab or .msu file to a Windows image.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packagePath"></param>
        /// <param name="ignoreCheck"></param>
        /// <param name="preventPending"></param>
        /// <param name="cancelEvent"></param>
        /// <param name="progress"></param>
        /// <param name="userData"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismAddPackage(
            uint session,
            string packagePath,
            bool ignoreCheck,
            bool preventPending,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        /// <summary>
        /// Applies an unattended answer file to a Windows image.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="unattendFile"></param>
        /// <param name="singleSession"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismApplyUnattend(
            uint session,
            string unattendFile,
            bool singleSession);

        /// <summary>
        /// Checks whether the image can be serviced or is corrupted.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="scanImage"></param>
        /// <param name="cancelEvent"></param>
        /// <param name="progress"></param>
        /// <param name="userData"></param>
        /// <param name="imageHealth"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismCheckImageHealth(
            uint session,
            bool scanImage,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData,
            out DismImageHealthState imageHealth);

        /// <summary>
        /// Removes files and releases resources associated with corrupted or
        /// invalid mount paths.
        /// </summary>
        /// <param name=""></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismCleanupMountPoints();

        /// <summary>
        /// Shuts down the given DISM session.
        /// </summary>
        /// <param name="session"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismCloseSession(uint session);

        /// <summary>
        /// Commits the changes made to a Windows image in a mounted .wim or
        /// .vhd file. The image must be mounted using the 
        /// <see cref="DismMountImage"/>.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="flags"></param>
        /// <param name="cancelEvent"></param>
        /// <param name="progress"></param>
        /// <param name="userData"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismCommitImage(
            uint session,
            uint flags,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        /// <summary>
        /// Releases resources held by a structure or an array of structures
        /// returned by other DISM API Functions.
        /// </summary>
        /// <param name="dismStructure"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismDelete(IntPtr dismStructure);

        /// <summary>
        /// Disables a feature in the current image.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="featureName"></param>
        /// <param name="packageName"></param>
        /// <param name="removePayload"></param>
        /// <param name="cancelEvent"></param>
        /// <param name="progress"></param>
        /// <param name="userData"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismDisableFeature(
            uint session,
            string featureName,
            string? packageName,
            bool removePayload,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        /// <summary>
        /// Enables a feature in an image. Features are identified by a name and
        /// can optionally be tied to a package.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="featureName"></param>
        /// <param name="dentifier"></param>
        /// <param name="packageIdentifier"></param>
        /// <param name="limitAccess"></param>
        /// <param name="sourcePaths"></param>
        /// <param name="sourcePathCount"></param>
        /// <param name="enableAll"></param>
        /// <param name="cancelEvent"></param>
        /// <param name="progress"></param>
        /// <param name="userData"></param>
        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismEnableFeature(
            uint session,
            string featureName,
            string? dentifier,
            DismPackageIdentifier packageIdentifier,
            bool limitAccess,
            string[] sourcePaths,
            uint sourcePathCount,
            bool enableAll,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetCapabilities(
            uint session,
            IntPtr capability,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetCapabilityInfo(
            uint session,
            string name,
            out IntPtr info); // _In_ DismCapabilityInfo** Info

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetDriverInfo(
            uint session,
            string driverPath,
            out IntPtr driver,          //_Out_ DismDriver        **Driver,
            out uint count,
            out IntPtr driverPackage);  //_Out_opt_ DismDriverPackage **DriverPackage

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetDrivers(
            uint session,
            bool allDrivers,
            out IntPtr driverPackage,   //_Out_ DismDriverPackage **DriverPackage,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetFeatureInfo(
            uint session,
            string featureName,
            string? identifier,
            DismPackageIdentifier packageIdentifier,
            out IntPtr featureInfo); //_Out_ DismFeatureInfo       **FeatureInfo

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetFeatureParent(
            uint session,
            string featureName,
            string? dentifier,
            DismPackageIdentifier packageIdentifier,
            out IntPtr dismFeature, //_Out_ DismFeature           **Feature,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetFeatures(
            uint session,
            string identifier,
            DismPackageIdentifier packageIdentifier,
            out IntPtr dismFeature, //_Out_ DismFeature           **Feature,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetLastErrorMessage(
            out IntPtr errorMessage); //  _Out_ DismString **ErrorMessage

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetImageInfo(
            string imageFilePath,
            out IntPtr imageInfo, ///_Out_ DismImageInfo **ImageInfo,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetMountedImageInfo(
            out IntPtr mountedImageInfo, //_Out_ DismMountedImageInfo **MountedImageInfo,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetPackageInfo(
            uint session,
            string identifier,
            DismPackageIdentifier packageIdentifier,
            out IntPtr packageInfo); // _Out_ DismPackageInfo       **PackageInfo

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetPackages(
            uint session,
            out IntPtr package,    //DismPackage **Package,
            out uint count);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismGetReservedStorageState(
            uint session,
            out uint state);

        [DllImport(LibraryName,
            EntryPoint = "DismInitialize",
            CharSet = CharSet.Unicode,
            PreserveSig = false)]
        public static extern void DismInitialise(
            DismLogLevel logLevel,
            string? logFilePath,
            string? scratchDirectory);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismMountImage(
            string imageFilePath,
            string mountPath,
            uint imageIndex,
            string? imageName,
            DismImageIdentifier imageIdentifier,
            uint flags,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismOpenSession(
            string imagePath,
            string? windowsDirectory,
            string? systemDrive,
            out uint session);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismRemountImage(
            string mountPath);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismRemoveCapability(
            uint session,
            string name,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismRemoveDriver(
            uint session,
            string driverPath);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismRemovePackage(
            uint session,
            string identifier,
            DismPackageIdentifier packageIdentifier,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismRestoreImageHealth(
            uint session,
            string[]? sourcePaths,
            uint sourcePathCount,
            bool limitAccess,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismSetReservedStorageState(
            uint session,
            uint state);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismShutdown();

        [DllImport(LibraryName, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DismUnmountImage(
            string mountPath,
            uint flags,
            SafeWaitHandle cancelEvent,
            DismProgressCallback? progress,
            IntPtr userData);
    }
}
