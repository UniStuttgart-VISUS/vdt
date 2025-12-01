// <copyright file="WellKnownSections.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// Holds the names of well-known sections in INF files.
    /// </summary>
    public static class WellKnownSections {

        public const string ClassInstall32 = "ClassInstall32";

        public const string ClassInstall32Services = "ClassInstall32.Services";

        public const string ControlFlags = "ControlFlags";

        public const string DDInstall = "DDInstall";

        [Obsolete]
        public const string DDInstallCoInstallers = "DDInstall.CoInstallers";

        public const string DDInstallCom = "DDInstall.COM";

        public const string DDInstallComponents = "DDInstall.Components";

        public const string DDInstallEvents = "DDInstall.Events";

        [Obsolete]
        public const string DDInstallFactDef = "DDInstall.FactDef";

        public const string DDInstallFilters = "DDInstall.Filters";

        public const string DDInstallHW = "DDInstall.HW";

        public const string DDInstallInterfaces = "DDInstall.Interfaces";

        [Obsolete]
        public const string DDInstallLogConfigOverride
            = "DDInstall.LogConfigOverride";

        public const string DDInstallServices = "DDInstall.Services";

        public const string DDInstallSoftware = "DDInstall.Software";

        public const string DefaultInstall = "DefaultInstall";

        public const string DefaultInstallServices = "DefaultInstall.Services";

        public const string DestinationDirs = "DestinationDirs";

        public const string InterfaceInstall32 = "InterfaceInstall32";

        public const string Manufacturer = "Manufacturer";

        public const string Models = "Models";

        public const string SignatureAttributes =   "SignatureAttributes";

        public const string SourceDisksFiles = "SourceDisksFiles";

        public const string SourceDisksNames = "SourceDisksNames";

        public const string Strings = "Strings";

        public const string Version = "Version";
    }
}
