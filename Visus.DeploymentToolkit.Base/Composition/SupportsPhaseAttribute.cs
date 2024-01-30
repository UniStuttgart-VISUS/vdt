// <copyright file="SupportsPhaseAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>

using System.Reflection;


namespace Visus.DeploymentToolkit.Composition {

    /// <summary>
    /// Annotates an implementation of <see cref="TaskBase"/> or
    /// <see cref="TaskBase{TResult}"/> to support the specific deploypment
    /// <see cref="DeploymentToolkit.Phase"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true, Inherited = false)]
    public sealed class SupportsPhaseAttribute : Attribute {

        /// <summary>
        /// Gets all <see cref="Phases"/> the given <paramref name="type"/> is
        /// annotated for.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check for
        /// <see cref="SupportsPhaseAttribute"/>s. It is safe to pass
        /// <c>null</c>, in which case the result will be empty.</param>
        /// <returns>The <see cref="DeploymentToolkit.Phase"/>s the type is annotated
        /// with.</returns>
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
            Phase = phase;
        }

        /// <summary>
        /// Gets the phase in which the task is supported.
        /// </summary>
        public Phase Phase { get; }
    }
}
