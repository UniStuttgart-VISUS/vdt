// <copyright file="FileSystemExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Linq;
using System.Reflection;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Provides extension methods for the <see cref="FileSystem"/> enumeration.
    /// </summary>
    internal static class FileSystemExtensions {

        /// <summary>
        /// Gets the implementation-agnostic <see cref="FileSystem"/> value fro the
        /// given VDS-specific file system type <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FileSystem FromVds(this VDS_FILE_SYSTEM_TYPE that) {
            var values = from f in typeof(FileSystem).GetFields()
                         let a = f.GetCustomAttribute<VdsFileSystemAttribute>()
                         where (a is not null) && (a.Value == (uint) that)
                         select (FileSystem?) f.GetValue(null);
            return values.SingleOrDefault() ?? throw new ArgumentException(
                string.Format(Errors.NoVdsFileSystem, that));
        }

        /// <summary>
        /// Gets the implementation-agnostic <see cref="FileSystem"/> value fro the
        /// given WMI-specific file system type <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FileSystem FromWmi(this ushort that) {
            var values = from f in typeof(FileSystem).GetFields()
                         let a = f.GetCustomAttribute<WmiFileSystemAttribute>()
                         where (a is not null) && (a.Value == that)
                         select (FileSystem?) f.GetValue(null);
            return values.SingleOrDefault() ?? throw new ArgumentException(
                string.Format(Errors.NoWmiFileSystem, that));
        }

        /// <summary>
        /// Converts the given implementation-independent
        /// <see cref="FileSystem"/> to the matching VDS-specific constant.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static VDS_FILE_SYSTEM_TYPE ToVds(this FileSystem that) {
            var att = that.GetCustomAttribute<VdsFileSystemAttribute,
                FileSystem>();
            if (att is null) {
                throw new ArgumentException(string.Format(
                    Errors.NoVdsFileSystem, that));
            }

            return (VDS_FILE_SYSTEM_TYPE) att.Value;
        }

        /// <summary>
        /// Converts the given implementation-independent
        /// <see cref="FileSystem"/> to the matching WMI-specific constant.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static ushort ToWmi(this FileSystem that) {
            var att = that.GetCustomAttribute<WmiFileSystemAttribute,
                FileSystem>();
            if (att is null) {
                throw new ArgumentException(string.Format(
                    Errors.NoWmiFileSystem, that));
            }

            return att.Value;
        }
    }
}
