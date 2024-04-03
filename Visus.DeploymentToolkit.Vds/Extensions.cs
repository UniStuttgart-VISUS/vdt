// <copyright file="VdsServiceExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Extension methods for VDS interfaces, mostly for marshalling variable
    /// size arrays.
    /// </summary>
    public static class Extensions {

        public static IEnumerable<VDS_FILE_SYSTEM_TYPE_PROP> QueryFileSystemTypes(
                this IVdsService that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryFileSystemTypes(out var props, out var cnt);
            return Enumerate<VDS_FILE_SYSTEM_TYPE_PROP>(props, cnt);
        }

        public static IEnumerable<VDS_REPARSE_POINT_PROP> QueryReparsePoints(
                this IVdsVolumeMF that) {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            that.QueryReparsePoints(out var props, out var cnt);
            return Enumerate<VDS_REPARSE_POINT_PROP>(props, cnt);
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
