// <copyright file="TaskSequenceStoreOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Configures a file-based <see cref="TaskSequenceStore"/>.
    /// </summary>
    public sealed class TaskSequenceStoreOptions {

        /// <summary>
        /// The configuration section that is typically mapped to this class.
        /// </summary>
        public const string SectionName = "TaskSequenceStore";

        /// <summary>
        /// Gets or sets the way how task sequence IDs are compared.
        /// </summary>
        public StringComparison CompareOption {
            get;
            set;
        } = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// Gets or sets the filter for the files to be considered as task.
        /// </summary>
        [Required]
        public string Filter { get; set; } = "*.json";

        /// <summary>
        /// Gets or sets the path to the store.
        /// </summary>
        [Required]
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether the store should be searched recursively or
        /// only the top directory (the default).
        /// </summary>
        public SearchOption SearchOption {
            get;
            set;
        } = SearchOption.TopDirectoryOnly;
    }
}
