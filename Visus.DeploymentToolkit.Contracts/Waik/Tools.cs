// <copyright file="Tools.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.IO;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Waik {

    /// <summary>
    /// Provides methods for resolving the expected locations of specific tools
    /// in the WAIK installation folder.
    /// </summary>
    public static class Tools {

        /// <summary>
        /// Gets the architecture string used in the WinPE paths, which is
        /// derived from <see cref="Architecture"/>.
        /// </summary>
        /// <param name="architecture">The architecture to get the folder name
        /// for.</param>
        /// <returns>The name of teh subfolder for the specified architecture.
        /// </returns>
        /// <exception cref="ArgumentException">The architecture is not
        /// supported by the WAIK.</exception>
        public static string GetArchitecturePath(Architecture architecture)
            => architecture switch {
                Architecture.X64 => "amd64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => throw new ArgumentException(string.Format(
                    Errors.UnsupportedWaikArchitecture, architecture))
            };

        /// <summary>
        /// Gets the path to the bcdboot tool for restoring the BCD store.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of bcdboot.exe in the given WAIK installation.
        /// </returns>
        public static string GetBcdbootPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                GetArchitecturePath(architecture),
                "BCDBoot",
                "bcdboot.exe");

        /// <summary>
        /// Gets the path to the bcdedit tool for editing the BCD store.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of bcdedit.exe in the given WAIK installation.
        /// </returns>
        public static string GetBcdedit(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                GetArchitecturePath(architecture),
                "BCDBoot",
                "bcdedit.exe");

        /// <summary>
        /// Gets the path to the bootsect tool.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of bootsect.exe in the given WAIK installation.
        /// </returns>
        public static string GetBootsect(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                GetArchitecturePath(architecture),
                "BCDBoot",
                "bootsect.exe");

        /// <summary>
        /// Gets the path where the deployment tools are installed, which
        /// includes tools for modifying the boot sector and creating
        /// images.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <returns>The path where the deployment tools are located.</returns>
        public static string GetDeploymentToolsPath(string basePath)
            => Path.Combine(basePath, "Deployment Tools");

        /// <summary>
        /// Gets the path to the DISM tool.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of dism.exe in the given WAIK installation.
        /// </returns>
        public static string GetDismPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                GetArchitecturePath(architecture),
                "DISM",
                "dism.exe");

        /// <summary>
        /// Gets the path to the oscdimg tool for creating ISO images.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of oscdimg.exe in the given WAIK installation.
        /// </returns>
        public static string GetOscdimgPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                GetArchitecturePath(architecture),
                "Oscdimg",
                "oscdimg.exe");

        /// <summary>
        /// Gets the path to the Windows setup files.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of the setup files.</returns>
        public static string GetSetupPath(string basePath,
                Architecture architecture)
            => Path.Combine(basePath,
                "Windows Setup",
                GetArchitecturePath(architecture));

        /// <summary>
        /// Gets the base path where the WinPE extension to WAIK is installed.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <returns>The path where the WinPE templates are located.</returns>
        public static string GetWinPePath(string basePath)
            => Path.Combine(basePath, "Windows Preinstallation Environment");

    }
}
