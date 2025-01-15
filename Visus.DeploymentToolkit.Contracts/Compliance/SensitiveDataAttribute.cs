// <copyright file="SensitiveDataAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Compliance.Classification;


namespace Visus.DeploymentToolkit.Compliance {

    /// <summary>
    /// Annotates data as being sensitive, which applies for instance to
    /// passwords and/or cryptographic keys.
    /// </summary>
    public sealed class SensitiveDataAttribute()
        : DataClassificationAttribute(Compliance.Classification.SensitiveData) { }
}
