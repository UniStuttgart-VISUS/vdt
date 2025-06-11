// <copyright file="NumberExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for numbers.
    /// </summary>
    internal static class NumberExtensions {

        /// <summary>
        /// Align the given number <paramref name="that"/> to the next
        /// <paramref name="n"/> bytes.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong CeilingAlign(this ulong that, ulong n)
            => (((that + n - 1) / n) * n);

        /// <summary>
        /// Align the given number <paramref name="that"/> to the previous
        /// <paramref name="n"/> bytes.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong FloorAlign(this ulong that, ulong n)
            => ((that / n) * n);
    }
}
