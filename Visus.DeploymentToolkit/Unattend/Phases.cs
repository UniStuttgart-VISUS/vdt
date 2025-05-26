// <copyright file="Phases.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Provides constants for the different phases that can be customised
    /// in an unattend.xml file.
    /// </summary>
    public static class Phases {

        public const string AuditSystem = "auditSystem";

        /// <summary>
        /// Runs as soon as you start audit mode.
        /// </summary>
        public const string AuditUser = "auditUser";

        /// <summary>
        /// These settings are applied to the image when you run sysprep.
        /// </summary>
        public const string Generalise = "generalize";

        /// <summary>
        /// These settings are applied to offline images where you apply an
        /// unattend.xml file with DISM. When you apply an unattend.xml file
        /// with DISM to an offline image, only the settings in this
        /// configuration pass are processed.
        /// </summary>
        public const string OfflineServicing = "offlineServicing";

        /// <summary>
        /// Use sparingly. Most of these settings run after the user completes
        /// OOBE.
        /// </summary>
        public const string OobeSystem = "oobeSystem";

        /// <summary>
        /// Most settings should be added here. These settings are triggered
        /// both at the beginning of audit mode and at the beginning of OOBE. If
        /// you need to make multiple updates or test settings, generalise the
        /// device again and add another batch of settings in the specialise
        /// configuration pass.
        /// </summary>
        public const string Specialise = "specialize";

        /// <summary>
        /// These settings are used by the Windows Setup installation programme.
        /// </summary>
        public const string WindowsPE = "windowsPE";
    }
}
