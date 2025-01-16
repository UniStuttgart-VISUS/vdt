// <copyright file="ConsoleInputService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <inheritdoc />
        public int Select(string? prompt, IEnumerable<string> values) {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            var options = values.Count();
            //Math.Floor(Math.Log10(options) + 1)

            {
                int i = 0;
                foreach (var v in values) {
                    Console.WriteLine($"[{i}] {v}");
                    ++i;
                }
            }

            while (options > 0) {
                if (prompt != null) {
                    Console.Write($"{prompt}: ");
                }

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) {
                    return 0;
                }

                if (int.TryParse(input, out var retval)
                        && (retval >= 0)
                        && (retval < options)) {
                    return retval;
                }
            }

            return -1;
        }
    }
}
