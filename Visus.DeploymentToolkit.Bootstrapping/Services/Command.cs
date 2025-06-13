// <copyright file="Command.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the <see cref="ICommand" /> interfaces based on
    /// <see cref="ProcessStartInfo"/>.
    /// </summary>
    internal class Command : ICommand {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="processStartInfo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Command(ProcessStartInfo processStartInfo) {
            this._processStartInfo = processStartInfo
                ?? throw new ArgumentNullException(nameof(processStartInfo));
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public IEnumerable<string> Arguments
            => this._processStartInfo.ArgumentList;

        /// <inheritdoc />
        public bool DoNotWait { get; internal set; }

        /// <inheritdoc />
        public string Path => this._processStartInfo.FileName;

        /// <inheritdoc />
        public string WorkingDirectory
            => this._processStartInfo.WorkingDirectory;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task<int?> ExecuteAsync() {
            using (var p = Process.Start(this._processStartInfo)) {
                if ((p != null) && !this.DoNotWait) {
                    await p.WaitForExitAsync();
                    return p.ExitCode;
                } else {
                    return null;
                }
            }
            //p.ErrorDataReceived += (sender, e) => { }
            //p.OutputDataReceived += (sender, e) => { }
        }

        /// <inheritdoc />
        public override string ToString() {
            var sb = new StringBuilder(this._processStartInfo.FileName);

            if (this._processStartInfo.ArgumentList?.Any() == true) {
                foreach (var a in this._processStartInfo.ArgumentList) {
                    sb.Append(' ').Append(a);
                }

            } else if (!string.IsNullOrEmpty(this._processStartInfo.Arguments)) {
                sb.Append(' ').Append(this._processStartInfo.Arguments);
            }

            return sb.ToString();
        }
        #endregion

        #region Private fields
        private readonly ProcessStartInfo _processStartInfo;
        #endregion
    }
}
