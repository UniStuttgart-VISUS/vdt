// <copyright file="WmiExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using System.Text;


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
        /// Retrieves the property <paramref name="name"/> from the given
        /// <see cref="ManagementBaseObject"/> <paramref name="that"/> as
        /// <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="that"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TValue GetProperty<TValue>(
                this ManagementBaseObject that,
                string name) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(name);
            return (TValue) that[name];
        }

        /// <summary>
        /// Invokes the method <paramref name="name"/> on the given object
        /// <paramref name="that"/> with the named parameter in
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="that">The object to invoke the method on.</param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static ManagementBaseObject Invoke(
                this ManagementObject that,
                string name,
                IDictionary<string, object?> args,
                InvokeMethodOptions? opts = null) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(args);

            var inParameters = that.GetMethodParameters(name);
            foreach (var (n, v) in args) {
                inParameters[n] = v;
            }

            return that.InvokeMethod(name, inParameters, opts);
        }

        /// <summary>
        /// Invokes the method <paramref name="name"/> on the given object
        /// <paramref name="that"/> with the named parameter in
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="that">The object to invoke the method on.</param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static ManagementBaseObject Invoke(
                this ManagementObject that,
                string name,
                ExpandoObject args,
                InvokeMethodOptions? opts = null)
            => that.Invoke(name, (IDictionary<string, object?>) args, opts);

        /// <summary>
        /// Invokes the method <paramref name="name"/> on the given object
        /// <paramref name="that"/> with the named parameter in
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="that">The object to invoke the method on.</param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static ManagementBaseObject Invoke(
                this ManagementObject that,
                string name,
                params (string, object)[] args) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(args);

            var inParameters = that.GetMethodParameters(name);
            foreach (var (n, v) in args) {
                inParameters[n] = v;
            }

            return that.InvokeMethod(name, inParameters, null);
        }

        /// <summary>
        /// Write the properties of the given WMI object to the given
        /// <paramref name="logger"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        public static void LogProperties(
                this ManagementBaseObject that,
                ILogger logger,
                LogLevel level = LogLevel.Trace) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(logger);
            var sb = new StringBuilder(that.ClassPath.ToString());
            sb.AppendLine(":");

            foreach (var p in that.Properties) {
                sb.Append("\t").Append(p.Name)
                    .Append(" = ").Append(p.Value?.ToString())
                    .AppendLine();
            }

            logger.Log(level, sb.ToString());
        }

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

        /// <summary>
        /// Tries to retrieve the property <paramref name="name"/> from the
        /// given <see cref="ManagementBaseObject"/> <paramref name="that"/> as
        /// <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="that"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetProperty<TValue>(
                this ManagementBaseObject that,
                string name,
                out TValue? value) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(name);

            try {
                if (that[name] is TValue v) {
                    value = v;
                    return true;
                }
            } catch { /* Ignoring this is the whole point here. */ }

            value = default;
            return false;
        }
    }
}
