// <copyright file="Classification.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Compliance.Classification;


namespace Visus.DeploymentToolkit.Compliance {

    /// <summary>
    /// Defines the taxonomy for redacting personally identifiable or sensitive
    /// data.
    /// </summary>
    public static class Classification {

        /// <summary>
        /// Gets the name of the taxonomy.
        /// </summary>
        public static string TaxonomyName => typeof(Classification).Name;

        /// <summary>
        /// Gets the classification for sensitive data.
        /// </summary>
        public static DataClassification SensitiveData { get; }
            = new(TaxonomyName, nameof(SensitiveData));
    }
}
