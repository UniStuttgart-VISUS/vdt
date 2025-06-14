// <copyright file="TaskDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// The in-memory representation of a task description in JSON.
    /// </summary>
    internal sealed class TaskDescription : ITaskDescription {

        #region Factory methods
        /// <summary>
        /// Creates a description for the specified <paramref name="task"/>.
        /// </summary>
        /// <typeparam name="TTask">The type of the task to be described.
        /// </typeparam>
        /// <param name="task">The task to be described.</param>
        /// <returns>A description for the given task.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/>
        /// is <c>null</c>.</exception>
        public static TaskDescription FromTask<TTask>(TTask task)
                where TTask : ITask {
            ArgumentNullException.ThrowIfNull(task);
            var type = task.GetType();

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

            foreach (var p in GetParameters(typeof(TTask))) {
                var value = p.GetValue(task);
                retval.Parameters[p.Name] = value;
            }

            return retval;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the properties to be configured on the task.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <inheritdoc />
        IEnumerable<IParameterDescription> ITaskDescription.Parameters
            => this.GetParameters().Select(p => new ParameterDescription(p));

        /// <summary>
        /// Gets or sets the fully qualified type name of the task.
        /// </summary>
        [Required]
        public string Task {
            get => this._task;
            set {
                this._task = value ?? throw new ArgumentNullException();
                this._type = null;  // Reset the type to force a re-evaluation.
            }
        }

        /// <summary>
        /// Gets the type resolved from <see cref="Task"/>
        /// </summary>
        [JsonIgnore]
        public Type Type {
            get {
                if (this._type is null) {
                    this._type = Type.GetType(this.Task, false, true);

                    if (this._type is null) {
                        // Try harder.
                        this._type = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == this.Task);
                    }

                    if (this._type is null) {
                        // Now, we cannot do anything else.
                        throw new InvalidOperationException(string.Format(
                            Errors.TaskNotFound, this.Task));
                    }
                }

                return this._type;
            }
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override string ToString() => this.Task ?? base.ToString()!;

        /// <inheritdoc />
        public ITask ToTask(IServiceProvider services) {
            ArgumentNullException.ThrowIfNull(services);

            var retval = services.GetRequiredService(this.Type) as ITask;
            if (retval == null) {
                throw new InvalidOperationException(string.Format(
                    Errors.TypeNotTask, this.Task));
            }

            if (this.Parameters is not null) {
                foreach (var p in this.Parameters) {
                    var property = retval.GetType().GetProperty(p.Key);
                    if (property?.CanWrite == true) {
                        property.SetValue(retval, p.Value);
                    }
                }
            }

            return retval;
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Gets the properties of the task <paramref name="type"/> that are
        /// considered to be parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> GetParameters(Type type) {
            ArgumentNullException.ThrowIfNull(type);
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var retval = from p in type.GetProperties(flags)
                         where p.CanRead && p.CanWrite
                         select p;
            return retval;
        }

        /// <summary>
        /// Gets the properties of <see cref="Type"/> that are  are considered
        /// to be parameters.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<PropertyInfo> GetParameters()
            => GetParameters(this.Type);
        #endregion

        #region Private fields
        private string _task = null!;
        private Type? _type;
        #endregion
    }
}
