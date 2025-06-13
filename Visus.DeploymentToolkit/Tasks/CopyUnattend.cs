// <copyright file="CopyUnattend.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task copies one of the unattend.xml file templates to the specified
    /// location.
    /// </summary>
    /// <remarks>
    /// The advatage over this task over <see cref="CopyFiles"/> is that it knows
    /// about the strucutre of the deployment share and therefore can derive the
    /// name of the unattend file from a template string and the specified
    /// architecture.
    /// </remarks>
    public sealed class CopyUnattend : TaskBase {

        #region Public constants
        /// <summary>
        /// The default name of an unattend.xml file when no other name
        /// (autounttend.xml) is specified.
        /// </summary>
        public const string DefaultFileName = "unattend.xml";
        #endregion

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="copy"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CopyUnattend(IState state,
                ICopy copy,
                IDirectory directory,
                ILogger<CopyUnattend> logger)
                : base(state, logger) {
            this._copy = copy
                ?? throw new ArgumentNullException(nameof(copy));
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this.IsCritical = true;
            this.Name = Resources.CopyUnattend;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the architecture used to select the template provided
        /// no full path to the <see cref="Source"/> is given.
        /// </summary>
        /// <remarks>
        /// If <see cref="Source"/> is not an existing file, the task will try
        /// to append the architecture to select the appropriate template.
        /// </remarks>
        public Architecture Architecture {
            get;
            set;
        } = RuntimeInformation.ProcessArchitecture;

        /// <summary>
        /// Gets or sets the destination where the copy is placed.
        /// </summary>
        /// <remarks>
        /// The destination can be specified in different ways: If a
        /// full path including a file extension is given, the file will be
        /// copied to that exact location. If the destintion is a directory,
        /// a file named &quot;unattend.xml&quot; will be created in that
        /// directory. If the directory does not exist, it will be created
        /// recursively.
        /// </remarks>
        [Required]
        public string? Destination { get; set; }

        /// <summary>
        /// Gets or sets the source file to be copied.
        /// </summary>
        /// <remarks>
        /// If the file exists at the specified path, it will be copied as is.
        /// Otherwise, the task will try to find it in the templates directory
        /// of the deployment share and potentially try to append the
        /// <see cref="Architecture"/> to the file name, which allows for
        /// formulating the task sequence architecture independently.
        /// </remarks>
        public string Source { get; set; } = "Unattend";
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            if (!File.Exists(this.Source)) {
                var source = this.Source;

                source = this.CheckArchitecture(source, this.Architecture);

                var folder = this._state.DeploymentShare;
                if ((source is null) && (folder is not null)) {
                    source = Path.Combine(
                        folder,
                        Layout.TemplatesPath,
                        this.Source);
                    this._logger.LogTrace("Checking for unattend file in the "
                        + "deployment share {Source}.", source);
                    source = this.CheckArchitecture(source, this.Architecture);
                }

                folder = this._state.DeploymentDirectory;
                if ((source is null) && (folder is not null)) {
                    source = Path.Combine(
                        folder,
                        Layout.TemplatesPath,
                        this.Source);
                    this._logger.LogTrace("Checking for unattend file in the "
                        + "deployment directory {Source}.", source);
                    source = this.CheckArchitecture(source, this.Architecture);
                }

                if (source is null) {
                    throw new ArgumentException(
                        string.Format(Errors.InexistentUnattend, this.Source),
                        nameof(this.Source));
                }

                this.Source = source;
                Debug.Assert(File.Exists(this.Source));
            }

            if (Directory.Exists(this.Destination)) {
                this.Destination = Path.Combine(
                    this.Destination,
                    DefaultFileName);
                this._logger.LogTrace("Destination is a directory. Using "
                    + "{Destination} as file name.", this.Destination);

            } else if (!Path.HasExtension(this.Destination)) {
                this._logger.LogTrace("Creating destination directory "
                    + "{Destination}.", this.Destination);
                await this._directory.CreateAsync(this.Destination!)
                    .ConfigureAwait(false);
                this.Destination = Path.Combine(
                    this.Destination!,
                    DefaultFileName);
                this._logger.LogTrace("Destination is a new directory. Using "
                    + "{Destination} as file name.", this.Destination);
            }

            this._logger.LogInformation("Copying unattend file from "
                + "{Source} to {Destination}.", this.Source,
                this.Destination);
            await this._copy.CopyAsync(this.Source,
                this.Destination,
                CopyFlags.Required);
            this._logger.LogInformation("The unattend file was copied to "
                + "{Destination}.", this.Destination);
        }
        #endregion

        #region Private methods
        private string? CheckArchitecture(string path,
                Architecture architecture) {
            Debug.Assert(path is not null);

            if (File.Exists(path)) {
                this._logger.LogTrace("File {Source} exists in the "
                    + "specified form.", path);
                return path;
            }

            this._logger.LogTrace("Checking for unattend files for "
                + "architecture {Architecture}.", architecture);

            switch (architecture) {
                case Architecture.X86:
                    return this.CheckArchitecture(path, "x86", "i386");

                case Architecture.Arm:
                    return this.CheckArchitecture(path, "arm");

                case Architecture.Arm64:
                    return this.CheckArchitecture(path, "arm64", "aarch64");

                case Architecture.X64:
                    return this.CheckArchitecture(path, "amd64", "x64");

                default:
                    return null;
            }
        }

        private string? CheckArchitecture(string path,
                params string[] architectures) {
            if (Path.HasExtension(path)) {
                var index = path.LastIndexOf('.');

                foreach (var a in architectures) {
                    var retval = path.Insert(index, a);
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }

                    retval = path.Insert(index, '_' + a);
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }

                    retval = path.Insert(index, '-' + a);
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }
                }

            } else {
                var ext = ".xml";

                foreach (var a in architectures) {
                    var retval = path + ext;
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }

                    retval = path + a + ext;
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }

                    retval = path + '_' + a + ext;
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }

                    retval = path + '-' + a + ext;
                    this._logger.LogTrace("Checking unattend candidate "
                        + "{Source}.", retval);
                    if (Path.Exists(retval)) {
                        return retval;
                    }
                }
            } /* if (Path.HasExtension(path)) */

            return null;
        }
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        private readonly IDirectory _directory;
        #endregion
    }
}
