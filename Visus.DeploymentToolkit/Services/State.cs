// <copyright file="State.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using Visus.DeploymentToolkit.Contracts;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The default implementation of the application state.
    /// </summary>
    internal sealed class State : IState {

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger">A logger used to persist changes in the state.
        /// </param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="logger"/> is <c>null</c>.</exception>
        internal State(ILogger<State> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        #endregion
    }
}
