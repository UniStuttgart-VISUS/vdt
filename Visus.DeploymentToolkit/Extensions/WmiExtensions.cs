// <copyright file="WmiExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using static System.Formats.Asn1.AsnWriter;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides additional methods for working with WMI objects.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal static class WmiExtensions {

        /// <summary>
        /// Gets all instances of the specified management
        /// <paramref name="class"/> in the scope <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="class"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<ManagementObject> GetInstancesOf(
                this ManagementScope that,
                ManagementPath @class,
                ObjectGetOptions? options = null) {
            using (var c = new ManagementClass(that, @class, options)) {
                return c.GetInstances().Cast<ManagementObject>();
            }
        }

        /// <summary>
        /// Gets all instances of the specified management
        /// <paramref name="class"/> in the scope <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="class"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEnumerable<ManagementObject> GetInstancesOf(
                this ManagementScope that,
                string @class,
                ObjectGetOptions? options = null)
            => that.GetInstancesOf(new ManagementPath(@class), options);

        /// <summary>
        /// Issue an object query on the given scope <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<ManagementObject> QueryObjects(
                this ManagementScope that, ObjectQuery query) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(query);
            using (var search = new ManagementObjectSearcher(that, query)) {
                return search.Get().Cast<ManagementObject>();
            }
        }

        /// <summary>
        /// Issue an object query on the given scope <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<ManagementObject> QueryObjects(
                this ManagementScope that, string query)
            => that.QueryObjects(new ObjectQuery(query));
        }
}
