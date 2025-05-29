// <copyright file="BootstrappingOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Globalization;
using System.IO;
using Visus.DeploymentToolkit.Application;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.SystemInformation;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit {

    /// <summary>
    /// Structured representation of the options used for bootstrapping the
    /// installation.
    /// </summary>
    public sealed class Options : OptionsBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        public Options() {
            this.LogFile = "deimosbootstrapper.log";
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the agent.
        /// </summary>
        /// <remarks>
        /// See <see cref="IState.AgentPath"/> for further details.
        /// </remarks>
        [State(WellKnownStates.AgentPath)]
        public string AgentPath {
            get;
            set;
        } = @"Bin\Visus.DeploymentToolkit.Agent.exe";

        /// <summary>
        /// Gets or sets the drive where the deployment share should be mapped.
        /// </summary>
        /// <remarks>
        /// This property can be <c>null</c>, in which case the programme will
        /// choose a free drive on its own.
        /// </remarks>
        [State(WellKnownStates.DeploymentDirectory)]
        public string? DeploymentDrive { get; set; }

        /// <summary>
        /// Gets or sets the name of the domain the deployment server belongs
        /// to.
        /// </summary>
        /// <remarks>
        /// This property works like the <see cref="DeploymentShare"/> in that it
        /// allows for pre-populating the domain for the UI of
        /// <see cref="MountDeploymentShare"/>.
        /// </remarks>
        [State(WellKnownStates.DeploymentShareDomain)]
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the input locale to be applied by the bootstrapper.
        /// </summary>
        [State(nameof(SetInputLocale.InputLocale))]
        public string? InputLocale {
            get => this._inputLocale;
            set {
                this._inputLocale = value;

                // Check whether the locale is not a numeric stuff, but an IETF
                // culture code. If so, try to convert it to an keyboard layout.
                try {
                    var culture = new CultureInfo(this._inputLocale!);
                    this._inputLocale = InputProfiles.ForCulture(culture)
                        ?? this._inputLocale;
                } catch { /* This is expected and can be ignored. */ }
            }
        }

        /// <summary>
        /// Gets the path to the <see cref="StateFile"/> in the
        /// <see cref="WorkingDirectory"/>.
        /// </summary>
        [State(WellKnownStates.StateFile)]
        public string StatePath {
            get => Path.Combine(this.WorkingDirectory, this.StateFile);
        }

        /// <summary>
        /// Gets or sets the name of the user to connect to the deployment
        /// share.
        /// </summary>
        /// <remarks>
        /// This property works like the <see cref="DeploymentShare"/> in that it
        /// allows for pre-populating the domain for the UI of
        /// <see cref="MountDeploymentShare"/>.
        /// </remarks>
        [State(WellKnownStates.DeploymentShareUser)]
        public string? User { get; set; }

        /// <summary>
        /// Gets or sets the path to the local working directory where the agent
        /// and the task sequence are copied to.
        /// </summary>
        [State(WellKnownStates.WorkingDirectory)]
        public string WorkingDirectory { get; set; } = @"\deimos";
        #endregion

        #region Private fields
        private string? _inputLocale;
        #endregion
    }
}
