// <copyright file="EnumExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Reflection;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="Enum"/> types.
    /// </summary>
    public static class EnumExtensions {

        /// <summary>
        /// Gets the custom attribute of type <typeparamref name="TAttribute"/>
        /// that annotates the specified enumeration value.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="that"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute? GetCustomAttribute<TAttribute, TEnum>(
                this TEnum that, bool inherit = false)
                where TAttribute : Attribute
                where TEnum : struct, Enum {
            var field = that.GetField();
            return field.GetCustomAttribute<TAttribute>(inherit);
        }

        /// <summary>
        /// Gets the <see cref="FieldInfo"/> for a given enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="that">The value to get the field of.</param>
        /// <returns>The <see cref="FieldInfo"/> describing the specified
        /// enumeration value.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static FieldInfo GetField<TEnum>(this TEnum that)
                where TEnum : struct, Enum {
            var name = Enum.GetName(that);
            if (name is null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidEnumValue, that, typeof(TEnum).Name));
            }

            var retval = that.GetType().GetField(name);
            if (retval is null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidEnumValue, that, typeof(TEnum).Name));
            }

            return retval;
        }
    }
}
