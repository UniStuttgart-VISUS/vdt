// <copyright file="TypeExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions {

        /// <summary>
        /// Answer whether <paramref name="that"/> is one of the basic JSON
        /// types.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsBasicJson(this Type that) {
            return (that != null)
                && ((that == typeof(string))
                || (that == typeof(bool))
                || that.IsNumeric());
        }

        /// <summary>
        /// Answer whether <paramref name="that"/> is an
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this Type that)
            => that.IsGenericOf(typeof(IEnumerable<>));

        /// <summary>
        /// Answer whether <paramref name="that"/> is an
        /// <see cref="IEnumerable{T}"/> and its generic type parameter
        /// fulfills <paramref name="predicate"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="predicate"/> is <c>null</c>.</exception>
        public static bool IsEnumerable(this Type that,
                Func<Type, bool> predicate) {
            _ = predicate ?? throw new ArgumentNullException(nameof(predicate));
            if (that.IsEnumerable()) {
                return predicate(that.GenericTypeArguments.Single());
            } else {
                return false;
            }
        }

        /// <summary>
        /// Answer whether <paramref name="that"/> is a generic type instance
        /// of type <paramref name="generic"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static bool IsGenericOf(this Type that, Type generic) {
            return (that?.IsGenericType ?? false)
                && (that.GetGenericTypeDefinition() == generic);
        }

        /// <summary>
        /// Answer whether <paramref name="that"/> is derived from
        /// <see cref="INumber{TSelf}"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsNumeric(this Type that) {
            if (that == null) {
                return false;
            } else {
                var numeric = typeof(INumber<>);
                return that.GetInterfaces().Any(i => i.IsGenericOf(numeric));
            }
        }

    }
}
