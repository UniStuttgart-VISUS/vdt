// <copyright file="DismLogLevel.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies the kind of information that is reported in the log file.
    /// </summary>
    /// <remarks>
    /// Cf. https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/dism/dismloglevel-enumeration?view=windows-11
    /// </remarks>
    public enum DismLogLevel {

        /// <summary>
        /// Log file only contains errors.
        /// </summary>
        DismLogErrors = 0,

        /// <summary>
        /// Log file contains errors and warnings.
        /// </summary>
        DismLogErrorsWarnings = 1,

        /// <summary>
        /// Log file contains errors, warnings, and additional information.
        /// </summary>
        DismLogErrorsWarningsInfo = 2
    };
}
