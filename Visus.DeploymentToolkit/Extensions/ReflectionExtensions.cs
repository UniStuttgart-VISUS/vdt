// <copyright file="ReflectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for reflection objects.
    /// </summary>
    public static class ReflectionExtensions {

        /// <summary>
        /// Answer whether the given member has the specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to be
        /// checked.</typeparam>
        /// <param name="that">The member to get the attribute of.</param>
        /// <typeparamref name="TAttribute"/> was found, <see langword="false"/>
        /// otherwise.</returns>
        public static bool HasCustomAttribute<TAttribute>(this MemberInfo? that)
                where TAttribute : Attribute
            => that.TryGetCustomAttribute<TAttribute>(out _);

        /// <summary>
        /// Tries retrieving a custom attribute of the specified type
        /// <typeparamref name="TAttribute"/> from <paramref name="that"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to retrieve.
        /// </typeparam>
        /// <param name="that">The member to get the attribute of.</param>
        /// <param name="attribute">Receives the attribute in case of success.
        /// </param>
        /// <returns><see langword="true"/> if at least one attribute of
        /// <typeparamref name="TAttribute"/> was found, <see langword="false"/>
        /// otherwise.</returns>
        public static bool TryGetCustomAttribute<TAttribute>(
                this MemberInfo? that,
                [NotNullWhen(true)] out TAttribute? attribute)
                where TAttribute : Attribute {
            if (that is not null) {
                attribute = that.GetCustomAttribute<TAttribute>();
            } else {
                attribute = null;
            }

            return (attribute is not null);
        }

    }
}
