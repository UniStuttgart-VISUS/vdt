// <copyright file="InjectDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Composition;


namespace Visus.DeploymentToolkit.Tasks {

    public sealed class InjectDrivers : TaskBase {
        public override Task ExecuteAsync() {
            //DismApi.MountImage()
            throw new NotImplementedException();
        }
    }
}
