// <copyright file="CopyFiles.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for copying one or more files.
    /// </summary>
    public sealed class CopyFiles : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="copy"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CopyFiles(IState state,
                ICopy copy,
                ILogger<CopyFiles> logger)
                : base(state, logger) {
            this._copy = copy ?? throw new ArgumentNullException(nameof(copy));
            this.Name = Resources.CopyFiles;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the destination path of the copy operation.
        /// </summary>
        /// <remarks>
        /// The destination must be a folder unless the <see cref="Source"/>
        /// designates a single file.
        /// </remarks>
        [Required]
        public string Destination { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether existing data should be overwritten.
        /// </summary>
        [DefaultValue(false)]
        public bool IsOverwrite { get; set; }

        /// <summary>
        /// Gets or sets whether the source structure should be copied
        /// recursively if <see cref="Source"/> is a folder.
        /// </summary>
        [DefaultValue(true)]
        public bool IsRecursive { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the source file or directory must exist.
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets the source path of the copy operation.
        /// </summary>
        /// <remarks>
        /// If the source is a directory, all of its contents will be copied to
        /// the destination.
        /// </remarks>
        [Required]
        public string Source { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            var flags = CopyFlags.None;
            if (this.IsRecursive) {
                flags |= CopyFlags.Recursive;
            }
            if (this.IsRequired) {
                flags |= CopyFlags.Required;
            }
            if (this.IsOverwrite) {
                flags |= CopyFlags.Overwrite;
            }

            this.Validate();

            return this._copy.CopyAsync(this.Source, this.Destination, flags);
        }
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        #endregion
    }
}
