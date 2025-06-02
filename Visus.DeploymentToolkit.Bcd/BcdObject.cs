// <copyright file="BcdObject.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Bcd.Properties;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Represents an object in the BCD store.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(Name)}}} ({{{nameof(Type)}}})")]
    public sealed class BcdObject : IEnumerable<BcdElement> {

        //#region Public constructors
        ///// <summary>
        ///// Initialises a new instance.
        ///// </summary>
        ///// <param name="bcdObject">The type of the object to be created.</param>
        //public BcdObject(WellKnownBcdObject bcdObject) {
        //    this.Type = bcdObject.;
        //    //this.ID = type.Get
        //}
        //#endregion

        #region Public properties
        /// <summary>
        /// Gets or initialises the unique ID of the object.
        /// </summary>
        public Guid ID { get; init; }

        /// <summary>
        /// Gets a human-readable name of the object.
        /// </summary>
        public string Name => this.Type.GetNames().FirstOrDefault()
            ?? this.Type.ToString();

        /// <summary>
        /// Gets or initialises the type of the object.
        /// </summary>
        public BcdObjectType Type { get; init; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IEnumerator<BcdElement> GetEnumerator()
            => this._elements.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
            => this._elements.GetEnumerator();

        /// <inheritdoc />
        public override string ToString() => this.Name;
        #endregion

        #region Internal constructors
        /// <summary>
        /// Reconstructs an object from a registry key.
        /// </summary>
        /// <param name="key">The registry key representing the object.</param>
        [SupportedOSPlatform("windows")]
        internal BcdObject(RegistryKey key) {
            ArgumentNullException.ThrowIfNull(key);
            this.ID = Guid.Parse(Path.GetFileName(key.Name));

            using var desc = key.OpenSubKey("Description", false);
            if (desc is null) {
                throw new ArgumentException(Errors.InvalidRegistryKey,
                    nameof(key));
            }

            var type = desc.GetValue("Type") as int?;
            if (type is null) {
                throw new ArgumentException(Errors.InvalidRegistryKey,
                    nameof(key));
            }

            this.Type = (BcdObjectType) type;

            using var elements = key.OpenSubKey("Elements", false);
            if (elements is null) {
                throw new ArgumentException(Errors.InvalidRegistryKey,
                    nameof(key));
            }

            foreach (var e in elements.GetSubKeyNames()) {
                using var element = elements.OpenSubKey(e, false);
                if (element is null) {
                    throw new ArgumentException(Errors.InvalidRegistryKey,
                        nameof(key));
                }

                this._elements.Add(new(element));
            }
        }
        #endregion

        #region Private fields
        private readonly List<BcdElement> _elements = new();
        #endregion
    }
}
