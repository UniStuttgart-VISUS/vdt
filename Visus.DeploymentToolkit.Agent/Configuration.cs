// <copyright file="Configuration.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart. Alle Rechte vorbehalten.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Agent {

    /// <summary>
    /// Holds the configuration of the agent, which basically determines what it
    /// should do.
    /// </summary>
    /// <remarks>
    /// The configuration is read from the application settings file or from the
    /// command line and most importantly determines in which phase the process
    /// is. In the application settings file, the <see cref="Configuration"/>
    /// class maps to the root of the file.
    /// </remarks>
    internal sealed class Configuration {

        /// <summary>
        /// Gets or sets the phase in which the agent is currently running.
        /// </summary>
        public Phase Phase { get; set; }
    }
}
