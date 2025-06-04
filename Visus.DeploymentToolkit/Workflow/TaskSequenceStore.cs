// <copyright file="TaskSequenceStore.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Implements a <see cref="ITaskSequenceStore"/> based on JSON files.
    /// </summary>
    internal sealed class TaskSequenceStore : ITaskSequenceStore {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="options">The options configuring the location where
        /// the task sequences are stored.</param>
        /// <param name="logger">A logger for the store.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TaskSequenceStore(
                IOptions<TaskSequenceStoreOptions> options,
                ILogger<TaskSequenceStore> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._options = options?.Value
                ?? throw new ArgumentNullException(nameof(options));

            if (!Directory.Exists(this._options.Path)) {
                var msg = Errors.TsStoreNotFound;
                msg = string.Format(msg, this._options.Path);
                throw new ArgumentException(msg, nameof(options));
            }

            this._logger.LogInformation("A task sequence store using task "
                + "sequences from {Path} was initialised.",
                this._options.Path);
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task<ITaskSequenceDescription?> GetTaskSequenceAsync(
                string? taskSequence) {
            // Trivial reject: if we have no name, there is not TS.
            if (taskSequence == null) {
                return null;
            }

            // First, check whether this is a TS file.
            {
                var path = Path.IsPathRooted(taskSequence)
                    ? taskSequence
                    : Path.Combine(this._options.Path, taskSequence);
                if (File.Exists(path)) {
                    this._logger.LogTrace("A task sequence was identified via"
                        + " its path {Path}.", taskSequence);
                    return await TaskSequenceDescription.ParseAsync(path)
                        .ConfigureAwait(false);
                }
            }

            // Otherwise, we need to check the sequences.
            foreach (var t in await this.GetTaskSequencesAsync()) {
                if (t.ID.Equals(taskSequence, this._options.CompareOption)) {
                    this._logger.LogTrace("A task sequence was identified via"
                        + " its ID {TaskSequence}.", taskSequence);
                    return t;
                }
            }

            this._logger.LogWarning("A task sequence in a file {Path} or "
                + "with ID {TaskSequence} could not be found.",
                taskSequence, taskSequence);
            return null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ITaskSequenceDescription>>
                GetTaskSequencesAsync() {
            var files = Directory.GetFiles(this._options.Path,
                this._options.Filter,
                this._options.SearchOption);
            var retval = new List<ITaskSequenceDescription>() {
                Capacity = files.Length
            };

            foreach (var f in files) {
                var path = Path.Combine(this._options.Path, f);

                try {
                    var desc = await TaskSequenceDescription.ParseAsync(path)
                        .ConfigureAwait(false);

                    if(desc is null) {
                        this._logger.LogWarning("Parsing the task sequence in "
                            + "{Path} succeeded, but the task sequence in "
                            + "the file was empty.", path);
                    } else {
                        retval.Add(desc);
                    }
                } catch (Exception ex) {
                    this._logger.LogWarning(ex, "The task sequence in "
                        + "{Path} could not be parsed.", path);
                }
            }

            return retval;
        }
        #endregion

        #region Private fields
        private readonly ILogger<TaskSequenceStore> _logger;
        private readonly TaskSequenceStoreOptions _options;
        #endregion
    }
}
