// <copyright file="TaskSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Contracts;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// The default implementation of a <see cref="ITaskSequence"/>
    /// </summary>
    internal sealed class TaskSequence : ITaskSequence {

        #region Public methods
        /// <inheritdoc />
        public async Task ExecuteAsync(Phase phase) {
            foreach (var t in this[phase]) {
                await t.ExecuteAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Public indexers
        /// <inheritdoc />
        public IEnumerable<ITask> this[Phase phase] {
            get {
                if (this._tasks.TryGetValue(phase, out var retval)) {
                    return retval;
                } else {
                    return Enumerable.Empty<ITask>();
                }
            }
        }
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="tasks"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal TaskSequence(Dictionary<Phase, List<ITask>> tasks) {
            this._tasks = tasks
                ?? throw new ArgumentNullException(nameof(tasks));
        }
        #endregion

        #region Private fields
        private readonly Dictionary<Phase, List<ITask>> _tasks;
        #endregion
    }
}
