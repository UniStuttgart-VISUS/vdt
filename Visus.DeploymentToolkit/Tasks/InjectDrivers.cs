// <copyright file="InjectDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    public sealed class InjectDrivers : TaskBase {

        public InjectDrivers(ILogger<InjectDrivers> logger) : base(logger) { }

        public override Task ExecuteAsync(IState state) {

            //DismApi.MountImage()
            throw new NotImplementedException();
        }
    }
}
