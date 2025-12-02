// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Compares two strings for equality, ignoring case.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string? that, string? other)
            => string.Equals(that, other, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Replaces any occurrence of any character in
        /// <paramref name="needles"/> in <paramref name="that"/> with
        /// <paramref name="replacement"/> in place.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="needles"></param>
        /// <param name="replacement"></param>
        public static void ReplaceAll(this Span<char> that,
                ReadOnlySpan<char> needles,
                char replacement) {
            foreach (var n in needles) {
                that.Replace(n, replacement);
            }
        }

        /// <summary>
        /// Replaces any occurrence of any character in
        /// <paramref name="needles"/> in <paramref name="that"/> with
        /// <paramref name="replacement"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="needles"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceAll(this string that,
                ReadOnlySpan<char> needles,
                char replacement) {
            ArgumentNullException.ThrowIfNull(that);
            foreach (var n in needles) {
                that = that.Replace(n, replacement);
            }
            return that;
        }

        /// <summary>
        /// Converts a <see cref="SecureString"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(that))]
        public static string? ToInsecureString(this SecureString? that) {
            string? retval = null;

            if (that != null) {
                var ptr = Marshal.SecureStringToGlobalAllocUnicode(that);

                try {
                    retval = Marshal.PtrToStringUni(ptr);
                } finally {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }

            return retval;
        }

        /// <summary>
        /// Convert a <see cref="string"/> to a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SecureString? ToSecureString(this string? that) {
            SecureString? retval = null;

            if (that != null) {
                retval = new SecureString();

                foreach (var c in that) {
                    retval.AppendChar(c);
                }
            }

            return retval;
        }
    }
}
