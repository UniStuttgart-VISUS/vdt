// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    internal static class StringExtensions {

        /// <summary>
        /// Answer whether <paramref name="that"/> and <paramref name="rhs"/>
        /// are equal, ignoring case.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(
                this ReadOnlySpan<char> that,
                ReadOnlySpan<char> rhs)
            => that.Equals(rhs, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Answer whether <paramref name="index"/> is a valid index into
        /// <paramref name="str"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsInRange(this ReadOnlySpan<char> str, int index)
            => ((index >= 0) && (index < str.Length));
    }
}
