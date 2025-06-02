// <copyright file="SelectWindowsPeSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Unattend;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that either restores the configured
    /// <see cref="IState.TaskSequence"/> or creates the default WinPE servicing
    /// task sequence.
    /// </summary>
    [SupportsPhase(Phase.PreinstalledEnvironment)]
    public sealed class SelectWindowsPeSequence : SelectTaskSequenceBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="store"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectWindowsPeSequence(IState state,
                ITaskSequenceStore? store,
                ITaskSequenceFactory tasks,
                RunCommandCustomisation runBootstrapper,
                LocalisationCustomisation setLanguage,
                ILogger<SelectWindowsPeSequence> logger)
                : base(state, store, tasks, logger) {
            ArgumentNullException.ThrowIfNull(runBootstrapper);
            ArgumentNullException.ThrowIfNull(setLanguage);

            // The following unattend.xml customisations are used when the
            // task sequence creates a new Windows PE image.
            runBootstrapper.Passes = [ Passes.WindowsPE ];
            runBootstrapper.Description = Resources.RunDeimosBootstrapper;
            runBootstrapper.Path = Path.DirectorySeparatorChar
                + Path.Combine(WorkingDirectory, Bootstrapper);

            setLanguage.Passes = [ Passes.WindowsPE ];
            setLanguage.InputLocale
                = setLanguage.UserLocale
                = setLanguage.SystemLocale
                = new("de-DE");

            //this._customisations = [runBootstrapper ];
            this._customisations = [ runBootstrapper, setLanguage ];
        }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectWindowsPeSequence(IState state,
                ITaskSequenceFactory tasks,
                RunCommandCustomisation runBootstrapper,
                LocalisationCustomisation setLanguage,
                ILogger<SelectWindowsPeSequence> logger)
            : this(state, null, tasks, runBootstrapper, setLanguage, logger) { }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            if (this._state.TaskSequence is string id) {
                var desc = await this.GetTaskSequenceAsync(id);
                if (desc != null) {
                    this._state.TaskSequence = this._tasks.CreateBuilder()
                        .FromDescription(desc)
                        .Build();
                }
            }

            if (this.IsNoTaskSequence) {
                this._logger.LogInformation("Creating default servicing task "
                    + "sequence for Windows PE.");

                this._state.TaskSequence = this._tasks.CreateBuilder()
                    .ForPhase(Phase.PreinstalledEnvironment)
                    .Add<CheckElevation>()
                    // Copy the Windows PE files from the deployment share.
                    .Add<CopyWindowsPe>()
                    // Mount the boot.wim used by Windows PE.
                    .Add<MountWim>()
                    // Copy the unattend file for Windows PE into the image.
                    .Add<CopyUnattend>(
                        (t, s) => {
                            ArgumentNullException.ThrowIfNull(s.WimMount);
                            t.Source = "Unattend_PE";
                            t.Destination = s.WimMount.MountPoint;
                        })
                    // Customise the unattend.xml to set the language and
                    // start the bootstrapper as synchronous command.
                    .Add<CustomiseUnattend>((t, s) => {
                        ArgumentNullException.ThrowIfNull(s.WimMount);
                        t.Customisations = this._customisations;
                        t.IsCritical = true;
                        t.Path = Path.Combine(s.WimMount.MountPoint,
                            CopyUnattend.DefaultFileName);
                    })
                    // Copy the bootstrapper from the deployment share into the
                    // WIM image.
                    .Add<CopyFiles>((t, s) => {
                        ArgumentNullException.ThrowIfNull(s.DeploymentShare);
                        ArgumentNullException.ThrowIfNull(s.WimMount);
                        t.Source = Path.Combine(s.DeploymentShare!,
                            DeploymentShare.Layout.BootstrapperPath);
                        t.Destination = Path.Combine(s.WimMount.MountPoint,
                            WorkingDirectory);
                        t.IsRecursive = true;
                        t.IsRequired = true;
                        t.IsCritical = true;
                    })
                    //.Add<ApplyUnattend>((t, s) => {
                    //    ArgumentNullException.ThrowIfNull(s.WimMount);
                    //    t.InstallationPath = s.WimMount.MountPoint;
                    //    t.UnattendFile = Path.Combine(s.WimMount.MountPoint,
                    //        CopyUnattend.DefaultFileName);
                    //})
                    // Unmount the image and commit the changes.
                    .Add<UnmountWim>()
                    // Create an ISO image from the customised Window PE.
                    .Add<CreateWindowsPeIso>()
                    .Build();
            }

            CheckPhase(this._state.TaskSequence as ITaskSequence,
                Phase.PreinstalledEnvironment);
        }
        #endregion

        #region Private constants
        private const string Bootstrapper = "Visus.DeploymentToolkit.Bootstrapper.exe";
        private const string WorkingDirectory = "deimos";
        #endregion

        #region Private fields
        private readonly IEnumerable<ICustomisation> _customisations;
        #endregion
    }

}
