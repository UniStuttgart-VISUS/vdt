// <copyright file="ToolsOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Configures the location of tools like ones in the Windows Assessment and
    /// Deployment Kit (WAIK).
    /// </summary>
    /// <remarks>
    /// The paths are from C:\Program Files\Microsoft Deployment Toolkit\Bin\DeploymentTools.xml.
    /// </remarks>
    public class ToolsOptions {

        #region Public constants
        /// <summary>
        /// The suggested name of the configuration section mapping to this
        /// class.
        /// </summary>
        public const string Section = "Tools";
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the bcdboot tool for restoring the BCD
        /// store.
        /// </summary>
        public string BcdbootPath {
            get {
                if (this._bcdbootPath is null) {
                    this._bcdbootPath = Path.Combine(
                        this.DeploymentToolsPath,
                        "BCDBoot",
                        "bcdboot.exe");
                }
                return this._bcdbootPath;
            }
            set => this._bcdbootPath = value;
        }

        /// <summary>
        /// Gets or sets the path to the bcdedit tool for editing the BCD store.
        /// </summary>
        public string BcdeditPath {
            get {
                if (this._bcdeditPath is null) {
                    this._bcdeditPath = Path.Combine(
                        this.DeploymentToolsPath,
                        "BCDBoot",
                        "bcdedit.exe");
                }
                return this._bcdeditPath;
            }
            set => this._bcdeditPath = value;
        }

        /// <summary>
        /// Gets or sets the path to the bootsect tool.
        /// </summary>
        public string BootsectPath {
            get {
                if (this._bootsectPath is null) {
                    this._bootsectPath = Path.Combine(
                        this.DeploymentToolsPath,
                        "BCDBoot",
                        "bootsect.exe");
                }
                return this._bootsectPath;
            }
            set => this._bootsectPath = value;
        }

        /// <summary>
        /// Gets or sets the path where the deployment tools are installed,
        /// which includes tools for modifying the boot sector and creating
        /// images.
        /// </summary>
        public string DeploymentToolsPath {
            get {
                if (this._deploymentToolsPath is null) {
                    this._deploymentToolsPath = Path.Combine(
                        this.WaikPath,
                        "Deployment Tools",
                        "%WaikArchitectures%");
                }
                return this._deploymentToolsPath;
            }
            set => this._deploymentToolsPath = value;
        }

        /// <summary>
        /// Gets or sets the path to the dism tool.
        /// </summary>
        public string DismPath {
            get {
                if (this._dismPath is null) {
                    this._dismPath = Path.Combine(
                        this.DeploymentToolsPath,
                        "DISM",
                        "dism.exe");
                }
                return this._dismPath;
            }
            set => this._dismPath = value;
        }

        /// <summary>
        /// Gets or sets the path to the loadstate tool for user state
        /// migration.
        /// </summary>
        public string LoadStatePath {
            get {
                if (this._loadStatePath is null) {
                    this._loadStatePath = Path.Combine(
                        this.WaikPath,
                        "User State Migration Tool",
                        "%WaikArchitectures%",
                        "loadstate.exe");
                }
                return this._loadStatePath;
            }
            set => this._loadStatePath = value;
        }

        /// <summary>
        /// Gets or sets the path to the oscdimg tool for creating ISO images.
        /// </summary>
        public string OscdimgPath {
            get {
                if (this._oscdimgPath is null) {
                    this._oscdimgPath = Path.Combine(
                        this.DeploymentToolsPath,
                        "Oscdimg",
                        "oscdimg.exe");
                }
                return this._oscdimgPath;
            }
            set => this._oscdimgPath = value;
        }

        /// <summary>
        /// Gets or sets the path to the scanstate tool for user state
        /// migration.
        /// </summary>
        public string ScanStatePath {
            get {
                if (this._scanStatePath is null) {
                    this._scanStatePath = Path.Combine(
                        this.WaikPath,
                        "User State Migration Tool",
                        "%WaikArchitectures%",
                        "scanstate.exe");
                }
                return this._scanStatePath;
            }
            set => this._scanStatePath = value;
        }

        /// <summary>
        /// Gets or sets the path to the Windows setup files.
        /// </summary>
        public string SetupPath {
            get {
                if (this._setupPath is null) {
                    this._setupPath = Path.Combine(
                        this.WaikPath,
                        "Windows Setup",
                        "%WaikArchitectures%");
                }
                return this._setupPath;
            }
            set => this._setupPath = value;
        }

        /// <summary>
        /// Gets or sets the architecture-dependent paths used by the WAIK.
        /// </summary>
        public IDictionary<Architecture, string> WaikArchitectures {
            get;
            set;
        } = new Dictionary<Architecture, string> {
            { Architecture.Arm, "arm" },
            { Architecture.Arm64, "arm64" },
            { Architecture.X64, "amd64" },
            { Architecture.X86, "x86" }
        };

        /// <summary>
        /// Gets or sets the installation path of the Windows Assessment and
        /// Deployment Kit (WAIK).
        /// </summary>
        /// <remarks>This path is architecture-independent.</remarks>
        public string WaikPath {
            get {
                if (this._waikPath is null) {
                    this._waikPath = Path.Combine(
                        ProgrammeFiles,
                        "Windows Kits",
                        "10",
                        "Assessment and Deployment Kit");
                }
                return this._waikPath;
            }
            set => this._waikPath = value;
        }

        /// <summary>
        ///  Gets or sets the path to the Windows PE extension to WAIK.
        /// </summary>
        /// <remarks>This path is architecture-independent.</remarks>
        public string WinPePath {
            get {
                if (this._winPePath is null) {
                    this._winPePath = Path.Combine(
                        this.WaikPath,
                        "Windows Preinstallation Environment");
                }
                return this._winPePath;
            }
            set => this._winPePath = value;
        }

        /// <summary>
        /// Gets or sets the path where optional components for the Windows
        /// preinstallation environment are located.
        /// </summary>
        public string WinPeOptionalComponentsPath {
            get {
                if (this._winPeOptionalComponentsPath is null) {
                    this._winPeOptionalComponentsPath = Path.Combine(
                        this.WinPePath,
                        "%WaikArchitectures%",
                        "WinPE_OCs");
                }
                return this._winPeOptionalComponentsPath;
            }
            set => this._winPeOptionalComponentsPath = value;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Replaces all architecture placeholders in the given
        /// <paramref name="path"/> by the
        /// <paramref name="architecture"/>-specific path. The names of all
        /// properties that are dictionaries from <see cref="Architecture"/> to
        /// strings are valid placeholder names. All placeholders are to be
        /// enclosed in percent signs.
        /// </summary>
        /// <param name="path">The path in which the placeholders are to be
        /// replaced.</param>
        /// <param name="architecture">The architecture to look for.</param>
        /// <returns>The p<paramref name="path"/> with all placeholders replaced
        /// by the respective value from the dictionary.s</returns>
        public string EvaluateArchitecture(string path,
                Architecture architecture) {
            ArgumentNullException.ThrowIfNull(path);
            var luts = this.GetArchLuts<IDictionary<Architecture, string>>();

            foreach (var (placeholder, lut) in luts) {
                if (lut.TryGetValue(architecture, out var value)) {
                    path = path.Replace(placeholder, value);
                }
            }

            return path;
        }
        #endregion

        #region Private properties
        /// <summary>
        /// The folder where we expect the WAIK to be installed, which currently
        /// us the 32-bit programme files folder.
        /// </summary>
        private static string ProgrammeFiles => Environment.GetFolderPath(
            Environment.SpecialFolder.ProgramFilesX86);
        #endregion

        #region Private methods
        private IEnumerable<(string, TLut)> GetArchLuts<TLut>()
                where TLut : class, IDictionary<Architecture, string> {
            return from p in this.GetType().GetProperties()
                   where p.PropertyType == typeof(TLut)
                   let l = p.GetValue(this) as TLut
                   where l is not null
                   select ($"%{p.Name}%", l);
        }
        #endregion

        #region Private fields
        private string? _bcdbootPath;
        private string? _bcdeditPath;
        private string? _bootsectPath;
        private string? _deploymentToolsPath;
        private string? _dismPath;
        private string? _loadStatePath;
        private string? _oscdimgPath;
        private string? _scanStatePath;
        private string? _setupPath;
        private string? _waikPath;
        private string? _winPePath;
        private string? _winPeOptionalComponentsPath;
        #endregion
    }
}
