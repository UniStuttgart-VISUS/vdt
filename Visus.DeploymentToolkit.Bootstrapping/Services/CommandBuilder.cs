// <copyright file="CommandBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <param name="processImage">The absolute path to the process image
        /// to be started.</param>
        public CommandBuilder(string processImage) {
            _ = processImage
                ?? throw new ArgumentNullException(nameof(processImage));
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
            return new Command(this._processStartInfo);
        }

        /// <inheritdoc />
        public ICommandBuilder InWorkingDirectory(string? workingDirectory) {
            this._processStartInfo.WorkingDirectory
                = workingDirectory ?? string.Empty;
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
        private readonly ProcessStartInfo _processStartInfo;
        #endregion
    }
}
