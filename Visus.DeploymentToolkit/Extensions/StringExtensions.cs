// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Escapes a string for use in WQL queries.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        [return:NotNullIfNotNull(nameof(that))]
        public static string? EscapeWql(this string? that) {
            if (that is null) {
                return null;
            }

            var sb = new StringBuilder(that.Length);

            foreach (var c in that) {
                if (c == '\\' || c == '\'' || c == '"') {
                    sb.Append('\\');
                }
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
