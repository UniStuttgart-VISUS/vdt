// <copyright file="IState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to the gobal application state.
    /// </summary>
    /// <remarks>
    /// <para>The application state is created when a task sequence starts
    /// executing. It can be used to transport information from one
    /// <see cref="ITask"/> to another.</para>
    /// <para>Tasks can store any type of data in the state, but serialisation
    /// is only supported for basic JSON data types, which are
    /// <see cref="string"/>, <see cref="bool"/>, any
    /// <see cref="System.Numerics.INumber{T}"/>, or
    /// <see cref="System.Collections.Generic.IEnumerable{T}"/> thereof.
    /// Implementations may strip other values from the file or fail to
    /// reconstruct them.</para>
    /// </remarks>
    public interface IState {

        #region Public properties
        /// <summary>
        /// Gets the phase that we are running.
        /// </summary>
        Phase Phase { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the specified state.
        /// </summary>
        /// <param name="key">The key identifying the state.</param>
        /// <returns>The value of the state or <c>null</c> if the specified
        /// key is not part of the state.</returns>
        object? Get(string key);

        /// <summary>
        /// Restores the state object from a JSON file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task LoadAsync(string path);

        /// <summary>
        /// Persists the state to a JSON file at the specified location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task SaveAsync(string path);

        /// <summary>
        /// Set the given state to the specified value.
        /// </summary>
        /// <param name="key">The key of the state object.</param>
        /// <param name="value">The value of the specified key.</param>
        /// <returns>The previous value of the key or <c>null</c> if the key
        /// was not present in the state before.</returns>
        object? Set(string key, object value);
        #endregion
    }
}
