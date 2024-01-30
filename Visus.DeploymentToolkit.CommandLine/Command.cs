// <copyright file="Synchroniser.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>

using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;


namespace Visus.DeploymentToolkit.CommandLine {

    /// <summary>
    /// A wrapper around a command line process being started.
    /// </summary>
    public sealed class Command {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Command(string path) {
            _ = path ?? throw new ArgumentNullException(nameof(path));
            this._processStartInfo = new ProcessStartInfo(path);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the arguments passed to the command.
        /// </summary>
        public IEnumerable<string> Arguments
            => this._processStartInfo.ArgumentList;

        /// <summary>
        /// Gets the path to the file to be executed.
        /// </summary>
        public string Path => this._processStartInfo.FileName;

        /// <summary>
        /// Gets the working directory to run the command in.
        /// </summary>
        public string WorkingDirectory => this._processStartInfo.WorkingDirectory;
        #endregion

        #region Public methods
        /// <summary>
        /// Causes the command to run as the user of the calling process.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public Command AsCurrentUser() {
            this._processStartInfo.UserName = null;
            this._processStartInfo.Password = null;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="NetworkCredential"/> of the user who is running
        /// the command.
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public Command AsUser(NetworkCredential? credential) {
            this._processStartInfo.UserName = credential?.UserName;
            this._processStartInfo.Password = credential?.SecurePassword;
            return this;
        }

        public async Task ExecuteAsync() {
            using (var p = Process.Start(this._processStartInfo)) {

                if (p != null) {
                    await p.WaitForExitAsync();
                }
            }
            //p.ErrorDataReceived += (sender, e) => { }
            //p.OutputDataReceived += (sender, e) => { }
        }

        /// <summary>
        /// Sets the <see cref="WorkingDirectory"/> for the command.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public Command InWorkingDirectory(string? workingDirectory) {
            this._processStartInfo.WorkingDirectory
                = workingDirectory ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Arguments"/> for the command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Command WithArguments(params string[] arguments) {
            if (arguments != null) {
                foreach (var a in arguments) {
                    this._processStartInfo.ArgumentList.Add(a);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="Arguments"/> for the command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Command WithArguments(IEnumerable<string>? arguments) {
            if (arguments != null) {
                foreach (var a in arguments) {
                    this._processStartInfo.ArgumentList.Add(a);
                }
            }

            return this;
        }

        ///// <summary>
        ///// Sets the given <see cref="Stream"/> to receive the output of
        ///// the standard error stream.
        ///// </summary>
        ///// <param name="stream"></param>
        ///// <returns></returns>
        //public Command WithErrorStream(Stream? stream) {
        //    this.ErrorStream = stream;
        //    return this;
        //}

        ///// <summary>
        ///// Sets the given <see cref="Stream"/> to receive the standard
        ///// output.
        ///// </summary>
        ///// <param name="stream"></param>
        ///// <returns></returns>
        //public Command WithOutputStream(Stream? stream) {
        //    this.OutputStream = stream;
        //    return this;
        //}

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
