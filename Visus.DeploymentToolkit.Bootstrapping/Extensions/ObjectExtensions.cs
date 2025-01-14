// <copyright file="ObjectExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions {

        /// <summary>
        /// Copies (if <paramref name="force"/> is <c>false</c> only unset)
        /// properties annotated with <see cref="FromStateAttribute"/> from
        /// <paramref name="src"/> to <paramref name="dst"/>.
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        /// <param name="force"></param>
        public static void CopyFrom(this object dst,
                IState src,
                bool force = false) {
            _ = dst ?? throw new ArgumentNullException(nameof(dst));
            _ = src ?? throw new ArgumentNullException(nameof(src));

            var props = from p in dst.GetType().GetProperties()
                        let fa = p.GetCustomAttribute<FromStateAttribute>()
                        let ra = p.GetCustomAttribute<RequiredAttribute>()
                        where (fa != null)
                        select (p, fa.Property ?? p.Name, ra != null);

            foreach (var (p, s, r) in props) {
                var v = p.GetValue(dst);

                if ((v == null) || force) {
                    if ((v = src[s]) != null) {
                        p.SetValue(dst, v);
                    }
                }

                if (r && (v == null)) {
                    var msg = Errors.RequiredStateNotSet;
                    msg = string.Format(msg, p.Name, dst.GetType().FullName, s);
                    throw new InvalidOperationException(msg);
                }
            }
        }

        /// <summary>
        /// Copies the properties of the source object annotated by
        /// <see cref="StateAttribute"/> to the destination state.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CopyTo(this object src, IState dst) {
            _ = src ?? throw new ArgumentNullException(nameof(src));
            _ = dst ?? throw new ArgumentNullException(nameof(dst));

            var props = from p in src.GetType().GetProperties()
                        let a = p.GetCustomAttribute<StateAttribute>()
                        where (a != null)
                        select (p, a.Property ?? p.Name);

            foreach (var (p, n) in props) {
                dst[n] = p.GetValue(src);
            }
        }
    }
}
