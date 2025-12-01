// <copyright file="Parser.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Visus.DeploymentToolkit.InfParser.Properties;


namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// Parses the contents of an INF file.
    /// </summary>
    public static class Parser {

        #region Public methods
        /// <summary>
        /// Reads the given file and parses it as an INF file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDictionary<string, ISection> ParseFile(
                string path) {
            var fileContents = File.ReadAllText(path);
            return ParseString(fileContents.AsSpan());
        }

        /// <summary>
        /// Parses the given string as the contents of an INF file.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IDictionary<string, ISection> ParseString(
                ReadOnlySpan<char> str) {
            var retval = new Dictionary<string, ISection>();

            while (!str.IsEmpty) {
                if (str[0].IsCommentStart()) {
                    str = ParseComment(str);
                    continue;
                }

                if (str[0].IsSectionBegin()) {
                    str = ParseSection(str, out var section);
                    retval.Add(section.Name, section);
                    continue;
                }

                str = ParseWhiteSpace(str);
            }

            return retval;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Parse a comment by skipping ahead to the next line.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static ReadOnlySpan<char> ParseComment(ReadOnlySpan<char> str) {
            Debug.Assert(!str.IsEmpty);
            Debug.Assert(str[0].IsCommentStart());

            int i = 0;
            while ((i < str.Length) && !str[i].IsLineBreak()) {
                ++i;
            }

            if (i < str.Length) {
                Debug.Assert(str[i].IsLineBreak());
                ++i;
            }

            return str.Slice(i);
        }

        /// <summary>
        /// Parses a secttion comprising key/value pairs.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="name"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private static ReadOnlySpan<char> ParseKeyValueSection(
                ReadOnlySpan<char> str,
                ReadOnlySpan<char> name,
                out KeyValueSection section) {
            Debug.Assert(!str.IsEmpty);
            section = new KeyValueSection(name);

            while (!str.IsEmpty) {
                if (str[0].IsCommentStart()) {
                    // Make sure to consume all comments first.
                    str = ParseComment(str);
                    continue;
                }

                if (str[0].IsSectionBegin()) {
                    // Next section begins here, so we are done with this one.
                    break;
                }

                // Try parsing a key/value pair from the line.
                str = ParseLine(str, out var key, out var value);
                if (!key.IsEmpty) {
                    section.Values.Add(key.ToString(), value.ToString());
                }
            }

            return str;
        }

        /// <summary>
        /// Parse a single line, discarding any comments.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static ReadOnlySpan<char> ParseLine(
                ReadOnlySpan<char> str,
                out ReadOnlySpan<char> line) {
            Debug.Assert(!str.IsEmpty);

            int i = 0;
            while (i < str.Length) {
                if (str[i].IsCommentStart()) {
                    // Emit the line up to the comment and skip the rest.
                    line = str.Slice(0, i);
                    return str.Slice(i);
                }

                if (str[i].IsLineBreak()) {
                    // This is a non-quoted line break, so we emit the line.
                    line = str.Slice(0, i);
                    if (i < str.Length) {
                        Debug.Assert(str[i].IsLineBreak());
                        ++i;
                    }
                    return str.Slice(i);
                }

                if (str[i].IsQuote()) {
                    // Skip ahead to the end of the quoted string.
                    i += ParseQuotedString(str.Slice(i));
                    continue;
                }

                ++i;
            }

            line = str.Slice(i, 0);
            return str;
        }

        /// <summary>
        /// Parses a single line into a key/value pair.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ReadOnlySpan<char> ParseLine(
                ReadOnlySpan<char> str,
                out ReadOnlySpan<char> key,
                out ReadOnlySpan<char> value) {
            Debug.Assert(!str.IsEmpty);
            key = value = default;

            int i = 0;
            int v = 0;
            while (i < str.Length) {
                if (str[i].IsCommentStart()) {
                    // Emit the line up to the comment and skip the rest.
                    Debug.Assert(v <= i);
                    value = str.Slice(v, i - v).Trim();
                    return str.Slice(i);
                }

                if (str[i].IsLineBreak()) {
                    // This is a non-quoted line break, so we emit the line.
                    Debug.Assert(v <= i);
                    value = str.Slice(v, i - v).Trim();
                    if (i < str.Length) {
                        Debug.Assert(str[i].IsLineBreak());
                        ++i;
                    }
                    return str.Slice(i);
                }

                if (str[i].IsQuote()) {
                    // Skip ahead to the end of the quoted string.
                    i += ParseQuotedString(str.Slice(i));
                    continue;
                }

                if (str[i].IsEquals() && key.IsEmpty) {
                    // Found the separator between key and value.
                    key = str.Slice(0, i).Trim();
                    if (i < str.Length) {
                        Debug.Assert(str[i].IsEquals());
                        ++i;
                    }
                    v = i;
                }

                ++i;
            }

            return str;
        }

        /// <summary>
        /// Parses a section of non-empty lines.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="name"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private static ReadOnlySpan<char> ParseLinesSection(
                ReadOnlySpan<char> str,
                ReadOnlySpan<char> name,
                out LinesSection section) {
            Debug.Assert(!str.IsEmpty);
            section = new LinesSection(name);

            while (!str.IsEmpty) {
                if (str[0].IsCommentStart()) {
                    // Make sure to consume all comments first.
                    str = ParseComment(str);
                    continue;
                }

                if (str[0].IsSectionBegin()) {
                    // Next section begins here, so we are done with this one.
                    break;
                }

                // Search for the end of the line and emit it if non-empty.
                str = ParseLine(str, out var line);
                line = line.Trim();
                if (!line.IsEmpty) {
                    section.Lines.Add(line.ToString());
                }
            }

            return str;
        }

        /// <summary>
        /// Determines the end of a quoted string starting at the beginning of
        /// <paramref name="str"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private static int ParseQuotedString(ReadOnlySpan<char> str) {
            Debug.Assert(!str.IsEmpty);
            Debug.Assert(str[0].IsQuote());

            int i = 1;
            while (i < str.Length) {
                if (str[i++].IsQuote()) {
                    if (!str.IsInRange(i) || !str[i].IsQuote()) {
                        // Found the end of the quoted string.
                        return i;
                    }
                }
            }

            throw new FormatException(Resources.ErrorUnterminatedQuotedString);
        }

        /// <summary>
        /// Parses a section starting with its header.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private static ReadOnlySpan<char> ParseSection(ReadOnlySpan<char> str,
                out ISection section) {
            Debug.Assert(!str.IsEmpty);
            Debug.Assert(str[0].IsSectionBegin());

            int i = 1;
            while ((i < str.Length) && !str[i].IsSectionEnd()) {
                ++i;
            }

            if (i >= str.Length) {
                throw new FormatException(Resources.ErrorUnterminatedHeader);
            }

            // Remember the name of the section.
            var name = str.Slice(1, i - 1);
            Debug.WriteLine($"Found section \"{name}\".");
            if (++i >= str.Length) {
                throw new FormatException(Resources.ErrorEmptySection);
            }

            if (name.EqualsIgnoreCase(WellKnownSections.Version)) {
                var retval = ParseKeyValueSection(str.Slice(i),
                    WellKnownSections.Version, out var kvs);

                // Perform some post-processing of known keys.
                object? value;
                if (kvs.Values.TryGetValue("DriverVer", out value)) {
                    if (DriverVersion.TryParse(value as string, out var v)) {
                        kvs.Values["DriverVer"] = v;
                    }
                }

                if (kvs.Values.TryGetValue("ClassGuid", out value)) {
                    if (Guid.TryParse(value as string, out var v)) {
                        kvs.Values["ClassGuid"] = v;
                    }
                }

                section = kvs;
                return retval;

            } else {
                var retval = ParseLinesSection(str.Slice(i), name, out var ls);
                section = ls;
                return retval;
            }
        }

        /// <summary>
        /// Parse all white space between sections by discarding it.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ReadOnlySpan<char> ParseWhiteSpace(
                ReadOnlySpan<char> str) {
            Debug.Assert(!str.IsEmpty);
            Debug.Assert(str[0].IsWhiteSpace());

            int i = 0;
            while ((i < str.Length) && str[i].IsWhiteSpace()) {
                ++i;
            }

            return str.Slice(i);
        }

        #endregion
    }
}
