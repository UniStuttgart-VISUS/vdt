// <copyright file="CopyFiles.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for copying one or more files.
    /// </summary>
    internal sealed class CopyFiles : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CopyFiles(ILogger<CopyFiles> logger) : base(logger) { }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the destination path of the copy operation.
        /// </summary>
        /// <remarks>
        /// The destination must be a folder unless the <see cref="Source"/>
        /// designates a single file.
        /// </remarks>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets whether existing data should be overwritten.
        /// </summary>
        public bool IsOverwrite { get; set; }

        /// <summary>
        /// Gets or sets whether the source structure should be copied
        /// recursively if <see cref="Source"/> is a folder.
        /// </summary>
        public bool IsRecursive { get; set; } = true;

        /// <summary>
        /// Gets or sets the source path of the copy operation.
        /// </summary>
        /// <remarks>
        /// If the source is a directory, all of its contents will be copied to
        /// the destination.
        /// </remarks>
        public string Source { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(IState state) {
            _ = state ?? throw new ArgumentNullException(nameof(state));
            var sourceDir = Directory.Exists(this.Source);
            var destDir = Directory.Exists(this.Destination);

            if (sourceDir && File.Exists(this.Destination)) {
                throw new InvalidOperationException(string.Format(
                    Errors.CopyDirectoryToFile,
                    this.Source,
                    this.Destination));
            }

            return Task.Factory.StartNew(() => {
                // Make sure that the destination directory exists.
                {
                    var dir = sourceDir
                        ? this.Destination
                        : Path.GetDirectoryName(this.Destination);
                    if ((dir != null) && !Directory.Exists(dir)) {
                        this._logger.LogTrace(
                            Resources.CreatingDestinationFolder,
                            dir);
                        Directory.CreateDirectory(dir);
                    }
                }

                if (sourceDir) {
                    var fmt = this.IsRecursive
                        ? Resources.CopyRecursively
                        : Resources.CopyFiles;
                    var opt = this.IsRecursive
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly;
                    var src = this.Source;

                    this._logger.LogTrace(fmt,
                        this.Source,
                        this.Destination);

                    foreach (var f in Directory.GetFiles(src, "*", opt)) {
                        var d = RemovePrefix(f, src).TrimStart(Separators);
                        d = Path.Combine(this.Destination, d);

                        var dd = Path.GetDirectoryName(d);
                        if (dd != null) {
                            Directory.CreateDirectory(dd);
                        }

                        File.Copy(f, d, this.IsOverwrite);
                    }
                }
            });
        }
        #endregion

        #region Private class methods
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
    }
}
