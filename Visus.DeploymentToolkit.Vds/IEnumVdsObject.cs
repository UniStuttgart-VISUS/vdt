// <copyright file="IEnumVdsObject.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Enumerates through a set of VDS objects of a given type.
    /// </summary>
    [ComImport]
    [Guid("118610b7-8d94-4030-b5b8-500889788e4e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumVdsObject {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Returns a specified number of objects in the enumeration, beginning
        /// from the current point.
        /// </summary>
        /// <param name="celt">The number of objects to return.</param>
        /// <param name="retval">Receives an array of pointers, which VDS
        /// initializes on return.</param>
        /// <param name="fetched">Receives the number of pointers returned to
        /// <paramref name="retval"/>.</param>
        void Next(uint celt,
            [MarshalAs(UnmanagedType.IUnknown)] out object retval,
            out uint fetched);

        /// <summary>
        /// Skips a specified number of objects in the enumeration.
        /// </summary>
        /// <param name="celt">The number of objects to skip.</param>
        void Skip(uint celt);

        /// <summary>
        /// Resets to the beginning of the enumeration.
        /// </summary>
        void Reset();

        /// <summary>
        /// Creates an enumeration with the same state as the current enumeration.
        /// </summary>
        /// <param name="retval">Receives the copy.</param>
        void Clone(out IEnumVdsObject retval);
    }
}
