// <copyright file="DismProgressCallback.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// A client-defined callback function that DISM API uses to report
    /// progress on time-consuming operations. API functions that report
    /// progress accept a pointer to a <see cref="DismProgressCallback"/>.
    /// </summary>
    /// <param name="current">The current progress value.</param>
    /// <param name="total">The total progress value.</param>
    /// <param name="userData">User defined custom data. This parameter can be
    /// passed to another DISM function that accepts a progress callback and
    /// that function will then pass it through to
    /// <see cref="DismProgressCallback"/>.</param>
    public delegate void DismProgressCallback(uint current, uint total,
        IntPtr userData);
}
