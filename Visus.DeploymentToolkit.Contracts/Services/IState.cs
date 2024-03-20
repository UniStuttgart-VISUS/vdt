// <copyright file="IState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to the gobal application state.
    /// </summary>
    /// <remarks>
    /// The application state is created when a task sequence starts executing.
    /// It can be used to transport information from one <see cref="ITask"/>
    /// to another.
    /// </remarks>
    public interface IState {

        /// <summary>
        /// Gets the phase that we are running.
        /// </summary>
        public string Phase { get; }
    }
}
