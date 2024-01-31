// <copyright file="VdsService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to disk management via the Virtual Disk Service VDS.
    /// </summary>
    internal sealed class VdsService : IDiskManagement {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="Exception">If the <see cref="VdsServiceLoader"/>
        /// does nto implement <see cref="IVdsServiceLoader"/>, which should
        /// never happen.</exception>
        public VdsService(ILogger<VdsService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            try {
                var loader = new VdsServiceLoader() as IVdsServiceLoader
                    ?? throw new Exception(Errors.NoVdsServiceLoader);
                loader.LoadService(null, out this._service);
            } catch (Exception ex) {
                this._logger.LogError(ex.Message, ex);
                throw;
            }
        }

        private readonly ILogger _logger;
        private readonly IVdsService _service;
    }
}
