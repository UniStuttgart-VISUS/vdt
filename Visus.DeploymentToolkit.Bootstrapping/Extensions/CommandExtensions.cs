// <copyright file="CommandExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for <see cref="ICommands"/>.
    /// </summary>
    public static class CommandExtensions {

        /// <summary>
        /// Executes <paramref name="that"/> and checks that the exit code is
        /// <paramref name="success"/>.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="success"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task<int?> ExecuteAndCheckAsync(
                this ICommand that,
                int success,
                ILogger? logger) {
            ArgumentNullException.ThrowIfNull(that);

            var retval = await that.ExecuteAsync().ConfigureAwait(false);
            logger?.LogTrace("Command \"{Command}\" exited with return "
                + "value {ExitCode}.", that.ToString(), retval);

            if (retval == null) {
                throw new InvalidOperationException(string.Format(
                    Errors.CommandNotRun, that.ToString()));
            }

            if (retval != success) {
                logger?.LogError("The command \"{Command}\" returned the "
                    + "exit code {ExitCode}, which does not indicate "
                    + "success.", that.ToString(), retval.Value);
                throw new CommandFailedException(that.ToString()!,
                    retval.Value);
            }

            return retval;
        }

        /// <summary>
        /// Executes <paramref name="that"/> and checks that the exit code is
        /// one of <paramref name="successes"/> or none of
        /// <paramref name="failures"/>.
        /// </summary>
        /// <remarks>
        /// If both, <paramref name="successes"/> and
        /// <paramref name="failures"/> are specified, the behaviour is as if
        /// only <paramref name="successes"/> had been specified.
        /// </remarks>
        /// <param name="that"></param>
        /// <param name="successes"></param>
        /// <param name="failures"></param>
        /// <returns></returns>
        public static async Task<int?> ExecuteAndCheckAsync(
                this ICommand that,
                IEnumerable<int>? successes,
                IEnumerable<int>? failures,
                ILogger logger) {
            ArgumentNullException.ThrowIfNull(that);
            ArgumentNullException.ThrowIfNull(logger);

            var retval = await that.ExecuteAsync().ConfigureAwait(false);
            logger?.LogTrace("Command \"{Command}\" exited with return "
                + "value {ExitCode}.", that.ToString(), retval);

            if (retval == null) {
                throw new InvalidOperationException(string.Format(
                    Errors.CommandNotRun, that.ToString()));
            }

            if (successes?.Any() == true && !successes.Contains(retval.Value)) {
                logger?.LogError("The command \"{Command}\" returned the "
                    + "exit code {ExitCode}, which does not indicate "
                    + "success.", that.ToString(), retval.Value);
                throw new CommandFailedException(that.ToString()!,
                    retval.Value);
            }

            if (failures?.Any() == true && failures.Contains(retval.Value)) {
                logger?.LogError("Command \"{Command}\" returned the "
                    + "exit code {ExitCode}, which indicates failure.",
                    that.ToString(), retval.Value);
                throw new CommandFailedException(that.ToString()!,
                    retval.Value);
            }

            return retval;
        }

    }
}
