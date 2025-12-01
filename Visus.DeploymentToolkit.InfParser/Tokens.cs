// <copyright file="Tokens.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.CompilerServices;


namespace Visus.DeploymentToolkit.InfParser {

    /// <summary>
    /// Identifies tokens in an INF file.
    /// </summary>
    internal static class Tokens {

        /// <summary>
        /// Indicates whether the given character is the start of a comment.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is the start
        /// of a comment, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCommentStart(this char c) => (c == ';');

        /// <summary>
        /// Indicates whether the given character is a line-continuation token
        /// that causes the parser to continue reading the next line as part of
        /// the current one.
        /// </summary>
        /// <returns><see langword="true"/> if <paramref name="c"/> is the line
        /// continuator, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsContinuator(this char c) => (c == '\\');
        // See https://learn.microsoft.com/en-us/windows-hardware/drivers/install/general-syntax-rules-for-inf-files

        /// <summary>
        /// Indicates whether the given character is an equals sign.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is an equals
        /// sign, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEquals(this char c) => (c == '=');

        /// <summary>
        /// Indicates whether the given character is a line-break token.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is a line
        /// break, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsLineBreak(this char c) => (c == '\n');

        /// <summary>
        /// Indicates whether the given character is double quote.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is a double
        /// quote, <see langword="false" /> otherwise.</returns>
        internal static bool IsQuote(this char c) => (c == '"');

        /// <summary>
        /// Indicates whether the given character is the start of a section
        /// header.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is the start
        /// of a section header, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSectionBegin(this char c) => (c == '[');

        /// <summary>
        /// Indicates whether the given character is the end of a section
        /// header.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is the end
        /// of a section header, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSectionEnd(this char c) => (c == ']');

        /// <summary>
        /// Indicates whether the given character is the space character, but
        /// not any other white space.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is the space
        /// character, <see langword="false" /> otherwise.</returns>
        internal static bool IsSpace(this char c) => (c == ' ');

        /// <summary>
        /// Answer whether the current character is the beginning or end of a
        /// string token to be replaced from a string table.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is a string
        /// token quote, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsStringToken(this char c) => (c == '%');

        /// <summary>
        /// Indicates whether the given character is a comma separating multiple
        /// values.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is a comma,
        /// <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsValueSeparator(this char c) => IsEquals(',');

        /// <summary>
        /// Indicates whether the given character is a white-space token.
        /// </summary>
        /// <param name="c">The token to be tested.</param>
        /// <returns><see langword="true"/> if <paramref name="c"/> is a white
        /// space, <see langword="false" /> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);
    }
}
