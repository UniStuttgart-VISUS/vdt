// <copyright file="ObjectExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
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
        /// Copies (if <paramref name="force"/> is <see langword="false"/>
        /// only unset) properties annotated with
        /// <see cref="FromStateAttribute"/> from <paramref name="src"/> to
        /// <paramref name="dst"/>.
        /// </summary>
        /// <param name="dst">The object receiving property values from the
        /// given <see cref="IState"/>.</param>
        /// <param name="src">The state object to read the data from.</param>
        /// <param name="force">If <see langword="true"/>, the values in
        /// <paramref name="dst"/> set set even if the respective property
        /// already has a non-<see langword="null"/> value.</param>
        /// <exception cref="ArgumentNullException">If either
        /// <paramref name="dst"/> or <paramref name="src"/> is
        /// <see langword="null"/>.</exception>
        public static void CopyFrom(this object dst,
                IState src,
                bool force = false) {
            ArgumentNullException.ThrowIfNull(dst);
            ArgumentNullException.ThrowIfNull(src);

            var props = from p in dst.GetType().GetProperties()
                        let fa = p.GetCustomAttribute<FromStateAttribute>()
                        let ra = p.GetCustomAttribute<RequiredAttribute>()
                        where (fa != null)
                        let pp = fa.Properties.Any()
                        select (p, pp ? fa.Properties : [ p.Name ], ra != null);

            foreach (var (p, ss, r) in props) {
                var v = p.GetValue(dst);

                if ((v == default) || force) {
                    foreach (var s in ss) {
                        if ((v = src[s]) != null) {
                            p.SetValue(dst, v);
                            break;
                        }
                    }
                }

                if (r && (v == null)) {
                    var msg = string.Format(Errors.RequiredStateNotSet,
                        p.Name,
                        dst.GetType().FullName,
                        string.Join(", ", ss));
                    throw new InvalidOperationException(msg);
                }
            }
        }

        /// <summary>
        /// Copies (if <paramref name="force"/> is <see langword="false"/> only
        /// unset) public settable properties from <paramref name="src"/> to
        /// <paramref name="dst"/> based on their name.
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        /// <param name="force"></param>
        public static void CopyFrom(this object dst,
                IConfiguration src,
                bool force = false) {
            ArgumentNullException.ThrowIfNull(dst);
            ArgumentNullException.ThrowIfNull(src);

            var flags = BindingFlags.Public | BindingFlags.Instance;
            var props = from p in dst.GetType().GetProperties(flags)
                        let ra = p.GetCustomAttribute<RequiredAttribute>()
                        where (p.SetMethod != null)
                        select (p, p.Name, ra != null);

            foreach (var (p, s, r) in props) {
                var v = p.GetValue(dst);

                if ((v == default) || force) {
                    if ((v = src[s]) != null) {
                        p.SetValue(dst, v);
                    }
                }

                if (r && (v == null)) {
                    var msg = Errors.RequiredPropertyNotSet;
                    msg = string.Format(msg, p.Name, dst.GetType().FullName, s);
                    throw new InvalidOperationException(msg);
                }
            }
        }

        /// <summary>
        /// Copies (if <paramref name="force"/> is set <see langword="false"/>
        /// only unsed) public settable properties annotated with
        /// <see cref="FromEnvironmentAttribute"/> of <paramref name="dst"/>
        /// from the annotated environment variables.
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="force"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="dst"/>
        /// is <see langword="null"/>.</exception>
        public static void CopyFromEnvironment(this object dst,
                bool force = false) {
            ArgumentNullException.ThrowIfNull(dst);

            var props = from p in dst.GetType().GetProperties()
                        let fa = p.GetCustomAttribute<FromEnvironmentAttribute>()
                        let ra = p.GetCustomAttribute<RequiredAttribute>()
                        where (fa != null)
                        let pp = fa.Variables.Any()
                        select (p, pp ? fa.Variables : [p.Name], ra != null, fa.Expand);

            foreach (var (p, vars, required, expand) in props) {
                var value = p.GetValue(dst);

                if ((value == default) || force) {
                    foreach (var v in vars) {
                        var env = Environment.GetEnvironmentVariable(v);

                        if (env is not null) {
                            if (expand) {
                                // If requested, expand the environment variable
                                // before setting the value.
                                env = Environment.ExpandEnvironmentVariables(
                                    env);
                            }

                            p.SetValue(dst, env);
                            break;
                        }
                    }
                }

                if (required && (value is null)) {
                    var msg = string.Format(Errors.RequiredEnvironmentNotSet,
                        p.Name,
                        dst.GetType().FullName,
                        string.Join(", ", vars));
                    throw new InvalidOperationException(msg);
                }
            }
        }

        /// <summary>
        /// Copies the properties of the source object annotated by
        /// <see cref="StateAttribute"/> to the destination state.
        /// </summary>
        /// <param name="src">The object which of the annotated properties are
        /// to be persisted in the <see cref="IState"/>.</param>
        /// <param name="dst">The state object receiving the data.</param>
        /// <paramref name="dst"/> or <paramref name="src"/> is
        /// <exception cref="ArgumentNullException">If either
        /// <paramref name="dst"/> or <paramref name="src"/> is
        /// <see langword="null"/>.</exception>
        public static void CopyTo(this object src, IState dst) {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dst);

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
