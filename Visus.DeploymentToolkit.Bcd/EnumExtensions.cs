// <copyright file="EnumExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visus.DeploymentToolkit.Bcd.Properties;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Extension methods for the annotated enumerations in this assembly.
    /// </summary>
    public static class EnumExtensions {

        /// <summary>
        /// Gets the <see cref="Guid"/> that identifies the given well-known BCD
        /// object.
        /// </summary>
        /// <param name="that">The well-known BCD object to get the GUID for.
        /// </param>
        /// <returns>The GUID of the given object or <see cref="Guid.Empty"/> if
        /// the GUID could not be determined.</returns>
        public static Guid GetGuid(this WellKnownBcdObject that)
            => that.GetField().GetCustomAttribute<BcdIDAttribute>()?.ID
                ?? Guid.Empty;

        /// <summary>
        /// Gets, if known, the human-readable names for the given BCD element
        /// as used in the bcdedit application.
        /// </summary>
        /// <param name="that"></param>
        /// <returns>The names used for the element in bcdedit.exe.</returns>
        public static IEnumerable<string> GetNames(this BcdElementType that)
            => that.GetField()
                .GetCustomAttributes<BcdEditNameAttribute>()
                .Select(a => a.Name);

        /// <summary>
        /// Gets, if known, the human-readable names for the given BCD element
        /// as used in the bcdedit application.
        /// </summary>
        /// <param name="that"></param>
        /// <returns>The names used for the element in bcdedit.exe.</returns>
        public static IEnumerable<string> GetNames(this BcdObjectType that)
            => that.GetField()
                .GetCustomAttributes<BcdEditNameAttribute>()
                .Select(a => a.Name);

        /// <summary>
        /// Gets, if known, the name used for the given well-known BCD object in
        /// the bcdedit application.
        /// </summary>
        /// <param name="that">The well-known BCD object to get the name in
        /// bcdedit.exe for.</param>
        /// <returns>The names used for the object in bcdedit.exe.</returns>
        public static IEnumerable<string> GetNames(this WellKnownBcdObject that)
            => that.GetField()
                .GetCustomAttributes<BcdEditNameAttribute>()
                .Select(a => a.Name);

        #region Private methods
        /// <summary>
        /// Gets the <see cref="FieldInfo"/> for a given enumeration value.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static FieldInfo GetField<TEnum>(this TEnum that)
                where TEnum : struct, Enum {
            var name = Enum.GetName(that);
            if (name is null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidWellKnownBcdObject, that));
            }

            var retval = that.GetType().GetField(name);
            if (retval is null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidWellKnownBcdObject, that));
            }

            return retval;
        }
        #endregion
    }
}
