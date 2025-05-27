// <copyright file="Components.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Provides constants for well-known components in the unattend schema.
    /// </summary>
    public static class Components {

        /// <summary>
        /// The name of the international core component.
        /// </summary>
        public const string InternationalCore
            = "Microsoft-Windows-International-Core";

        /// <summary>
        /// The name of the international core component for WinPE.
        /// </summary>
        public const string InternationalCoreWinPe
            = "Microsoft-Windows-International-Core-WinPE";

        /// <summary>
        /// The name of the component for installing Windows.
        /// </summary>
        public const string WindowsSetup = "Microsoft-Windows-Setup";

    }
}
