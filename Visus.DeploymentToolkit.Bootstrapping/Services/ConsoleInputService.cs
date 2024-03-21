// <copyright file="ConsoleInputService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Security;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the console input service.
    /// </summary>
    internal sealed class ConsoleInputService : IConsoleInput {

        /// <inheritdoc />
        public string? ReadInput(string? prompt, string? defaultValue) {
            if ((prompt != null) && (defaultValue != null)) {
                Console.Write($"{prompt} [{defaultValue}]: ");
            } else if (prompt != null) {
                Console.Write($"{prompt}: ");
            }

            var retval = Console.ReadLine();

            if (string.IsNullOrEmpty(retval) && (defaultValue != null)) {
                retval = defaultValue;
            }

            return retval;
        }

        /// <inheritdoc />
        public SecureString ReadPassword(string? prompt) {
            if (prompt != null) {
                Console.Write($"{prompt}: ");
            }

            ConsoleKeyInfo key;
            SecureString retval = new();

            do {
                key = Console.ReadKey(true);

                if ((key.Key == ConsoleKey.Backspace) && (retval.Length > 0)) {
                    Console.Write("\b \b");
                    retval.RemoveAt(retval.Length - 1);

                } else if (!char.IsControl(key.KeyChar)) {
                    Console.Write("*");
                    retval.AppendChar(key.KeyChar);
                }

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return retval;
        }
    }
}
