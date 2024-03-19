// <copyright file="DismCapabilityInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies a date and time, using individual members for the month, day,
    /// year, weekday, hour, minute, second, and millisecond. The time is either
    /// in coordinated universal time (UTC) or local time, depending on the
    /// function that is being called.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime {

        /// <summary>
        /// Convert <see cref="SystemTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="that">The <see cref="SystemTime"/> to be converted.
        /// </param>
        public static implicit operator DateTime(SystemTime that)
            => new DateTime(that.Year, that.Month, that.Day,
                that.Hour, that.Minute, that.Second, that.Millisecond);

        /// <summary>
        /// Convert <see cref="DateTime"/> to <see cref="SystemTime"/>.
        /// </summary>
        /// <param name="that"></param>
        public static implicit operator SystemTime(DateTime that) => new SystemTime() {
            Year = (ushort) that.Year,
            Month = (ushort) that.Month,
            Day = (ushort) that.Day,
            Hour = (ushort) that.Hour,
            Minute = (ushort) that.Minute,
            Second = (ushort) that.Second,
            Millisecond = (ushort) that.Millisecond,
            DayOfWeek = (ushort) that.DayOfWeek
        };

        /// <summary>
        /// The year. The valid values for this member are 1601 through 30827.
        /// </summary>
        public ushort Year;

        /// <summary>
        /// The month.
        /// </summary>
        public ushort Month;

        /// <summary>
        /// The day of the week.
        /// </summary>
        public ushort DayOfWeek;

        /// <summary>
        /// The day of the month. The valid values for this member are 1
        /// through 31.
        /// </summary>
        public ushort Day;

        /// <summary>
        /// The hour. The valid values for this member are 0 through 23.
        /// </summary>
        public ushort Hour;

        /// <summary>
        /// The minute. The valid values for this member are 0 through 59.
        /// </summary>
        public ushort Minute;

        /// <summary>
        /// The second. The valid values for this member are 0 through 59.
        /// </summary>
        public ushort Second;

        /// <summary>
        /// The millisecond. The valid values for this member are 0 through 999.
        /// </summary>
        public ushort Millisecond;
    }
}
