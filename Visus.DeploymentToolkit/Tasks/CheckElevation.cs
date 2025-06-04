// <copyright file="RequireElevation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task checks whether it is running as administrator or fails.
    /// </summary>
    public sealed class CheckElevation : TaskBase{

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        public CheckElevation(IState state, ILogger<CheckElevation> logger)
                : base(state, logger) {
            this.IsCritical = true;
            this.Name = Resources.CheckElevation;
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Checking whether the process is "
                + "running with administrative privileges.");

            if (OperatingSystem.IsWindows()) {
                this.CheckWindowsRole();

            } else if (OperatingSystem.IsLinux()) {
                this.CheckEffectiveUid();

            } else {
                this._logger.LogError("The current operating system is not "
                    + "supported by this task.");
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Private methods
        [SupportedOSPlatform("linux")]
        [DllImport("libc")]
        private static extern uint geteuid();

        [SupportedOSPlatform("windows")]
        private void CheckWindowsRole() {
            using var identity = WindowsIdentity.GetCurrent();
            this._logger.LogTrace("Running as user {User} with SID {Sid}.",
                identity.Name, identity.User);

            var principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator)) {
                this._logger.LogError("The user {User} is not an "
                    + "administrator.", identity.Name);
                throw new UnauthorizedAccessException(Errors.NotAdministrator);
            }

            this._logger.LogInformation("The process is running as {User} "
                + "with administrative privileges.", identity.Name);
        }

        [SupportedOSPlatform("linux")]
        private void CheckEffectiveUid() {
            var uid = geteuid();
            this._logger.LogError("Running under the effective user ID {User}.",
                uid);

            if (uid != 0) {
                this._logger.LogError("The effective user ID is not root.");
                throw new UnauthorizedAccessException(Errors.NotAdministrator);
            }

            this._logger.LogInformation("The process is running as root.");
        }
        #endregion
    }
}
