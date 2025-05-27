// <copyright file="UnattendBuilderOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The options used to configure the tools for modifying an
    /// unattend.xml file.
    /// </summary>
    public sealed class UnattendBuilderOptions {

        /// <summary>
        /// The suggested name of the configuration section mapped to this
        /// type.
        /// </summary>
        public const string SectionName = "UnattendBuilder";

        /// <summary>
        /// Gets or sets the language that is added to components.
        /// </summary>
        public string Language { get; set; } = "neutral";

        /// <summary>
        /// Gets or sets the default namespace used in unattend.xml files.
        /// </summary>
        public string Namespace {
            get;
            set;
        } = "urn:schemas-microsoft-com:unattend";

        /// <summary>
        /// Gets or sets the public key token that is added to components.
        /// </summary>
        public string PublicKeyToken { get; set; } = "31bf3856ad364e35";

        /// <summary>
        /// Gets or sets the version scope that is added to components.
        /// </summary>
        public string VersionScope { get; set; } = "nonSxS";
    }
}
