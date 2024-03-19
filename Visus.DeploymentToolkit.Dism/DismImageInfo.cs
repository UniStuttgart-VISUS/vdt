// <copyright file="DismImageInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes the metadata of an image.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismImageInfo {

        /// <summary>
        /// A <see cref="DismImageType"/> enumeration value such as
        /// <see cref="DismImageType.Wim"/>.
        /// </summary>
        public DismImageType ImageType;

        /// <summary>
        /// The index number, starting at 1, of the image.
        /// </summary>
        public uint ImageIndex;

        /// <summary>
        /// The name of the image.
        /// </summary>
        public string ImageName;

        /// <summary>
        /// A description of the image.
        /// </summary>
        public string ImageDescription;

        /// <summary>
        /// The size of the image in bytes.
        /// </summary>
        public ulong ImageSize;

        /// <summary>
        /// The architecture of the image.
        /// </summary>
        public uint Architecture;

        /// <summary>
        /// The name of the product, for example, &quot;Microsoft Windows
        /// Operating System&quot;.
        /// </summary>
        public string ProductName;

        /// <summary>
        /// The edition of the product, for example, &quot;Ultimate&quot;.
        /// </summary>
        public string EditionID;

        /// <summary>
        /// A string identifying whether the installation is a
        /// &quot;Client&quot; or &quot;Server&quot; type.
        /// </summary>
        public string InstallationType;

        /// <summary>
        /// The hardware abstraction layer (HAL) type of the operating system.
        /// </summary>
        public string Hal;

        /// <summary>
        /// The product type, for example, &quot;WinNT&quot;.
        /// </summary>
        public string ProductType;

        /// <summary>
        /// The product suite, for example, &quot;Terminal Server &quot;.
        /// </summary>
        public string ProductSuite;

        /// <summary>
        /// The major version of the operating system.
        /// </summary>
        public uint MajorVersion;

        /// <summary>
        /// The minor version of the operating system.
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// The build number, for example, &quot;10240&quot;.
        /// </summary>
        public uint Build;

        /// <summary>
        /// The service pack build.
        /// </summary>
        public uint ServicePackBuild;

        /// <summary>
        /// The service pack number.
        /// </summary>
        public uint ServicePackLevel;

        /// <summary>
        /// A <see cref="DismImageBootable"/> enumeration value such as
        /// <see cref="DismImageBootable.Yes"/>.
        /// </summary>
        public DismImageBootable Bootable;

        /// <summary>
        /// The Windows directory.
        /// </summary>
        public string SystemRoot;

        /// <summary>
        /// An array of <see cref="DismLanguage"/> structures representing the
        /// languages in the image.
        /// </summary>
        public IntPtr Language;

        /// <summary>
        /// The number of elements in the <see cref="Language"/> array.
        /// </summary>
        public uint LanguageCount;

        /// <summary>
        /// The index number of the default language.
        /// </summary>
        public uint DefaultLanguageIndex;

        /// <summary>
        /// The customised information for the image file. A
        /// <see cref="DismWimCustomisedInfo "/> structure for a WIM file.
        /// <c>null</c> for a VHD image.
        /// </summary>
        public IntPtr CustomisedInfo;
    }
}
