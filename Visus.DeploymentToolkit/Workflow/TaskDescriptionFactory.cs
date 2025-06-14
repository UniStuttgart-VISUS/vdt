// <copyright file="TaskDescriptionBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// A factory for creating <see cref="ITaskDescription"/>s.
    /// </summary>
    public static class TaskDescriptionFactory {

        /// <summary>
        /// Creates a description for the specified <paramref name="task"/>.
        /// </summary>
        /// <typeparam name="TTask">The type of the task to create the
        /// description of.</typeparam>
        /// <param name="task">The task to be described.</param>
        /// <returns>A description of the specified task.</returns>
        public static ITaskDescription FromTask<TTask>(TTask task)
                where TTask : ITask {
            var retval = (TaskDescription) FromType<TTask>();

            foreach (var p in TaskDescription.GetParameters(typeof(TTask))) {
                var value = p.GetValue(task);
                retval.Parameters[p.Name] = value;
            }

            return retval;
        }

        /// <summary>
        /// Creates a description for the specified <paramref name="type"/>
        /// of task.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ITaskDescription FromType(Type type) => Create(type);

        /// <summary>
        /// Creates a description for a default-initialised task of the
        /// specified <typeparamref name="TTask"/> type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task to create the
        /// description of.</typeparam>
        /// <returns>A description of the specified task.</returns>
        public static ITaskDescription FromType<TTask>() where TTask : ITask
            => Create(typeof(TTask));

        /// <summary>
        /// Creates a new description of a task from its type name.
        /// </summary>
        /// <param name="type">The name of the task type.</param>
        /// <returns>A description of the specified task.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/>
        /// is <see langword="null"/>.</exception>
        public static ITaskDescription FromType(string type) {
            var retval = new TaskDescription() {
                Task = type ?? throw new ArgumentNullException(nameof(type)),
                Parameters = new Dictionary<string, object?>()
            };
            _ = retval.Type; // Force type resolution.
            return retval;
        }

        #region Internal methods
        /// <summary>
        /// Creates a description for the specified <paramref name="type"/>
        /// of task.
        /// </summary>
        /// <param name="type">The type of the task to describe.</param>
        /// <returns>A description of the specified task.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="type"/> is
        /// either not derived from <see cref="ITask"/> or is an abstract type.
        /// </exception>
        internal static TaskDescription Create(Type type) {
            ArgumentNullException.ThrowIfNull(type);

            if (!typeof(ITask).IsAssignableFrom(type)) {
                throw new ArgumentException(string.Format(Errors.TypeNoTask,
                    type.FullName));
            }

            if (type.IsAbstract) {
                throw new ArgumentException(string.Format(Errors.TaskAbstract,
                    type.FullName));
            }

            if (type.IsGenericOf(typeof(SelfConfiguringTask<>))) {
                // Serialise a self-configuring task as its base task. It is
                // impossible to restore the self configuration part from JSON
                // as this happens via lambdas in code.
                type = type.GetGenericArguments().Single();
            }

            var retval = new TaskDescription {
                //Task = type.AssemblyQualifiedName!,
                Task = type.FullName!,
                Parameters = new Dictionary<string, object?>()
            };

            return retval;
        }

        /// <summary>
        /// Creates a description for the specified <paramref name="task"/>.
        /// </summary>
        /// <typeparam name="TTask">The type of the task to create the
        /// description of.</typeparam>
        /// <param name="task">The task to be described.</param>
        /// <returns>A description of the specified task.</returns>
        internal static TaskDescription Create<TTask>(TTask task)
                where TTask : ITask {
            var type = typeof(TTask);

            // Create the abstract description of the task type.
            var retval = Create(type);

            // Assign the specific parameter values from the instance.
            foreach (var p in TaskDescription.GetParameters(type)) {
                var value = p.GetValue(task);
                retval.Parameters[p.Name] = value;
            }

            return retval;
        }
        #endregion
    }
}
