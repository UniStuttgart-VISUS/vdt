// <copyright file="CommandBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the command builder, which prepares a
    /// <see cref="ProcessStartInfo"/>.
    /// </summary>
    internal sealed class CommandBuilder : ICommandBuilder {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="processImage">The path to the process image to be
        /// started. If the process image is not an absolute path and not
        /// located in the current working directory, the builder will try
        /// resolving the absolute path using the PATH environment variable.
        /// </param>
        public CommandBuilder(string processImage) {
            _ = processImage
                ?? throw new ArgumentNullException(nameof(processImage));

            // If the path to the process image is not absolute or in the
            // current working directory, resolve it using the path
            // variable.
            if (!File.Exists(processImage)) {
                var path = Environment.GetEnvironmentVariable("PATH");
                if (path != null) {
                    foreach (var p in path.Split(Path.PathSeparator,
                            StringSplitOptions.RemoveEmptyEntries)) {
                        var pi = Path.Combine(p, processImage);
                        if (File.Exists(pi)) {
                            processImage = pi;
                            break;
                        }
                    }
                }
            }

            this._processStartInfo = new ProcessStartInfo(processImage);
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public ICommandBuilder AsCurrentUser() {
            this._processStartInfo.UserName = null;
            this._processStartInfo.Password = null;
            return this;
        }

        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public ICommandBuilder AsUser(NetworkCredential? credential) {
            this._processStartInfo.UserName = credential?.UserName;
            this._processStartInfo.Password = credential?.SecurePassword;
            return this;
        }

        /// <inheritdoc />
        public ICommand Build() {
            return new Command(this._processStartInfo) {
                DoNotWait = this._doNotWait
            };
        }

        /// <inheritdoc />
        public ICommandBuilder DoNotWaitForProcess() {
            this._doNotWait = true;
            return this;
        }

        /// <inheritdoc />
        public ICommandBuilder InWorkingDirectory(string? workingDirectory) {
            this._processStartInfo.WorkingDirectory
                = workingDirectory ?? string.Empty;
            return this;
        }

        /// <inheritdoc />
        public ICommandBuilder WaitForProcess() {
            this._doNotWait = false;
            return this;
        }

        /// <inheritdoc />
        public ICommandBuilder WithArgumentList(params string[]? arguments) {
            this._processStartInfo.ArgumentList.Clear();

            if (arguments != null) {
                foreach (var a in arguments) {
                    this._processStartInfo.ArgumentList.Add(a);
                }
            }

            return this;
        }

        /// <inheritdoc />
        public ICommandBuilder WithArgumentList(IEnumerable<string>? arguments) {
            this._processStartInfo.ArgumentList.Clear();

            if (arguments != null) {
                foreach (var a in arguments) {
                    this._processStartInfo.ArgumentList.Add(a);
                }
            }

            return this;
        }

        /// <inheritdoc />
        public ICommandBuilder WithArguments(string? arguments) {
            this._processStartInfo.ArgumentList.Clear();

            if (arguments != null) {
                this._processStartInfo.Arguments = arguments;
            }

            return this;
        }
        #endregion

        #region Private fields
        private bool _doNotWait = false;
        private readonly ProcessStartInfo _processStartInfo;
        #endregion
    }
}
