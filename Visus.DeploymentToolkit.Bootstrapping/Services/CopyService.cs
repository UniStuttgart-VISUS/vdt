// <copyright file="CopyService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the file copy service.
    /// </summary>
    internal class CopyService : ICopy {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CopyService(IDirectory directory,
                ILogger<CopyService> logger) {
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public Task CopyAsync(string source,
                string dest,
                CopyFlags flags) {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = dest ?? throw new ArgumentNullException(nameof(dest));
            var destDir = Directory.Exists(dest);
            var sourceDir = Directory.Exists(source);

            if ((flags & CopyFlags.Required) != CopyFlags.None) {
                var sourceFile = File.Exists(source);

                if (!sourceDir && !sourceFile) {
                    throw new ArgumentException(string.Format(
                        Errors.CopySourceMissing,
                        source));
                }
            }

            if (sourceDir && File.Exists(dest)) {
                throw new ArgumentException(string.Format(
                    Errors.CopyDirectoryToFile,
                    source,
                    dest));
            }

            return Task.Factory.StartNew(async () => {
                // Make sure that the destination directory exists.
                {
                    var dir = sourceDir ? dest : Path.GetDirectoryName(dest);
                    if ((dir != null) && !Directory.Exists(dir)) {
                        this._logger.LogTrace("Ensuring that the destination "
                            + "directory {Destination} exists.",
                            dir);
                        await this._directory.CreateAsync(dir);
                    }
                }

                if (sourceDir) {
                    var recursive = ((flags & CopyFlags.Recursive) != 0);
                    var opt = recursive
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly;

                    this._logger.LogTrace("Copying files {Recursively} from "
                        + "{Source} to {Destination}.",
                        recursive ? "recursively" : "non-recursively",
                        source, dest);

                    foreach (var f in Directory.GetFiles(source, "*", opt)) {
                        var d = RemovePrefix(f, source).TrimStart(Separators);
                        d = Path.Combine(dest, d);

                        var dd = Path.GetDirectoryName(d);
                        if (dd != null) {
                            await this._directory.CreateAsync(dd);
                        }

                        File.Copy(f, d, (flags & CopyFlags.Overwrite) != 0);
                    }

                } else {
                    if (destDir) {
                        dest = Path.Combine(dest, Path.GetFileName(source));
                    }

                    this._logger.LogTrace("Copying {Source} to "
                        + "{Destination}.", source, dest);

                    File.Copy(source, dest, (flags & CopyFlags.Overwrite) != 0);
                }
            });
        }
        #endregion

        #region Private class methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string RemovePrefix(string path, string prefix) {
            if (path.Length >= prefix.Length) {
                if (path.StartsWith(prefix, CompareMode)) {
                    return path.Substring(prefix.Length);
                }
            }

            return path;
        }
        #endregion

        #region Private constants
        private const StringComparison CompareMode
            = StringComparison.InvariantCultureIgnoreCase;

        private static readonly char[] Separators = {
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        };
        #endregion

        #region Private fields
        private readonly IDirectory _directory;
        private readonly ILogger _logger;
        #endregion
    }
}
