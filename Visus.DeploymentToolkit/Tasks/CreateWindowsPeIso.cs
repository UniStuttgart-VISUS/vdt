﻿// <copyright file="CreateWindowsPeIso.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task creates an ISO file holding the specified Windows PE image.
    /// </summary>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public sealed class CreateWindowsPeIso : WindowsPeTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The application state, which can be used to
        /// populate the <see cref="WorkingDirectory"/>.</param>
        /// <param name="commands">The factory for creating commands.</param>
        /// <param name="tools">Provides access to the locations of the deployment
        /// tools.</param>
        /// <param name="logger">A logger.</param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="commands"/> is <c>null</c>.</exception>
        public CreateWindowsPeIso(IState state,
                ICommandBuilderFactory commands,
                IOptions<ToolsOptions> tools,
                ILogger<CreateWindowsPeIso> logger)
                : base(state, tools,logger) {
            this._commands = commands
                ?? throw new ArgumentNullException(nameof(commands));
            this.IsCritical = true;
            this.Name = Resources.CreateIso;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path of the ISO file to be created.
        /// </summary>
        [Required]
        public string Path { get; set; } = "winpe.iso";
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            var oscdimg = this._tools.EvaluateArchitecture(
                this._tools.OscdimgPath,
                this.Architecture);
            this._logger.LogTrace("oscdimg is configured to be at {Path}.",
                oscdimg);
            var cmd = this._commands.Run(oscdimg)
                .WithArguments($"-bootdata:{this.BootData} -u1 -udfver102 "
                    + $"\"{this.MediaDirectory}\" \"{this.Path}\"")
                .WaitForProcess()
                .Build();

            this._logger.LogInformation("Running command {Command}.", cmd);
            await cmd.ExecuteAndCheckAsync(0, this._logger);

            this._logger.LogInformation("ISO file {Path} created "
                + "successfully.", this.Path);
        }
        #endregion

        #region Private methods
        /// <inheritdoc />
        protected override void Validate() {
            base.Validate();

            if (!Directory.Exists(this.WorkingDirectory)) {
                throw new ValidationException(Errors.InvalidWindowsPeDirectory);
            }
        }

        #endregion

        #region Private properties
        private string BiosPath => System.IO.Path.Combine(
            this.FirmwareDirectory, "etfsboot.com");

        private string BootData {
            get {
                var sb = new StringBuilder();

                if (File.Exists(this.BiosPath)) { 
                    sb.Append("2#p0,e,b\"");
                    sb.Append(this.BiosPath);
                    sb.Append("\"#pEF,e,b\"");
                    sb.Append(this.EfiPath);
                    sb.Append('"');

                } else {
                    sb.Append("1#pEF,e,b\"");
                    sb.Append(this.EfiPath);
                    sb.Append('"');
                }

                return sb.ToString();
            }
        }

        private string EfiPath => System.IO.Path.Combine(
            this.FirmwareDirectory, "efisys.bin");
        #endregion

        #region Private fields
        private readonly ICommandBuilderFactory _commands;
        #endregion
    }
}
