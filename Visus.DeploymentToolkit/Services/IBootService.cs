// <copyright file="IBootService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Management;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Bcd;
using Visus.DeploymentToolkit.SystemInformation;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a service class that allows for modifying the BCD
    /// store, the boot sector, etc.
    /// </summary>
    public interface IBootService {

        /// <summary>
        /// Cleans the boot data on the specified drive assuming the specified
        /// version of NTLDR and the specified type of firmware.
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="version">The version, which must be either
        /// &quot;nt52&quot; or &quot;nt60&quot;.</param>
        /// <param name="firmware"></param>
        /// <returns></returns>
        Task CleanAsync(string drive, string version, FirmwareType firmware);

        /// <summary>
        /// Creates a new BCD store on the system parition using bcdboot.exe.
        /// </summary>
        /// <param name="windowsPath">The path of the the Windows installation.
        /// </param>
        /// <param name="bootDrive">The name of the boot drive. If not
        /// specified, the system partition will be used.</param>
        /// <param name="firmware">The firmware for which to install the boot
        /// setctor.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task CreateBcdStore(string windowsPath, string? bootDrive,
            FirmwareType firmware);

        /// <summary>
        /// Creates an boot sector on the specified drive for the specified
        /// version of NTLDR using the bootsect tool.
        /// </summary>
        /// <param name="drive">The drive where the boot sector is to be
        /// installed. This must be the root folder of the drive.</param>
        /// <param name="version">The version, which must be either
        /// &quot;nt52&quot; or &quot;nt60&quot;.</param>
        /// <param name="firmware">The firmware for which to install the boot
        /// setctor.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task CreateBootsector(string drive, string version,
            FirmwareType firmware);

    }
}
