// <copyright file="DictionaryEytensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods dictionaries.
    /// </summary>
    internal static class DictionaryEytensions {

        /// <summary>
        /// Tries retrieving an element as a string, which will fail if the
        /// <paramref name="key"/> either does not exist or is not a string.
        /// </summary>
        /// <typeparam name="TKey">The type of the key in the dictionary.
        /// </typeparam>
        /// <param name="that">The dictionary to look up the key.</param>
        /// <param name="key">The name of the element to look up.</param>
        /// <param name="value">Receives the value of the element if it is set.
        /// </param>
        /// <returns><see langword="true"/> if the element was set and a string
        /// that has been returned to <paramref name="value"/>,
        /// <see langword="false"/> otherwise.</returns>
        public static bool TryGetStringValue<TKey>(
                this IDictionary<TKey, object?> that,
                TKey key,
                [MaybeNullWhen(false)] out string value)
                where TKey : notnull {
            if (that is not null) {
                if (that.TryGetValue(key, out var v)
                    && (v is string s)
                    && (s is not null)) {
                    value = s;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
