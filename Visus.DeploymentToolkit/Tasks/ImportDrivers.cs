// <copyright file="ImportDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.InfParser;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that imports one or more drivers into the deployment share.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="copy"></param>
    /// <param name="directories"></param>
    /// <param name="logger"></param>
    public sealed class ImportDrivers(
            IState state,
            ICopy copy,
            IDirectory directories,
            ILogger<ImportDrivers> logger)
            : TaskBase(state, logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the directory where the drivers are located in the deployment
        /// share.
        /// </summary>
        /// <remarks>
        /// If this path is not given, it will be derived from the deployment share in
        /// the state and the default <see cref="Layout"/>.
        /// </remarks>
        [DirectoryExists]
        public string Destination { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether existing drivers in the destination should be
        /// overwritten.
        /// </summary>
        public bool Overwrite { get; set; } = false;

        /// <summary>
        /// Gets or sets whether drivers should be imported from subdirectories of
        /// <see cref="Source"/> or only directly from this directory.
        /// </summary>
        public bool Recursive { get; set; } = true;

        /// <summary>
        /// Get or sets the path to the driver package to import.
        /// </summary>
        [DirectoryExists]
        public string Source { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            if (string.IsNullOrEmpty(this.Destination)) {
                this.Destination = Path.Combine(
                    this._state.DeploymentShare!,
                    Layout.DriverPath);
                this._logger.LogInformation("Derived driver destination "
                    + "directory {Destination} from deployment share.",
                    this.Destination);
            }

            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            this._logger.LogInformation("Searching drivers in {Source}.",
                this.Source);
            var enumFlags = GetItemsFlags.FilesOnly;
            if (this.Recursive) {
                enumFlags |= GetItemsFlags.Recursive;
            }

            var drivers = this._directories.GetItems(this.Source, "*.inf",
                enumFlags);

            foreach (var d in drivers) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Importing driver {InfFile}.",
                    d.FullName);
                var folder = new StringBuilder();
                folder.Append(Path.GetFileNameWithoutExtension(d.Name));

                // Get the INF file to obtain version information to generate
                // the target directory name.
                var inf = Parser.ParseFile(d.FullName);
                if (!inf.TryGetValue(WellKnownSections.Version, out var s)) {
                    throw new ArgumentException(string.Format(
                        Errors.NoInfFileVersion, d.FullName));
                }

                var section = s as KeyValueSection;
                if ((section?.TryGetValue("DriverVer", out var v) != true)
                        || (v is not DriverVersion version)) {
                    throw new ArgumentException(string.Format(
                        Errors.NoDriverVer, d.FullName));
                }
                folder.AppendFormat("_{0}.{1}.{2}.{3}_", version.Major,
                    version.Minor, version.Build, version.Revision);

                // Compute a hash of the driver files such that we can detect
                // duplicates.
                var drvDir = Path.GetDirectoryName(d.FullName)!;
                var hash = await this._directories.HashAsync(drvDir, HashFlags);
                var hexHash = Convert.ToHexString(hash);
                this._logger.LogTrace("Computed hash {Hash} for driver in "
                    + "{Directory}.", hexHash, drvDir);
                folder.Append(hexHash);

                var target = folder.ToString();
                if ((section?.TryGetValue("Class", out v) == true)
                        && (v is string @class)) {
                    @class = @class.ReplaceAll(Path.GetInvalidFileNameChars(),
                        '-');
                    target = Path.Combine(@class, target);
                }
                target = Path.Combine(this.Destination, target);
                this._logger.LogInformation("Creating driver folder {Folder}.",
                    target);
                await this._directories.CreateAsync(target);

                this._logger.LogInformation("Copying driver files from "
                    + "{Source} to {Destination}.", drvDir, target);
                var copyFlags = CopyFlags.Recursive | CopyFlags.Recursive;
                if (this.Overwrite) {
                    copyFlags |= CopyFlags.Overwrite;
                }
                await this._copy.CopyAsync(drvDir, target, copyFlags);
            }
        }
        #endregion

        #region Private fields
        private static readonly HashItemsFlags HashFlags
            = HashItemsFlags.Recursive | HashItemsFlags.IncludeDirectories;
        private readonly ICopy _copy = copy
            ?? throw new ArgumentNullException(nameof(copy));
        private readonly IDirectory _directories = directories
            ?? throw new ArgumentNullException(nameof(directories));
        #endregion
    }
}
