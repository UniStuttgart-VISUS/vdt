// <copyright file="DirectoryService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the file copy service.
    /// </summary>
    internal class DirectoryService : IDirectory {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DirectoryService(ILogger<DirectoryService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task CleanAsync(string path) {
            ArgumentNullException.ThrowIfNull(nameof(path));
            this._logger.LogTrace("Purging all contents from {Path}.",
                path);

            await Task.Factory.StartNew(async () => {
                foreach (var d in Directory.GetDirectories(path)) {
                    await this.DeleteAsync(d, true);
                }

                foreach (var f in Directory.GetFiles(path)) {
                    File.Delete(f);
                }
            }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<DirectoryInfo> CreateAsync(string path) {
            this._logger.LogTrace("Creating directory {Path}.", path);
            return Task.FromResult(Directory.CreateDirectory(path));
        }

        /// <inheritdoc />
        public Task DeleteAsync(string path, bool recursive = false) {
            if (recursive) {
                ArgumentNullException.ThrowIfNull(nameof(path));
                this._logger.LogTrace("Deleting directory {Path} and all "
                    + "of its contents.", path);
                return Task.Factory.StartNew(() => {
                    Directory.Delete(path, recursive);
                });

            } else {
                this._logger.LogTrace("Deleting directory {Path}.", path);
                Directory.Delete(path);
                return Task.CompletedTask;
            }
        }

        /// <inheritdoc />
        public bool Exists(string? path) => Directory.Exists(path);
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        #endregion
    }
}
