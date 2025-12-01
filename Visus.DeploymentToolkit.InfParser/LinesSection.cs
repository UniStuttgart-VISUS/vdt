// <copyright file="LinesSection.cs" company="Visualisierungsinstitut der Universität Stuttgart">
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
    public class LinesSection : ISection, IEnumerable<string> {

        #region Public properties
        /// <inheritdoc/>
        public string Name { get; internal set; }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
            => this.Lines.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.Lines.GetEnumerator();
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal LinesSection(string name)
            => this.Name = name
            ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="name"></param>
        internal LinesSection(ReadOnlySpan<char> name)
            => this.Name = name.ToString();
        #endregion

        #region Internal properties
        /// <summary>
        /// Provides access to the editable list of lines in this section.
        /// </summary>
        internal List<string> Lines{ get; } = new();
        #endregion
    }
}
