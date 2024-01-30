// <copyright file="DismProgressCallback.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// A client-defined callback function that DISM API uses to report
    /// progress on time-consuming operations.
    /// </summary>
    /// <remarks>
    /// Cf. https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/dism/dismprogresscallback?view=windows-11
    /// </remarks>
    /// <param name="current"></param>
    /// <param name="total"></param>
    /// <param name="userData"></param>
    public delegate void DismProgressCallback(uint current, uint total,
        IntPtr userData);

}
