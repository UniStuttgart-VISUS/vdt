// <copyright file="ISystemInformation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using Visus.DeploymentToolkit.SystemInformation;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service for retrieving information about the system we are running.
    /// </summary>
    public interface ISystemInformation {

        /// <summary>
        /// Gets an optional asset tag that might be set via SMBIOS.
        /// </summary>
        string? AssetTag { get; }

        /// <summary>
        /// Gets the chassis or form factor of the system.
        /// </summary>
        IEnumerable<ChassisType> Chassis { get; }

        /// <summary>
        /// Gets the <see cref="PortableExecutableKinds"/> of the currently
        /// executing CLR.
        /// </summary>
        PortableExecutableKinds ClrExecutableKind { get; }

        /// <summary>
        /// Gets the <see cref="ImageFileMachine"/> of the currently executing
        /// CLR.
        /// </summary>
        ImageFileMachine ClrMachine { get; }

        /// <summary>
        /// Gets the HAL.
        /// </summary>
        string? Hal { get; }

        /// <summary>
        /// Gets the host name of the system.
        /// </summary>
        string HostName { get; }

        /// <summary>
        /// Gets all IP addresses assigned to any network adapter of the system.
        /// </summary>
        IEnumerable<IPAddress> IPAddresses { get; }

        /// <summary>
        /// Gets whether the Windows version is a preinstalled environment.
        /// </summary>
        bool IsWinPE { get; }

        /// <summary>
        /// Gets whether the Windows version is a server SKU.
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        /// Gets whether the Windows version is a UI-less server core
        /// installation.
        /// </summary>
        bool IsServerCore { get; }

        /// <summary>
        /// Gets the manufacturer of the computer as reported by the SMBIOS.
        /// </summary>
        string? Manufacturer { get; }

        /// <summary>
        /// Gets the model of the computer as reported by the SMBIOS.
        /// </summary>
        string? Model { get; }

        /// <summary>
        /// Gets the OS platform.
        /// </summary>
        PlatformID OperatingSystemPlatform { get; }

        /// <summary>
        /// Gets the version of the operating system we are running on.
        /// </summary>
        Version OperatingSystemVersion { get; }

        /// <summary>
        /// Gets the MAC addresses of the system.
        /// </summary>
        IEnumerable<PhysicalAddress> PhysicalAddresses { get; }

        /// <summary>
        /// Gets the system's serial number as reported by the BIOS.
        /// </summary>
        string? SerialNumber { get; }

        /// <summary>
        /// Gets the UUID of the system, if any.
        /// </summary>
        Guid? Uuid { get; }
    }
}
