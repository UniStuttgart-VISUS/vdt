// <copyright file="WellKnownBcdObjectExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
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
    /// Extension methods for the <see cref="WellKnownBcdObject"/> enumeration.
    /// </summary>
    public static class WellKnownBcdObjectExtensions {

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
        /// Gets, if known, the name used for the given well-known BCD object in
        /// the bcdedit.application.
        /// </summary>
        /// <param name="that">The well-known BCD object to get the name in
        /// bcdedit.exe for.</param>
        /// <returns>The name used for the object in bcdedit.exe or
        /// <see langword="null"/> if this information is unavailable.</returns>
        public static IEnumerable<string> GetNames(this WellKnownBcdObject that)
            => that.GetField()
                .GetCustomAttributes<BcdEditNameAttribute>()
                .Select(a => a.Name);

        #region Private methods
        private static FieldInfo GetField(this WellKnownBcdObject that) {
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
