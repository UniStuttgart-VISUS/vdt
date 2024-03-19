// <copyright file="DismImageType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies the kind of information that is reported in the log file.
    /// </summary>
    public enum DismLogLevel {

        /// <summary>
        /// Log file only contains errors.
        /// </summary>
        Errors = 0,

        /// <summary>
        /// Log file contains errors and warnings.
        /// </summary>
        ErrorsWarnings = 1,

        /// <summary>
        /// Log file contains errors, warnings, and additional information.
        /// </summary>
        ErrorsWarningsInfo = 2
    }
}
