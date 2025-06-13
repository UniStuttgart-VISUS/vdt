// <copyright file="LockScreen.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task setting a value in the registry.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class LockScreen : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for the task.</param>
        public LockScreen(IState state,
                ILogger<RegistryValue> logger)
                : base(state, logger) {
            this.IsCritical = false;
            this.Name = Resources.LockScreen;
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            this._logger.LogInformation("Locking the workstation.");
            if (!LockWorkStation()) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Private methods
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();
        #endregion
    }
}
