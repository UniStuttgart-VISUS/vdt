// <copyright file="IBcdStore.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// The interface of a class modifying a BCD store.
    /// </summary>
    public interface IBcdStore : IEnumerable<BcdObject> {

        //string? KeyName { get; }

        //bool IsSystem { get; }

        //bool TreatAsSystem { get; }
    }
}
