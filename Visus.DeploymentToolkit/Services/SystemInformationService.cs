// <copyright file="SystemInformationService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.SystemInformation;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of <see cref="ISystemInformation"/>.
    /// </summary>
    internal sealed class SystemInformationService : ISystemInformation {

        #region Public constructors
        public SystemInformationService(IRegistry registry,
                IManagementService wmi,
                ILogger<SystemInformationService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._wmi = wmi ?? throw new ArgumentNullException(nameof(wmi));

            _ = registry ?? throw new ArgumentNullException(nameof(registry));

            this._bios = new(() => this.GetWmi("Win32_BIOS"));
            {
                typeof(object).Module.GetPEKind(out var kind, out var machine);
                this.ClrExecutableKind = kind;
                this.ClrMachine = machine;
            }
            this._computer = new(() => this.GetWmi("Win32_ComputerSystem"));
            this._computerProduct = new(() => this.GetWmi("Win32_ComputerSystemProduct"));
            this._enclosure = new(() => this.GetWmi("Win32_SystemEnclosure"));
            this._hal = new(() => this.GetHal(this._wmi, registry));
            this.IsWinPE = this.GetIsWinPE(registry);
            this.IsServer = this.GetIsServer(registry);
            this.IsServerCore = !this.IsWinPE && !this.HasWindowsExplorer();
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public string? AssetTag => (this._enclosure.Value?["SMBIOSAssetTag"]
            as string)?.Trim();

        /// <inheritdoc />
        public IEnumerable<ChassisType> Chassis {
            get {
                return (this._enclosure.Value?["ChassisTypes"] as IEnumerable)
                    ?.Cast<ChassisType>()
                    ?? Enumerable.Empty<ChassisType>();
            }
        }

        /// <inheritdoc />
        public PortableExecutableKinds ClrExecutableKind { get; }

        /// <inheritdoc />
        public ImageFileMachine ClrMachine { get; }

        /// <inheritdoc />
        public string? Hal => this._hal.Value;

        /// <inheritdoc />
        public string HostName => Environment.MachineName;

        /// <inheritdoc />
        public IEnumerable<IPAddress> IPAddresses {
            get {
                foreach (var i in NetworkInterface.GetAllNetworkInterfaces()) {
                    foreach (var a in i.GetIPProperties().UnicastAddresses) {
                        yield return a.Address;
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool IsWinPE { get; }

        /// <inheritdoc />
        public bool IsServer { get; }

        /// <inheritdoc />
        public bool IsServerCore { get; }

        /// <inheritdoc />
        public string? Manufacturer
            => this._computer.Value?["Manufacturer"] as string;

        /// <inheritdoc />
        public string? Model
            => this._computer.Value?["Model"] as string;

        /// <inheritdoc />
        public PlatformID OperatingSystemPlatform
            => Environment.OSVersion.Platform;

        /// <inheritdoc />
        public Version OperatingSystemVersion => Environment.OSVersion.Version;

        /// <inheritdoc />
        public IEnumerable<PhysicalAddress> PhysicalAddresses {
            get {
                foreach (var i in NetworkInterface.GetAllNetworkInterfaces()) {
                    var retval = i.GetPhysicalAddress();
                    if ((retval != null) && retval.GetAddressBytes().Any()) {
                        yield return retval;
                    }
                }
            }
        }

        /// <inheritdoc />
        public string? SerialNumber
            => this._bios.Value?["SerialNumber"] as string;

        /// <inheritdoc />
        public Guid? Uuid {
            get {
                var uuid = this._computerProduct.Value?["UUID"] as string;
                return (uuid != null) ? new Guid(uuid) : null;
            }
        }
        #endregion

        #region Private methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetHal(IManagementService wmi, IRegistry registry) {
            Debug.Assert(wmi != null);
            Debug.Assert(registry != null);
            string? retval = null;

            var query = "SELECT DeviceID "
                + "FROM Win32_PnPEntity "
                + @"WHERE ClassGUID=""{4D36E966-E325-11CE-BFC1-08002BE10318}"" "
                + @"OR DeviceID LIKE ""ROOT\\%HAL%""";

            var device = wmi.Query(query).FirstOrDefault();

            if (device != null) {
                var key = @"HKLM\SYSTEM\CurrentControlSet\Enum\"
                    + device["DeviceID"];
                var hwID = registry.GetValue(key, "HardwareID");
                if (hwID is IEnumerable<object> e) {
                    retval = e.FirstOrDefault() as string;
                } else {
                    retval = hwID as string;
                }
            }

            // Like MDT, try the registry if we did not get the HAL via WMI.
            if (retval == null) {
                for (int i = 0; i < 9999; ++i) {
                    var key = @"HKLM\System\CurrentControlSet\Control\Class\"
                        + @"{4D36E966-E325-11CE-BFC1-08002BE10318}\"
                        + i.ToString("D4");
                    var hwID = registry.GetValue(key, "MatchingDeviceID");
                    if ((hwID != null) && (hwID is string s)) {
                        retval = s;
                        break;
                    }
                }
            }

            return retval ?? throw new NotFoundException(Errors.HalNotFound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetIsWinPE(IRegistry registry) {
            Debug.Assert(registry != null);
            var key = @"HKLM\System\CurrentControlSet\Control\MiniNT";
            return registry.KeyExists(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsServer(IRegistry registry) {
            Debug.Assert(registry != null);

            try {
                var productType = registry.GetValue(
                    @"HKLM\SYSTEM\CurrentControlSet\Control\ProductOptions",
                    "ProductType");
                return productType switch {
                    "ServerNT" => true,
                    "LanmanNT" => true,
                    _ => false,
                };
            } catch (Exception ex) {
                this._logger.LogError(ex, Errors.CouldNotGetProductType);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ManagementObject? GetWmi(string @class) {
            Debug.Assert(@class != null);
            return this._wmi.GetInstancesOf(@class).FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasWindowsExplorer() {
            var path = @"%WINDIR%\explorer.exe";
            path = Environment.ExpandEnvironmentVariables(path);
            return File.Exists(path);
        }
        #endregion

        #region Private fields
        private readonly Lazy<ManagementObject?> _bios;
        private readonly Lazy<ManagementObject?> _computer;
        private readonly Lazy<ManagementObject?> _computerProduct;
        private readonly Lazy<ManagementObject?> _enclosure;
        private readonly Lazy<string> _hal;
        private readonly ILogger _logger;
        private readonly IManagementService _wmi;
        #endregion
    }
}
