// <copyright file="PromptCredential.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Prompts the user for a network credential.
    /// </summary>
    public sealed class PromptCredential : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="input"></param>
        /// <param name="sessionSecurity"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PromptCredential(IState state,
                IConsoleInput input,
                ISessionSecurity sessionSecurity,
                ILogger<PromptCredential> logger)
                : base(state, logger) {
            this._input = input
                ?? throw new ArgumentNullException(nameof(input));
            this._sessionSecurity = sessionSecurity
                ?? throw new ArgumentNullException(nameof(sessionSecurity));
            this.Name = Resources.PromptCredential;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the name of a state where the credential is stored.
        /// </summary>
        public string? CredentialState { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the domain name.
        /// </summary>
        [Required]
        public string DomainPrompt { get; set; } = Resources.PromptDomain;

        /// <summary>
        /// Gets or sets the suggested domain to get the credential for.
        /// </summary>
        public string? DomainSuggestion { get; set; }

        /// <summary>
        /// Gets or sets the name of a state where the domain is stored.
        /// </summary>
        public string? DomainState { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the password.
        /// </summary>
        [Required]
        public string PasswordPrompt { get; set; } = Resources.PromptPassword;

        /// <summary>
        /// Gets or sets the name of a state where the password is stored.
        /// </summary>
        public string? PasswordState { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the user name.
        /// </summary>
        [Required]
        public string UserPrompt { get; set; } = Resources.PromptUser;

        /// <summary>
        /// Gets or sets the a suggestion for the user name.
        /// </summary>
        public string? UserSuggestion { get; set; }

        /// <summary>
        /// Gets or sets the name of a state where the user name is stored.
        /// </summary>
        public string? UserState { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            return Task.Run(() => {
                cancellationToken.ThrowIfCancellationRequested();
                var domain = this._input.ReadInput(
                        this.DomainPrompt,
                        this.DomainSuggestion);

                cancellationToken.ThrowIfCancellationRequested();
                var user = this._input.ReadInput(
                        this.UserPrompt,
                        this.UserSuggestion);

                cancellationToken.ThrowIfCancellationRequested();
                var password = this._input.ReadPassword(this.PasswordPrompt);

                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrWhiteSpace(this.CredentialState)) {
                    this._logger.LogTrace("Storing credential as {State}.",
                        this.CredentialState);
                    this._state[this.CredentialState] = new NetworkCredential(
                        user, password, domain);
                }

                if (!string.IsNullOrWhiteSpace(this.DomainState)) {
                    this._logger.LogTrace("Storing domain as {State}.",
                        this.DomainState);
                    this._state[this.DomainState] = domain;
                }

                if (!string.IsNullOrWhiteSpace(this.UserState)) {
                    this._logger.LogTrace("Storing user name as {State}.",
                        this.UserState);
                    this._state[this.UserState] = user;
                }

                if (!string.IsNullOrWhiteSpace(this.PasswordState)) {
                    this._logger.LogTrace("Storing password as {State}.",
                        this.PasswordState);
                    this._state[this.PasswordState] = this._sessionSecurity
                        .EncryptString(password.ToInsecureString());
                }
            });
        }
        #endregion

        #region Private fields
        private readonly IConsoleInput _input;
        private readonly ISessionSecurity _sessionSecurity;
        #endregion
    }
}
