// <copyright file="CustomisationDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;


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
            var type = step.GetType();
            var parameters = from p in type.GetPublicReadWriteInstanceProperties()
                             select new {
                                 Key = p.Name,
                                 Value = p.GetValue(step)
                             };

            var retval = new CustomisationDescription() {
                Customisation = type.FullName!,
                Parameters = parameters.ToDictionary(p => p.Key, p => p.Value)!
            };

            return retval;
        }

        /// <summary>
        /// Create a new description for a customisation step of the given type.
        /// </summary>
        /// <typeparam name="TStep">The type of the step to be described.
        /// </typeparam>
        /// <param name="parameters">The parameters to be set on the
        /// customisation. The method will not check whether the parameters
        /// actually exist on <typeparamref name="TStep"/>.</param>
        /// <returns>A description for <typeparamref name="TStep"/>.</returns>
        public static CustomisationDescription Create<TStep>(
                IDictionary<string, object?>? parameters = null)
                where TStep : ICustomisation {
            var type = typeof(TStep);
            var retval = new CustomisationDescription() {
                Customisation = type.FullName!,
                Parameters = parameters ?? new Dictionary<string, object?>()
            };
            return retval;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the fully qualified type name of the customisation
        /// step.
        /// </summary>
        [Required]
        public string Customisation {
            get => this._customisation;
            set {
                this._customisation = value ?? throw new ArgumentNullException();
                this._type = null;  // Reset the type to force a re-evaluation.
            }
        }

        /// <summary>
        /// Gets or sets the properties to be set in the step.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <summary>
        /// Gets the type of the customisation step based on the name stored in
        /// <see cref="Customisation" />.
        /// </summary>
        [JsonIgnore]
        public Type Type {
            get {
                if (this._type is null) {
                    this._type = this.Customisation.GetImplementingType<
                        ICustomisation>()
                        ?? throw new InvalidOperationException(string.Format(
                        Errors.UnknownCustomisationType, this.Customisation));
                }
                return this._type;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Instantiates the <see cref="ICustomisation"/> described by this
        /// object from the given <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ICustomisation Create(IServiceProvider services) {
            ArgumentNullException.ThrowIfNull(services);
            var retval = (ICustomisation) services.GetRequiredService(
                this.Type);
            var parameters = this.Type.GetPublicReadWriteInstanceProperties();

            foreach (var p in parameters) {
                if (this.Parameters.TryGetValue(p.Name, out var value)) {
                    p.SetValue(retval, value);
                }
            }

            return retval;
        }

        /// <inheritdoc />
        public override string ToString()
            => this.Customisation ?? base.ToString()!;
        #endregion

        #region Private fields
        private string _customisation = null!;
        private Type? _type;
        #endregion
    }
}
