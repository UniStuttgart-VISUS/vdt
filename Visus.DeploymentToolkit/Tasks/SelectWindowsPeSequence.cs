// <copyright file="SelectWindowsPeSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
                IOptions<ToolsOptions> tools,
                ILogger<SelectWindowsPeSequence> logger)
                : base(state, store, tasks, logger) {
            this._tools = tools?.Value
                ?? throw new ArgumentNullException(nameof(tools));

            // The following unattend.xml customisations are used when the
            // task sequence creates a new Windows PE image.
            this._customisations.Add(CustomisationDescription.Create<
                RunCommandCustomisation>(new Dictionary<string, object?> {
                    {
                        nameof(RunCommandCustomisation.Passes),
                        new string[] { Passes.WindowsPE }
                    },
                    { 
                        nameof(RunCommandCustomisation.Description),
                        Resources.RunDeimosBootstrapper
                    },
                    {
                        nameof(RunCommandCustomisation.Path),
                        Path.DirectorySeparatorChar + Path.Combine(
                            WorkingDirectory, Bootstrapper)
                    }
                }));

            this._customisations.Add(CustomisationDescription.Create<
                LocalisationCustomisation>());
            {
                var desc = this._customisations.Last();
                desc.Parameters[nameof(LocalisationCustomisation.Passes)]
                    = new string[] { Passes.WindowsPE };
                desc.Parameters[nameof(LocalisationCustomisation.InputLocale)]
                    = desc.Parameters[nameof(LocalisationCustomisation.UserLocale)]
                    = desc.Parameters[nameof(LocalisationCustomisation.SystemLocale)]
                    = new CultureInfo("de-DE");
            }
        }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectWindowsPeSequence(IState state,
                ITaskSequenceFactory tasks,
                IOptions<ToolsOptions> tools,
                ILogger<SelectWindowsPeSequence> logger)
            : this(state, null, tasks, tools, logger) { }
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
                    //// Inject WMI for manipulating disks from WinPE.
                    //.Add<AddPackage>((t, s) => {
                    //    ArgumentNullException.ThrowIfNull(s.WimMount);
                    //    t.InstallationPath = s.WimMount.MountPoint;
                    //    t.Path = "WinPE-WMI";
                    //    t.IsCommit = false;

                    //    if (string.IsNullOrEmpty(t.BasePath)) {
                    //        t.BasePath = Waik.Tools.GetWinPeOptionalComponentsPath(
                    //            s.Architecture);
                    //    }
                    //})
                    //.Add<AddPackage>((t, s) => {
                    //    ArgumentNullException.ThrowIfNull(s.WimMount);
                    //    t.InstallationPath = s.WimMount.MountPoint;
                    //    t.Path = "WinPE-NetFx";
                    //    t.IsCommit = false;

                    //    if (string.IsNullOrEmpty(t.BasePath)) {
                    //        t.BasePath = Waik.Tools.GetWinPeOptionalComponentsPath(
                    //            s.Architecture);
                    //    }
                    //})
                    //.Add<AddPackage>((t, s) => {
                    //    ArgumentNullException.ThrowIfNull(s.WimMount);
                    //    t.InstallationPath = s.WimMount.MountPoint;
                    //    t.Path = "WinPE-Scripting";
                    //    t.IsCommit = false;

                    //    if (string.IsNullOrEmpty(t.BasePath)) {
                    //        t.BasePath = Waik.Tools.GetWinPeOptionalComponentsPath(
                    //            s.Architecture);
                    //    }
                    //})
                    //.Add<AddPackage>((t, s) => {
                    //    ArgumentNullException.ThrowIfNull(s.WimMount);
                    //    t.InstallationPath = s.WimMount.MountPoint;
                    //    t.Path = "WinPE-PowerShell";
                    //    t.IsCommit = false;

                    //    if (string.IsNullOrEmpty(t.BasePath)) {
                    //        t.BasePath = Waik.Tools.GetWinPeOptionalComponentsPath(
                    //            s.Architecture);
                    //    }
                    //})
                    //.Add<AddPackage>((t, s) => {
                    //    ArgumentNullException.ThrowIfNull(s.WimMount);
                    //    t.InstallationPath = s.WimMount.MountPoint;
                    //    t.Path = "WinPE-StorageWMI";
                    //    t.IsCommit = true;

                    //    if (string.IsNullOrEmpty(t.BasePath)) {
                    //        t.BasePath = Waik.Tools.GetWinPeOptionalComponentsPath(
                    //            s.Architecture);
                    //    }
                    //})
                    .Add<AddPackage>((t, s) => {
                        ArgumentNullException.ThrowIfNull(s.WimMount);
                        t.InstallationPath = s.WimMount.MountPoint;
                        t.Path = "de-de/lp.cab";

                        if (string.IsNullOrEmpty(t.BasePath)) {
                            t.BasePath = this._tools.EvaluateArchitecture(
                                this._tools.WinPeOptionalComponentsPath,
                                s.Architecture);
                        }
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
        private readonly List<CustomisationDescription> _customisations = new();
        private readonly ToolsOptions _tools;
        #endregion
    }

}
