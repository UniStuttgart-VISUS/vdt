// <copyright file="KeyValueSection.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// The default kind of section, which is a collection key/value pairs.
    /// </summary>
    [DebuggerDisplay("[{Name}]")]
    public class KeyValueSection : ISection,
            IEnumerable<KeyValuePair<string, object>> {

        #region Public properties
        /// <summary>
        /// Gets the names of the values stored in this section.
        /// </summary>
        public IEnumerable<string> Fields => this.Values.Keys;

        /// <inheritdoc/>
        public string Name { get; internal set; }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => this.Values.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.Values.GetEnumerator();

        /// <summary>
        /// Returns whether the given <paramref name="key"/>. and if this is the
        /// case, also the associated value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object? value)
            => this.Values.TryGetValue(key, out value);
        #endregion

        #region Public indexers
        /// <summary>
        /// Returns, if existing, the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object? this[string key] {
            get => this.Values.TryGetValue(key, out var retval) ? retval : null;
        }
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal KeyValueSection(string name)
            => this.Name = name
            ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="name"></param>
        internal KeyValueSection(ReadOnlySpan<char> name)
            => this.Name = name.ToString();
        #endregion

        #region Internal properties
        /// <summary>
        /// Provides access to the key/value pairs stored in this section.
        /// </summary>
        internal IDictionary<string, object> Values {
            get;
        } = new Dictionary<string, object>();
        #endregion
    }
}
