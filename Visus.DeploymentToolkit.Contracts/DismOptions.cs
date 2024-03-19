// <copyright file="DismOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Contracts {

    /// <summary>
    /// Allows for controlling the global behaviour of the DISM scope.
    /// </summary>
    public sealed class DismOptions {

        /// <summary>
        /// Gets or sets the path to the DISM log file.
        /// </summary>
        /// <remarks>
        /// If <c>null</c>, the default log file will be used.
        /// </remarks>
        public string? LogFile { get; set; }

        /// <summary>
        /// Gets or sets the scratch directory used by DISM.
        /// </summary>
        /// <remarks>
        /// If <c>null</c>, DISM will use its default scratch directory in the
        /// system's temporary folder.
        /// </remarks>
        public string? ScratchDirectory { get; set; }
    }
}
