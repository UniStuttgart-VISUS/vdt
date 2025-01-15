// <copyright file="IConsoleInput.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Security;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service for reading user input from the console.
    /// </summary>
    public interface IConsoleInput {

        /// <summary>
        /// Reads a line from the console.
        /// </summary>
        /// <param name="prompt">The prompt printed on the console. If the
        /// prompt is <c>null</c>, nothing is printed.</param>
        /// <param name="defaultValue">The default value that is returned if the
        /// user entered an empty string.</param>
        /// <returns></returns>
        string? ReadInput(string? prompt, string? defaultValue);

        /// <summary>
        /// Reads a password from the console.
        /// </summary>
        /// <param name="prompt">The prompt printed on the console. If the
        /// prompt is <c>null</c>, nothing is printed.</param>
        /// <returns>The password read from the user.</returns>
        SecureString ReadPassword(string? prompt);

        /// <summary>
        /// Prompt the user to select one of the specified values and answers the
        /// selected index.
        /// </summary>
        /// <param name="prompt">The prompt printed on the console. If the
        /// prompt is <c>null</c>, nothing is printed.</param>
        /// <param name="values"></param>
        /// <returns>The zero-based selection made by the user or a negative
        /// value if no <paramref name="values"/> have been provided.</returns>
        int Select(string? prompt, IEnumerable<string> values);
    }
}
