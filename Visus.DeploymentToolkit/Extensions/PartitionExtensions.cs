// <copyright file="PartitionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Visus.DeploymentToolkit.DiskManagement;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="IPartition"/>.
    /// </summary>
    internal static class PartitionExtensions {

        /// <summary>
        /// Answer whether the given partition is of the specified
        /// <paramref name="type"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsType(this IPartition that, PartitionType type) {
            if ((that is null) || (type is null)) {
                return false;
            }

            return type.Equals(that.Type);
        }

        /// <summary>
        /// Answer whether the given partition is of one of the specified
        /// <paramref name="types"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool IsType(this IPartition that,
                IEnumerable<PartitionType> types) {
            if ((that is null) || (types is null)) {
                return false;
            }

            return types.Any(t => t.Equals(that.Type));
        }
    }
}
