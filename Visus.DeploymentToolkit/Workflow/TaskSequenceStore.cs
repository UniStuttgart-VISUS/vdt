// <copyright file="TaskSequenceStore.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        /// <param name="path">The location where the task sequences are
        /// stored.</param>
        /// <param name="logger">A logger for the store.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TaskSequenceStore(string path,
                ILogger<TaskSequenceStore> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._path = path
                ?? throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(this._path)) {
                var msg = Errors.TsStoreNotFound;
                msg = string.Format(msg, this._path);
                throw new ArgumentException(msg, nameof(path));
            }

            this._logger.LogInformation("A task sequence store using task "
                + "sequences from \"{Path}\" was initialised.", this._path);
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task<ITaskSequenceDescription?> GetTaskSequenceAsync(
                string taskSequence) {
            _ = taskSequence
                ?? throw new ArgumentNullException(nameof(taskSequence));

            // First, check whether this is a TS file.
            {
                var path = Path.Combine(this._path, taskSequence);
                if (File.Exists(path)) {
                    return await TaskSequenceDescription.ParseAsync(path);
                }
            }

            // Otherwise, we need to check the sequences.
            foreach (var t in await this.GetTaskSequencesAsync()) {
                if (string.Equals(t.ID,
                        taskSequence,
                        StringComparison.InvariantCultureIgnoreCase)) {
                    return t;
                }
            }

            this._logger.LogWarning("A task sequence in a file \"{Path}\" or "
                + "with ID \"{TaskSequence}\" could not be found.",
                taskSequence, taskSequence);
            return null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ITaskSequenceDescription>>
                GetTaskSequencesAsync() {
            var files = Directory.GetFiles(this._path,
                "*.json",
                SearchOption.AllDirectories);
            var retval = new List<ITaskSequenceDescription>() {
                Capacity = files.Length
            };

            foreach (var f in files) {
                var path = Path.Combine(this._path, f);

                try {
                    var desc = await TaskSequenceDescription.ParseAsync(path);

                    if(desc is null) {
                        this._logger.LogWarning("Parsing the task sequence in "
                            + "\"{Path}\" succeeded, but the task sequence in "
                            + "the file was empty.", path);
                    } else {
                        retval.Add(desc);
                    }
                } catch (Exception ex) {
                    this._logger.LogWarning(ex, "The task sequence in "
                        + "\"{Path}\" could not be parsed.", path);
                }
            }

            return retval;
        }
        #endregion

        #region Private fields
        private readonly ILogger<TaskSequenceStore> _logger;
        private readonly string _path;
        #endregion
    }
}
