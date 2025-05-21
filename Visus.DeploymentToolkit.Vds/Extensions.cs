// <copyright file="VdsServiceExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Extension methods for VDS interfaces, mostly for marshalling variable
    /// size arrays.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class Extensions {

        /// <summary>
        /// Enumerate the objects of an <see cref="IEnumVdsObject"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<TValue> Enumerate<TValue>(
                this IEnumVdsObject that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));

            while (true) {
                that.Next(1, out var unknown, out uint cnt);
                if (cnt == 0) {
                    yield break;
                }

                if (unknown is TValue retval) {
                    yield return retval;
                }
            }
        }

        /// <summary>
        /// Answer all disks in <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IVdsDisk> QueryDisks(this IVdsPack that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryDisks(out var enumerator);
            return enumerator.Enumerate<IVdsDisk>();
        }

        /// <summary>
        /// Answer the properties of all file systems known to VDS.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<VDS_FILE_SYSTEM_TYPE_PROP>
        QueryFileSystemTypes(this IVdsService that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryFileSystemTypes(out var props, out var cnt);
            return Enumerate<VDS_FILE_SYSTEM_TYPE_PROP>(props, cnt);
        }

        /// <summary>
        /// Answer all all packs managed by <paramref name="that">.
        /// </summary>
        /// <param name="packs"></param>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IVdsPack> QueryPacks(
                this IVdsSwProvider that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryPacks(out var enumerator);
            return enumerator.Enumerate<IVdsPack>();
        }

        /// <summary>
        /// Enumerates all providers of the specified
        /// <paramref name="type"/> known to the <see cref="IVdsService"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<object> QueryProviders(
                this IVdsService that,
                VDS_QUERY_PROVIDER_FLAG type) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryProviders(type, out var enumerator);
            return enumerator.Enumerate<object>();
        }

        /// <summary>
        /// Enumerates all <see cref="VDS_REPARSE_POINT_PROP"/>s of
        /// <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<VDS_REPARSE_POINT_PROP> QueryReparsePoints(
                this IVdsVolumeMF that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryReparsePoints(out var props, out var cnt);
            return Enumerate<VDS_REPARSE_POINT_PROP>(props, cnt);
        }

        /// <summary>
        /// Enumerates all <see cref="IVdsSwProvider"/>s known to the
        /// <see cref="IVdsService"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IVdsSwProvider> QuerySoftwareProviders(
                this IVdsService that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryProviders(VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS,
                out var enumerator);
            return enumerator.Enumerate<IVdsSwProvider>();
        }

        /// <summary>
        /// Answer all virtual disks in <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IVdsDisk> QueryVDisks(
                this IVdsVdProvider that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryVDisks(out var enumerator);
            return enumerator.Enumerate<IVdsDisk>();
        }

        /// <summary>
        /// Answer all volumes in <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IVdsVolume> QueryVolumes(this IVdsPack that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryVolumes(out var enumerator);
            return enumerator.Enumerate<IVdsVolume>();
        }

        #region Private methods
        private static IEnumerable<TStruct> Enumerate<TStruct>(IntPtr ptr,
                int cnt) where TStruct : struct {
            if (ptr == IntPtr.Zero) {
                yield break;
            }

            for (int i = 0; i < cnt; ++i) {
                var o = ptr + i * Marshal.SizeOf<TStruct>();
                yield return Marshal.PtrToStructure<TStruct>(o);
            }

            Marshal.FreeCoTaskMem(ptr);
        }
        #endregion
    }
}
