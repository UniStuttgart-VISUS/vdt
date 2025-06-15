// <copyright file="DirectoryService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public Task CleanAsync(string path) {
            ArgumentNullException.ThrowIfNull(nameof(path));
            this._logger.LogTrace("Purging all contents from {Path}.",
                path);

            if (!Directory.Exists(path)) {
                this._logger.LogTrace("Directory {Path} did not exist in "
                    + "the first place.", path);
                return Task.CompletedTask;
            }

            return Task.Run(async () => {
                this._logger.LogTrace("Cleaning directory {Path}.", path);
                foreach (var d in Directory.GetDirectories(path)) {
                    await this.DeleteAsync(d, true);
                }

                foreach (var f in Directory.GetFiles(path)) {
                    File.Delete(f);
                }
            });
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

        /// <inheritdoc />
        public IEnumerable<FileSystemInfo> GetItems(string? path,
                string? pattern = null,
                GetItemsFlags flags = GetItemsFlags.None) {
            if(string.IsNullOrWhiteSpace(path)) {
                path = Directory.GetCurrentDirectory();
                this._logger.LogTrace("Using current working directory "
                    + "{Path}.", path);
            }

            if (pattern is null) {
                pattern = "*";
            }
            this._logger.LogTrace("Enumerating items in directory {Path} "
                + "with pattern {Pattern}.", path, pattern);

            var options = SearchOption.TopDirectoryOnly;

            if (flags.HasFlag(GetItemsFlags.Recursive)) {
            this._logger.LogTrace("Recursively enumerating items in directory "
                + "{Path}.", path);
                options = SearchOption.AllDirectories;
            }

            if (flags.HasFlag(GetItemsFlags.FilesOnly)) {
                this._logger.LogTrace("Enumerating only files in directory "
                    + "{Path}.", path);
                foreach (var f in Directory.GetFiles(path, pattern, options)) {
                    yield return new FileInfo(Path.Combine(path, f));
                }

            } else {
                this._logger.LogTrace("Enumerating all items in directory "
                    + "{Path}.", path);
                foreach (var f in Directory.GetDirectories(path, pattern,
                        options)) {
                    var p = Path.Combine(path, f);
                    if (Directory.Exists(p)) {
                        yield return new DirectoryInfo(p);
                    } else {
                        yield return new FileInfo(p);
                    }
                }
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        #endregion
    }
}
