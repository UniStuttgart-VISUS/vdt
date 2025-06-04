// <copyright file="VdsServiceExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;


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
            ArgumentNullException.ThrowIfNull(that);

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
            ArgumentNullException.ThrowIfNull(that);
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
            ArgumentNullException.ThrowIfNull(that);
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
            ArgumentNullException.ThrowIfNull(that);
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
            ArgumentNullException.ThrowIfNull(that);
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
            ArgumentNullException.ThrowIfNull(that);
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
            ArgumentNullException.ThrowIfNull(that);
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
            ArgumentNullException.ThrowIfNull(that);
            that.QueryVDisks(out var enumerator);
            return enumerator.Enumerate<IVdsDisk>();
        }

        /// <summary>
        /// Calls <see cref="IVdsVolumeMF3.QueryVolumeGuidPathnames"/> and
        /// unmarshals the results.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static IEnumerable<string> QueryVolumeGuidPathNames(
                this IVdsVolumeMF3 that) {
            ArgumentNullException.ThrowIfNull(that);
            that.QueryVolumeGuidPathnames(out var blob, out var cnt);

            try {
                // Crowbared together with much help from Copilot,
                // https://learn.microsoft.com/lb-lu/windows/win32/api/vds/nf-vds-ivdsvolumemf3-queryvolumeguidpathnames
                // and https://github.com/pbatard/rufus/blob/master/src/drive.c.
                var paths = new nint[cnt];
                Marshal.Copy(blob, paths, 0, (int) cnt);

                for (int i = 0; i < cnt; ++i) {
                    yield return Marshal.PtrToStringUni(paths[i])!;
                }
            } finally {
                Marshal.FreeCoTaskMem(blob);
            }
        }

        /// <summary>
        /// Answer all volumes in <paramref name="that"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<IVdsVolume> QueryVolumes(this IVdsPack that) {
            ArgumentNullException.ThrowIfNull(that);
            that.QueryVolumes(out var enumerator);
            return enumerator.Enumerate<IVdsVolume>();
        }


        /// <summary>
        /// Sets up the cancellation of <paramref name="that"/> and returns a
        /// task that waits for the operation to complete.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<VDS_ASYNC_OUTPUT> WaitAsync(
                this IVdsAsync that,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(that);

            cancellationToken.Register(() => {
                that.QueryStatus(out var status, out var _);
                if (status == VDS_E_OPERATION_PENDING) {
                    that.Cancel();
                }
            }, false);

            return Task.Run(() => {
                that.Wait(out var hr, out var output);

                if (hr < 0) {
                    Marshal.ThrowExceptionForHR(hr);
                }

                return output;
            });
        }

        #region Private constants
        private const int VDS_E_OPERATION_PENDING = unchecked((int) 0x80042409);
        #endregion

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
