// <copyright file="SupportsPhaseAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Annotates an implementation of <see cref="TaskBase"/> or
    /// <see cref="TaskBase{TResult}"/> to support the specific deploypment
    /// <see cref="DeploymentToolkit.Phase"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true, Inherited = false)]
    public sealed class SupportsPhaseAttribute : Attribute {

        /// <summary>
        /// Checks whether <paramref name="type"/> is supported for the given
        /// <paramref name="phase"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="type"/> is not annotated with any
        /// <see cref="SupportsPhaseAttribute"/>, it is assume that it is
        /// supported in any phase.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        public static bool Check(Type? type, Phase phase) {
            var supported = GetPhases(type);
            return !supported.Any() || supported.Contains(phase);
        }

        /// <summary>
        /// Gets all <see cref="Phases"/> the given <paramref name="type"/> is
        /// annotated for.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check for
        /// <see cref="SupportsPhaseAttribute"/>s. It is safe to pass
        /// <c>null</c>, in which case the result will be empty.</param>
        /// <returns>The <see cref="DeploymentToolkit.Phase"/>s the type is
        /// annotated with.</returns>
        public static IEnumerable<Phase> GetPhases(Type? type) {
            if (type == null) {
                yield break;
            }

            var atts = type.GetCustomAttributes<SupportsPhaseAttribute>();
            foreach (var a in atts) {
                yield return a.Phase;
            }
        }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="phase">The phase in which the task is
        /// supported.</param>
        public SupportsPhaseAttribute(Phase phase) {
            this.Phase = phase;
        }

        /// <summary>
        /// Gets the phase in which the task is supported.
        /// </summary>
        public Phase Phase { get; }
    }

}
