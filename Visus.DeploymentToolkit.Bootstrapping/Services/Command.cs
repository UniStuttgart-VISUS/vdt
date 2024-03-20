// <copyright file="Command.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the <see cref="ICommand" /> interfaces based on
    /// <see cref="ProcessStar"/>
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
        public string Path => this._processStartInfo.FileName;

        /// <inheritdoc />
        public string WorkingDirectory
            => this._processStartInfo.WorkingDirectory;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task<int?> ExecuteAsync() {
            using (var p = Process.Start(this._processStartInfo)) {
                if (p != null) {
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
            var sb = new StringBuilder(this.Path);

            foreach (var a in this.Arguments) {
                sb.Append('"').Append(a).Append('"');
            }

            return sb.ToString();
        }
        #endregion

        #region Private fields
        private readonly ProcessStartInfo _processStartInfo;
        #endregion
    }
}
