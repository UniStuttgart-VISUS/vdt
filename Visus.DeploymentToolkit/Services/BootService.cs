// <copyright file="BootService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.SystemInformation;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the startup modification service.
    /// </summary>
    /// <remarks>
    /// The BCD-related stuff is shamelessly stolen from
    /// https://github.com/mattifestation/BCD/ - and from Copilot.
    /// </remarks>
    /// <param name="wmi"></param>
    /// <param name="logger"></param>
    internal sealed class BootService(IManagementService wmi,
            ICommandBuilderFactory commands,
            IDirectory directory,
            ILogger<BootService> logger) : IBootService {

        #region Public methods
        /// <inheritdoc />
        public async Task CleanAsync(string drive,
                string version,
                FirmwareType firmware) {
            ArgumentException.ThrowIfNullOrWhiteSpace(drive);
            ArgumentException.ThrowIfNullOrWhiteSpace(version);
            this._logger.LogTrace("Cleaning boot configuration on drive "
                + "{Drive}, assuming version {Version} and firmware "
                + "{Type}.", drive, version, firmware);

            if ("nt52".EqualsIgnoreCase(version)) {
                {
                    var path = Path.Combine(drive, "Boot");
                    this._logger.LogTrace("Deleting {Path}.", path);
                    await this._directory.DeleteAsync(path, true);
                }

                {
                    var path = Path.Combine(drive, "BootMgr");
                    if (File.Exists(path)) {
                        this._logger.LogTrace("Deleting {Path}.", path);
                        File.Delete(path);
                    }
                }

            } else if (firmware == FirmwareType.Bios) {
                var path = Path.Combine(drive, "boot", "bcd");
                if (File.Exists(path)) {
                    this._logger.LogTrace("Deleting {Path}.", path);
                    File.Delete(path);
                }

            } else {
                var path = Path.Combine(drive, "efi", "microsoft", "boot",
                    "bcd");
                if (File.Exists(path)) {
                    this._logger.LogTrace("Deleting {Path}.", path);
                    File.Delete(path);
                }
            }
        }

        /// <inheritdoc />
        public async Task CreateBcdStoreAsync(string windowsPath,
                string? bootDrive,
                FirmwareType firmware) {
            ArgumentException.ThrowIfNullOrWhiteSpace(windowsPath);
            this._logger.LogTrace("Creating BCD store for Windows installation "
                + " {WindowsPath} and boot drive {BootDrive}, "
                + "assuming firmware {Type}.", windowsPath, bootDrive,
                firmware);

            var destination = !string.IsNullOrWhiteSpace(bootDrive)
                ? $@" /s ""{bootDrive}"""
                : string.Empty;

            var cmd = this._commands.Run("bcdboot.exe")
                .WithArguments($@"""{windowsPath}""{destination}")
                .WaitForProcess()
                .Build();

            this._logger.LogTrace("Creating BCD store with {Command}.", cmd);
            await cmd.ExecuteAndCheckAsync(0, this._logger);
        }

        /// <inheritdoc />
        public async Task CreateBootsectorAsync(string drive,
                string version,
                FirmwareType firmware) {
            ArgumentException.ThrowIfNullOrWhiteSpace(drive);
            ArgumentException.ThrowIfNullOrWhiteSpace(version);
            this._logger.LogTrace("Creating bootsector on drive {Drive}, "
                + "assuming version {Version} and firmware {Type}.",
                drive, version, firmware);
            var mbr = firmware switch {
                FirmwareType.Bios => " /mbr",
                _ => string.Empty
            };

            var cmd = this._commands.Run("bootsect.exe")
                .WithArguments($"/{version} {drive}{mbr}")
                .WaitForProcess();

            try {
                this._logger.LogTrace("Creating book sector with {Command}.",
                    cmd);
                await cmd.Build().ExecuteAndCheckAsync(0, this._logger);
            } catch {
                if (string.IsNullOrEmpty(mbr)) {
                    // If we did not specify /mbr, this must be an actual error.
                    throw;

                } else {
                    // Similar to LTIApply.wsf in MDT.
                    this._logger.LogWarning("The command {Command} may not "
                        + "support the /mbr switch. Retrying without it.", cmd);
                    cmd.WithArguments($"/{version} {drive}");
                    await cmd.Build().ExecuteAndCheckAsync(0, this._logger);
                }
            }
        }
        #endregion

        #region Private constants
        /// <summary>
        /// The WMI class representing a BCD store.
        /// </summary>
        private const string BcdStoreClass = "BcdStore";
        #endregion

        #region Private fields
        private readonly ICommandBuilderFactory _commands = commands
            ?? throw new ArgumentNullException(nameof(commands));
        private readonly IDirectory _directory = directory
            ?? throw new ArgumentNullException(nameof(directory));
        private readonly ILogger<BootService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        private readonly IManagementService _wmi = wmi
            ?? throw new ArgumentNullException(nameof(wmi));
        #endregion
    }
}
