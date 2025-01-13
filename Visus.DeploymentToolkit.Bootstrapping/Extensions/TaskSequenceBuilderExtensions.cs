// <copyright file="TaskSequenceBuilderExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using Visus.DeploymentToolkit.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="ITaskSequenceBuilder"/>.
    /// </summary>
    public static class TaskSequenceBuilderExtensions {

        /// <summary>
        /// Instantiates a <typeparamref name="TTask"/> from
        /// <paramref name="services"/>, possibly configures it and adds it
        /// to <paramref name="that"/>.
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <param name="that"></param>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ITaskSequenceBuilder Add<TTask>(
                this ITaskSequenceBuilder that,
                IServiceProvider services,
                Action<TTask>? configure = null)
                where TTask : ITask {
            _ = that ?? throw new ArgumentNullException(nameof(that));
            _ = services ?? throw new ArgumentNullException(nameof(services));

            var task = services.GetRequiredService<TTask>();

            if (configure != null) {
                configure(task);
            }

            return that.Add(task);
        }
    }
}
