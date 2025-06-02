// <copyright file="BcdElement.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Bcd.Properties;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Represents a BCD element within a BCD object.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(Type)}}} = {{{nameof(Value)}}}")]
    public sealed class BcdElement {

        #region Public properties
        /// <summary>
        /// Gets the part of <see cref="Type"/> that describes the format of the
        /// <see cref="Value"/>.
        /// </summary>
        public BcdElementType Format => this.Type & BcdElementType.FormatMask;

        /// <summary>
        /// Gets a human-readable name of the element.
        /// </summary>
        public string Name => this.Type.GetNames().FirstOrDefault()
            ?? this.Type.ToString();

        /// <summary>
        /// Gets or initialises the type of the element.
        /// </summary>
        public BcdElementType Type { get; init; }

        /// <summary>
        /// Gets or initialises the value of the element.
        /// </summary>
        public object? Value { get; init; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override string ToString() => this.Name;
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="key"></param>
        [SupportedOSPlatform("windows")]
        internal BcdElement(RegistryKey key) {
            ArgumentNullException.ThrowIfNull(key);
            var type = int.Parse(Path.GetFileName(key.Name),
                NumberStyles.HexNumber);
            this.Type = (BcdElementType) type;

            this.Value = key.GetValue("Element")
                ?? throw new ArgumentException(Errors.InvalidRegistryKey,
                    nameof(key));

            // Post-process some values as necessary.
            switch (this.Format) {
                case BcdElementType.Boolean:
                    this.Value = (((byte[]) this.Value).Single() != 0);
                    break;

                case BcdElementType.Guid:
                    this.Value = Guid.Parse((string) this.Value);
                    break;

                case BcdElementType.GuidList:
                    this.Value = ((IEnumerable<string>) this.Value)
                        .Select(Guid.Parse);
                    break;

                case BcdElementType.IntegerList: {
                    // TODO: I am not sure whether this is the correct interpretation of the data ...
                    var input = (byte[]) this.Value;
                    Debug.Assert(input.Length % 4 == 0);

                    var output = new int[input.Length / 4];
                    this.Value = output;

                    for (int i = 0; i < output.Length; ++i) {
                        output[i] = BitConverter.ToInt32(input, 4 * i);
                    }
                    } break;
            }
        }
        #endregion
    }
}
