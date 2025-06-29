﻿// <copyright file="CustomisationDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Describes a customisation step for an unattend.xml file.
    /// </summary>
    public sealed class CustomisationDescription {

        #region Factory methods
        /// <summary>
        /// Creates a description for the given <see cref="ICustomisation"/>
        /// <paramref name="step"/>.
        /// </summary>
        /// <typeparam name="TStep">The type of the step to be described.
        /// </typeparam>
        /// <param name="step">The step to create the description for.</param>
        /// <returns>The description of the given <paramref name="step"/>.
        /// </returns>
        public static CustomisationDescription Create<TStep>(TStep step)
                where TStep : ICustomisation {
            ArgumentNullException.ThrowIfNull(step);
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var type = step.GetType();
            var parameters = from p in type.GetProperties(flags)
                             where p.CanRead && p.CanWrite
                             select new {
                                 Key = p.Name,
                                 Value = p.GetValue(step)
                             };

            var retval = new CustomisationDescription() {
                Parameters = parameters.ToDictionary(p => p.Key, p => p.Value)!,
                Type = type.FullName!
            };

            return retval;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the properties to be set in the step.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <summary>
        /// Gets or sets the fully qualified type name of the customisation
        /// step.
        /// </summary>
        [Required]
        public string Type { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override string ToString() => this.Type ?? base.ToString()!;
        #endregion
    }
}
