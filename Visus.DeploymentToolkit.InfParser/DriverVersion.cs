// <copyright file="DriverVersion.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Globalization;
using System.Text;
using Visus.DeploymentToolkit.InfParser.Properties;


namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// Represents a driver version entry in an INF file.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="build"></param>
    /// <param name="revision"></param>
    public sealed class DriverVersion(DateTime date, ushort major,
            ushort minor, ushort build, ushort revision) {

        #region Public class methods
        /// <summary>
        /// Parses the driver version from a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static DriverVersion Parse(ReadOnlySpan<char> str) {
            if (TryParse(str, out var retval)) {
                return retval;
            } else {
                throw new FormatException(Resources.ErrorInvalidDriverVersion);
            }
        }

        /// <summary>
        /// Represents a driver version entry in an INF file.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="retval"></param>
        /// <returns></returns>
        public static bool TryParse(ReadOnlySpan<char> str,
                out DriverVersion retval) {
            retval = null!;

            // Find the split between date and version.
            var sep = str.IndexOf(',');
            if (sep < 0) {
                return false;
            }

            // Parse the date.
            if (!DateTime.TryParse(str.Slice(0, sep).Trim(),
                    CultureInfo.CreateSpecificCulture("en-US"),
                    out var date)) {
                return false;
            }

            // Split the parts of the version.
            str = str.Slice(sep + 1).Trim();
            var version = new Range[4];
            if (str.Split(version, '.', StringSplitOptions.RemoveEmptyEntries)
                    != version.Length) {
                return false;
            }

            retval = new DriverVersion(date,
                ushort.Parse(str[version[0]], CultureInfo.InvariantCulture),
                ushort.Parse(str[version[1]], CultureInfo.InvariantCulture),
                ushort.Parse(str[version[2]], CultureInfo.InvariantCulture),
                ushort.Parse(str[version[3]], CultureInfo.InvariantCulture));
            return true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the build number.
        /// </summary>
        public ushort Build { get; } = build;

        /// <summary>
        /// Gets the driver date.
        /// </summary>
        public DateTime Date { get; } = date.Date;

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        public ushort Major { get; } = major;

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public ushort Minor { get; } = minor;

        /// <summary>
        /// Gets the revision number.
        /// </summary>
        public ushort Revision { get; } = revision;
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public override string ToString() => $"{this.Date:MM/dd/yyyy}, "
            + $"{this.Major}.{this.Minor}.{this.Build}.{this.Revision}";
        #endregion
    }
}
