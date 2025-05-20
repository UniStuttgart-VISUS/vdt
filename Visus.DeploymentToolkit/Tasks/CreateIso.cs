// <copyright file="CreateIso.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task creates an ISO file holding the specified Windows PE image.
    /// </summary>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public sealed class CreateIso : WindowsPeTaskBase {

        public CreateIso(IState state,
                ICommandBuilderFactory commands,
                ILogger<CreateIso> logger)
                : base(state, logger) {
            this._commands = commands
                ?? throw new ArgumentNullException(nameof(commands));
            this.IsCritical = true;
            this.Name = Resources.CreateIso;
        }

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the oscdimg tool.
        /// </summary>
        /// <remarks>
        /// If this parameter is not specified, the path is derived from the
        /// <see cref="DeploymentToolsRootDirectory"/>
        /// </remarks>
        public string? OscdImgPath { get; set; }

        /// <summary>
        /// Gets or sets the path of the ISO file to be created.
        /// </summary>
        public string Path { get; set; } = "winpe.iso";
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            if (string.IsNullOrWhiteSpace(this.WorkingDirectory)) {
                throw new InvalidOperationException(
                    Errors.InvalidWindowsPeDirectory);
            }

            if (string.IsNullOrWhiteSpace(this.OscdImgPath)) {
                this.OscdImgPath = System.IO.Path.Combine(
                    this.DeploymentToolsRootDirectory,
                    this.WinPeArchitecture,
                    "Oscdimg",
                    "oscdimg.exe");
            }

            var cmd = this._commands.Run(this.OscdImgPath)
                .WithArguments($"-bootdata:{this.BootData} -u1 -udfver102 "
                    + $"\"{this.MediaDirectory}\" \"{this.Path}\"")
                .WaitForProcess()
                .Build();

            this._logger.LogInformation("Running command \"{Command}\".", cmd);
            var exitCode = await cmd.ExecuteAsync();
            this._logger.LogTrace("Command \"{Command}\" exited with return "
                + "value {ExitCode}.", cmd, exitCode);

            if (exitCode != 0) {
                this._logger.LogError("Command \"{Command}\" failed with exit "
                    + "code {ExitCode}.", cmd.ToString(), exitCode.Value);
                throw new CommandFailedException(cmd.ToString()!,
                    exitCode.Value);
            }

            this._logger.LogInformation("ISO file \"{Path}\" created "
                + "successfully.", this.Path);
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
