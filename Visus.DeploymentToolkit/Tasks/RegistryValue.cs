// <copyright file="RegistryValue.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task setting a value in the registry.
    /// </summary>
    /// <param name="state">The current state of the task sequence.</param>
    /// <param name="registry">The registry service.</param>
    /// <param name="logger">A logger for the task.</param>
    [SupportedOSPlatform("windows")]
    public sealed class RegistryValue(IState state,
            IRegistry registry,
            ILogger<RegistryValue> logger)
            : TaskBase(state, logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the key where the value should be set.
        /// </summary>
        [Required]
        public string Key { get; set; } = null!;

        /// <summary>
        /// Gets or sets the operation to perform.
        /// </summary>
        public RegistryValueOperation Operation {
            get;
            set;
        } = RegistryValueOperation.Set;

        /// <summary>
        /// Gets or sets the name of the value to be set. This can be
        /// <c>null</c>, in which case the default value of the key
        /// is modified.
        /// </summary>
        public string? ValueName { get; set; }

        /// <summary>
        /// Gets or sets the value to be set.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the type of value in <see cref="Value"/>.
        /// </summary>
        public RegistryValueKind ValueKind {
            get;
            set;
        } = RegistryValueKind.None;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            switch (this.Operation) {
                case RegistryValueOperation.Add:
                    if (this._registry.KeyExists(this.Key)) {
                        this._logger.LogTrace("Leaving existing registry value "
                            + "\"{Value}\" in \"{Key}\" untouched.",
                            this.ValueName, this.Key);
                        return Task.CompletedTask;
                    }
                    break;

                case RegistryValueOperation.Change:
                    if (!this._registry.KeyExists(this.Key)) {
                        this._logger.LogTrace("A registry value \"{Value}\" "
                            + "does not exist in \"{Key}\".",
                            this.ValueName, this.Key);
                        return Task.CompletedTask;
                    }
                    break;

                case RegistryValueOperation.Delete:
                    this._logger.LogInformation("Deleting registry value "
                        + "\"{Value}\" in \"{Key}\".",
                        this.ValueName, this.Key);
                    this._registry.DeleteValue(this.Key, this.ValueName!);
                    return Task.CompletedTask;
            }

            this._logger.LogInformation("Setting registry value \"{Value}\" in "
                + "\"{Key}\".", this.ValueName, this.Key);
            this._registry.SetValue(this.Key,
                this.ValueName,
                this.Value!,
                this.ValueKind);

            return Task.CompletedTask;
        }
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void Validate() {
            base.Validate();

            switch (this.Operation) {
                case RegistryValueOperation.Add:
                case RegistryValueOperation.Change:
                case RegistryValueOperation.Set:
                    if (this.Value == null) {
                        throw new ValidationException(
                            Errors.MissingRegistryValue);
                    }
                    break;

                case RegistryValueOperation.Delete:
                    if (this.ValueName == null) {
                        throw new ValidationException(
                            Errors.CannotDeleteDefaultValue);
                    }
                    break;
            }
        }
        #endregion

        #region Private fields
        private readonly IRegistry _registry = registry
            ?? throw new ArgumentNullException(nameof(registry));
        #endregion
    }
}
