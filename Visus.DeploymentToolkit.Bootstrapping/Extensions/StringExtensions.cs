// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using System.Security;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Converts a <see cref="SecureString"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
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
