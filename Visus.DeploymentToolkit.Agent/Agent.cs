// <copyright file="Agent.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Agent {

    /// <summary>
    /// Implements the agent service that is performing the actual work.
    /// </summary>
    internal class Agent : IHostedService {

        public Agent(IOptions<Configuration> configuration,
                IHostApplicationLifetime lifetime,
                ILogger<Agent> logger) {
            this._configuration = configuration?.Value
                ?? throw new ArgumentNullException(nameof(configuration));
            // Cf. https://stackoverflow.com/questions/59879271/hosted-service-not-terminating-after-environment-exit
            this._lifetime = lifetime
                ?? throw new ArgumentNullException(nameof(lifetime));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation(string.Format(
                Properties.Resources.AgentStarting, this._configuration.Phase));

            try {

            } catch (Exception ex) {
                this._logger.LogError(ex.Message, ex);
                this._logger.LogError(Properties.Errors.UncaughtException);
            }

            this._lifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Agent stopping ...");
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        private readonly Configuration _configuration;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger _logger;
    }
}
