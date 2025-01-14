// <copyright file="PersistState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for copying one or more files.
    /// </summary>
    public sealed class PersistState : TaskBase {

        #region Public constants
        /// <summary>
        /// The default path of the state file if none is specified.
        /// </summary>
        public const string DefaultPath = "deimosstate.json";
        #endregion

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PersistState(IState state, ILogger<PersistState> logger)
                : base(state, logger) {
            this.Name = Resources.PersistState;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the destination path of the state file.
        /// </summary>
        /// <remarks>
        /// The destination must be a folder unless the <see cref="Source"/>
        /// designates a single file.
        /// </remarks>
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            if (string.IsNullOrWhiteSpace(this.Path)) {
                this.Path = DefaultPath;
            }

            return this._state.SaveAsync(this.Path);
        }
        #endregion
    }
}
