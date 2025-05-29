// <copyright file="TaskDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
            var retval = new TaskDescription {
                Task = task.GetType().FullName!,
                Parameters = new Dictionary<string, object?>()
            };

            var flags = BindingFlags.Public | BindingFlags.Instance;
            var properties = from p in task.GetType().GetProperties(flags)
                             where p.CanRead && p.CanWrite
                             select p;
            var options = new JsonSerializerOptions() {
                Converters = {
                    new JsonStringEnumConverter<Phase>()
                }
            };

            foreach (var p in properties) {
                var value = p.GetValue(task);
                var type = p.PropertyType;
                retval.Parameters[p.Name] = value switch {
                    null => null,
                    _ when type.IsBasicJson() => value,
                    _ => JsonSerializer.Serialize(value, type, options)
                };
            }

            return retval;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the properties to be configured on the task.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <summary>
        /// Gets or sets the fully qualified type name of the task.
        /// </summary>
        [Required]
        public string Task { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override string ToString() => this.Task ?? base.ToString()!;

        /// <inheritdoc />
        public ITask ToTask() {
            var type = Type.GetType(this.Task, true);
            if (type == null) {
                throw new InvalidOperationException(string.Format(
                    Errors.TaskNotFound, this.Task));
            }

            var retval = Activator.CreateInstance(type) as ITask;
            if (retval == null) {
                throw new InvalidOperationException(string.Format(
                    Errors.TypeNotTask, this.Task));
            }

            var options = new JsonSerializerOptions() {
                Converters = {
                    new JsonStringEnumConverter<Phase>()
                }
            };

            foreach (var p in this.Parameters) {
                var property = retval.GetType().GetProperty(p.Key);

                if (property?.CanWrite == true) {
                    type = property.PropertyType;
                    var value = p.Value switch {
                        null => null,
                        _ when type.IsBasicJson() => p.Value,
                        _ => JsonSerializer.Serialize(p.Value, type, options)
                    };
                    property.SetValue(retval, value);
                }
            }

            return retval;
        }
        #endregion
    }
}
