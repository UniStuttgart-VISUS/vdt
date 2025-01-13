// <copyright file="InjectDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    public sealed class InjectDrivers : TaskBase {

        public InjectDrivers(IState state,
                IDismScope dism,
                ILogger<InjectDrivers> logger)
                : base(state, logger) { }

        public override Task ExecuteAsync(CancellationToken cancellationToken) {

            //DismApi.MountImage()
            throw new NotImplementedException();
        }
    }
}
