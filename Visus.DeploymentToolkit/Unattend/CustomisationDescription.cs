// <copyright file="CustomisationDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Describes how an unattend.xml file should be customised.
    /// </summary>
    /// <remarks>
    /// This is effectively a glorified list of
    /// <see cref="CustomisationDescription"/>s that can be loaded from a
    /// JSON file.
    /// </remarks>
    public sealed class CustomisationDescription
            : IEnumerable<CustomisationStepDescription> {

        #region Public properties
        /// <summary>
        /// Gets or sets an optional description of what the customisation
        /// tries to achieve.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the descriptions of the individual customisation
        /// steps.
        /// </summary>
        [Required]
        public IEnumerable<CustomisationStepDescription> Steps {
            get;
            set;
        } = [];
        #endregion

        #region Public methods
        /// <summary>
        /// Saves the description to the file at the given location.
        /// </summary>
        /// <param name="path">The path to the file where the description should
        /// be stored.</param>
        /// <returns>A task for waiting to the serialisation to complete.</returns>
        public Task SaveAsync(string path) {
            using var file = File.OpenWrite(path);
            return JsonSerializer.SerializeAsync(file, this,
                new JsonSerializerOptions() {
                    AllowTrailingCommas = false,
                    WriteIndented = true
                });
        }

        /// <inheritdoc />
        public IEnumerator<CustomisationStepDescription> GetEnumerator()
            => this.Steps!.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
