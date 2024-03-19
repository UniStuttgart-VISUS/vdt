// <copyright file="DismDriverPackage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes the architecture and hardware that the driver supports.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismDriverPackage {

        /// <summary>
        /// The published driver name.
        /// </summary>
        public string PublishedName;

        /// <summary>
        /// The original file name of the driver.
        /// </summary>
        public string OriginalFileName;

        /// <summary>
        /// <c>true</c> if the driver is included on the Windows distribution
        /// media and automatically installed as part of Windows, otherwise
        /// <c>false</c>.
        /// </summary>
        public bool InBox;

        /// <summary>
        /// The catalog file for the driver.
        /// </summary>
        public string CatalogFile;

        /// <summary>
        /// The class name of the driver.
        /// </summary>
        public string ClassName;

        /// <summary>
        /// The class GUID of the driver.
        /// </summary>
        public string ClassGuid;

        /// <summary>
        /// The class description of the driver.
        /// </summary>
        public string ClassDescription;

        /// <summary>
        /// <c>true</c> if the driver is boot-critical, otherwise <c>false</c>.
        /// </summary>
        public bool BootCritical;

        /// <summary>
        /// A value from the <see cref="DismDriverSignature"> enumeration that
        /// indicates the driver signature status.
        /// </summary>
        public DismDriverSignature DriverSignature;

        /// <summary>
        /// The provider of the driver.
        /// </summary>
        public string ProviderName;

        /// <summary>
        /// The manufacturer's build date of the driver.
        /// </summary>
        public SystemTime Date;

        /// <summary>
        /// The major version number of the driver.
        /// </summary>
        public uint MajorVersion;

        /// <summary>
        /// The minor version number of the driver.
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// The build number of the driver.
        /// </summary>
        public uint Build;

        /// <summary>
        /// The revision number of the driver.
        /// </summary>
        public uint Revision;
    }
}
