// <copyright file="RebootMachine.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Security;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task reboots the machine when executed.
    /// </summary>
    public sealed class RebootMachine : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        public RebootMachine(IState state, ILogger<RebootMachine> logger)
                : base(state, logger) {
            this.Name = Resources.RebootMachine;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets whether applications blocking the reboot are forcefully
        /// closed, which is the default.
        /// </summary>
        public bool ForceAppsClosed { get; set; } = true;

        /// <summary>
        /// Gets or sets a message that is being displayed while it is possible
        /// to cancel the reboot.
        /// </summary>
        public string? Message { get; set; } = Resources.RebootMessage;

        /// <summary>
        /// Gets or sets the reason for the reboot using one of the codes from
        /// https://learn.microsoft.com/en-us/windows/win32/shutdown/system-shutdown-reason-codes.
        /// </summary>
        public uint Reason { get; set; } = 0x00030000 | 0x00000002;

        /// <summary>
        /// Gets or sets the the timeout during which it is possible to cancel
        /// the reboot.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this._logger.LogTrace("Enabling shutdown privilege.");
            Advapi32.AdjustTokenPrivileges("SeShutdownPrivilege", true);

            // TODO: preserve state?

            this._logger.LogInformation("Rebooting the system.");
            if (!InitiateSystemShutdownEx(null,
                    this.Message,
                    (uint) this.Timeout.TotalSeconds,
                    this.ForceAppsClosed,
                    true,
                    this.Reason)) {
                throw new Win32Exception();
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Private class methods
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool InitiateSystemShutdownEx(string? machineName,
            string? message,
            uint timeout,
            bool forceAppsClosed,
            bool rebootAfterShutdown,
            uint reason);
        #endregion
    }
}
