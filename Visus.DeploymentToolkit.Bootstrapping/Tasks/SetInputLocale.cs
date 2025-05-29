// <copyright file="SetInputLocale.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task changing the current input locale.
    /// </summary>
    public sealed class SetInputLocale : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public SetInputLocale(IState state,
                ILogger<SetInputLocale> logger)
                : base(state, logger) {
            this.Name = Resources.SetKeyboardLocale;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the input locale to configure.
        /// </summary>
        [FromState(nameof(InputLocale))]
        public string? InputLocale { get; set; }
        #endregion

        #region Public methods
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            if ((this.InputLocale is not null) && OperatingSystem.IsWindows()) {
                this._logger.LogInformation("Changing input locale to "
                    + "\"{InputLocale}\".", this.InputLocale);
                var hkl = LoadKeyboardLayout(this.InputLocale, 0);
                if (hkl == nint.Zero) {
                    var error = Marshal.GetLastWin32Error();
                    this._logger.LogError("Failed to load keyboard layout "
                        + "\"{InputLocale}\" with error {Error}.",
                        this.InputLocale, error);
                    throw new Win32Exception(error);
                }

                try {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!SystemParametersInfo(SPI_SETDEFAULTINPUTLANG,
                            0, ref hkl, 0)) {
                        var error = Marshal.GetLastWin32Error();
                        this._logger.LogError("Failed to set keyboard layout "
                            + "to \"{InputLocale}\" with error {Error}.",
                            this.InputLocale, error);
                        throw new Win32Exception(error);
                    }
                } finally {
                    UnloadKeyboardLayout(hkl);
                }

            } else {
                this._logger.LogInformation("No input locale specified for"
                    + $"an {nameof(SetInputLocale)} task.");
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Private methods
        [SupportedOSPlatform("windows")]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint LoadKeyboardLayout(string klid, uint flags);

        [SupportedOSPlatform("windows")]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint action,
            uint value, ref nint pointer, uint winIni);

        [SupportedOSPlatform("windows")]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnloadKeyboardLayout(nint hkl);
        #endregion

        #region Private constants
        private const uint SPI_SETDEFAULTINPUTLANG = 0x005A;
        #endregion
    }
}
