// <copyright file="TaskDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                where TTask : ITask
            => TaskDescriptionFactory.Create(task);
        #endregion

        #region Public properties
        /// <inheritdoc />
        public IEnumerable<IParameterDescription> DeclaredParameters
            => this.GetParameters().Select(p => new ParameterDescription(p));

        /// <summary>
        /// Gets or sets the properties to be configured on the task.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <inheritdoc />
        [Required]
        public string Task {
            get => this._task;
            set {
                this._task = value ?? throw new ArgumentNullException();
                this._type = null;  // Reset the type to force a re-evaluation.
            }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public Type Type {
            get {
                if (this._type is null) {
                    this._type = this.Task.GetImplementingType<ITask>()
                        ?? throw new InvalidOperationException(string.Format(
                        Errors.TaskNotFound, this.Task));
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
        /// <param name="type">The type of task to get the parameters for.
        /// </param>
        /// <returns>A list of parameter properties.</returns>
        internal static IEnumerable<PropertyInfo> GetParameters(Type type)
            => type.GetPublicReadWriteInstanceProperties();

        /// <summary>
        /// Gets the properties of <see cref="Type"/> that are are considered
        /// to be parameters and are listed in <see cref="Parameters"/>.
        /// </summary>
        /// <param name="force">If <see langword="true"/>, the parameters are
        /// not checked against the <see cref="Parameters"/> dictionary. This
        /// parameter defaults to <see langword="true"/>.</param>
        /// <returns>All properties that are considered to be parameters.
        /// </returns>
        internal IEnumerable<PropertyInfo> GetParameters(bool force = true) {
            var retval = from p in GetParameters(this.Type)
                         where force || this.Parameters.ContainsKey(p.Name)
                         select p;
            return retval;
        }
        #endregion

        #region Private fields
        private string _task = null!;
        private Type? _type;
        #endregion
    }
}
