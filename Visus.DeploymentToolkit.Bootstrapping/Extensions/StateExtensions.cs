// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 StateExtensions der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Diagnostics.CodeAnalysis;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="IState"/>.
    /// </summary>
    public static class StateExtensions {

        /// <summary>
        /// Tries to retrieve a state value and convert it to the specified
        /// type.
        /// </summary>
        /// <typeparam name="TValue">The desired type of the state value. If the
        /// state exists, but is not of the specified type, the method will fail,
        /// too.</typeparam>
        /// <param name="that"></param>
        /// <param name="state"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<TValue>(this IState that,
                string state,
                [MaybeNullWhen(false)] out TValue value) {
            if ((that is not null) && (that[state] is TValue v)) {
                value = v;
                return (value is not null);
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Tries to retrieve a state value and check whether it is a non-empty,
        /// non-whitespace string.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="state"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetNonEmptyString(this IState that,
                string state,
                [MaybeNullWhen(false)] out string value) {
            if (that.TryGetValue<string>(state, out value)) {
                return !string.IsNullOrWhiteSpace(value);
            } else {
                return false;
            }
        }

    }
}
