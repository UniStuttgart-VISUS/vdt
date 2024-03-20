// <copyright file="ICommandBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Net;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A builder for <see cref="ICommand"/>s.
    /// </summary>
    public interface ICommandBuilder {

        #region Public methods
        /// <summary>
        /// Causes the command to run as the user of the calling process.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        ICommandBuilder AsCurrentUser();

        /// <summary>
        /// Sets the <see cref="NetworkCredential"/> of the user who is running
        /// the command.
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        ICommandBuilder AsUser(NetworkCredential? credential);

        /// <summary>
        /// Builds the command.
        /// </summary>
        /// <returns></returns>
        ICommand Build();

        /// <summary>
        /// Sets the <see cref="WorkingDirectory"/> for the command.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        ICommandBuilder InWorkingDirectory(string? workingDirectory);

        /// <summary>
        /// Sets the <see cref="Arguments"/> for the command.
        /// </summary>
        /// <remarks>
        /// This method will remove any previously set arguments.
        /// </remarks>
        /// <param name="arguments"></param>
        /// <returns></returns>
        ICommandBuilder WithArgumentList(params string[]? arguments);

        /// <summary>
        /// Sets the <see cref="Arguments"/> for the command.
        /// </summary>
        /// <remarks>
        /// This method will remove any previously set arguments.
        /// </remarks>
        /// <param name="arguments"></param>
        /// <returns></returns>
        ICommandBuilder WithArgumentList(IEnumerable<string>? arguments);

        /// <summary>
        /// Splits <paramref name="arguments"/> and sets the argument list.
        /// </summary>
        /// <remarks>
        /// This method will remove any previously set arguments.
        /// </remarks>
        /// <param name="arguments"></param>
        /// <returns></returns>
        ICommandBuilder WithArguments(string? arguments);
        #endregion

    }
}
