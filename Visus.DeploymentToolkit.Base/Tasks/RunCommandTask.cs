// <copyright file="RunCommandTask.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.CommandLine;
using Visus.DeploymentToolkit.Composition;


namespace Visus.DeploymentToolkit.Tasks {

    public class RunCommandTask : TaskBase {

        [Required]
        public string Path { get; set; }

        public string WorkingDirectory { get; set; } = string.Empty;

        public override Task ExecuteAsync() {
            return new Command(this.Path).ExecuteAsync();
        }
    }
}
