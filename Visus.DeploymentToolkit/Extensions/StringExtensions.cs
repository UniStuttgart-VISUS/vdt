// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions {

        #region Public methods
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

        /// <summary>
        /// Split an account name (user or machine) into its domain and account
        /// name.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static (string?, string?) SplitAccount(this string? that) {
            if (that is null) {
                return (null, null);
            }

            {
                var split = that.IndexOf('@');
                if (split > 0) {
                    return (that[(split + 1)..], that[..split]);
                }
            }

            {
                var split = that.IndexOf('\\');
                if (split > 0) {
                    return (that[..split], that[(split + 1)..]);
                }
            }

            {
                var split = that.IndexOf('.');
                if (split > 0) {
                    return (that[(split + 1)..], that[..split]);
                }
            }

            return (null, that);
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Searches a type by its name.
        /// </summary>
        /// <typeparam name="TInterface">The interface that the type must
        /// implement.</typeparam>
        /// <param name="that">The name of the type to look for.</param>
        /// <returns>The type described by the given name or
        /// <see langword="null"/> if no matching type was found.</returns>
        internal static Type? GetImplementingType<TInterface>(
                this string that) {
            if (that is null) {
                return null;
            }

            var retval = Type.GetType(that, false, true);
            if (retval is not null) {
                return retval;
            }

            // Try harder.
            var candidates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TInterface).IsAssignableFrom(t))
                .ToList();

            retval = candidates.SingleOrDefault(t => t.FullName == that);
            if (retval is not null) {
                return retval;
            }

            // Try even harder by using the class name only.
            retval = candidates.SingleOrDefault(t => t.Name == that);
            if (retval is not null) {
                return retval;
            }

            // Finally, try case-insensitive matching as last resort.
            retval = candidates.SingleOrDefault(
                t => t.FullName.EqualsIgnoreCase(that));
            if (retval is not null) {
                return retval;
            }

            retval = candidates.SingleOrDefault(
                t => t.Name.EqualsIgnoreCase(that));
            if (retval is not null) {
                return retval;
            }

            return retval;
        }
        #endregion
    }
}
