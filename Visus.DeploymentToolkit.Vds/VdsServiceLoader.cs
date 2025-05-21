// <copyright file="VdsServiceLoader.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Performs the <c>CoCreateInstance</c> for the
    /// <see cref="IVdsServiceLoader"/>. This class can be cased to this
    /// service loader interface.
    /// </summary>
    [ComImport]
    [Guid("9c38ed61-d565-4728-aeee-c80952f0ecde")]
    [SupportedOSPlatform("windows")]
    public class VdsServiceLoader { }
}
