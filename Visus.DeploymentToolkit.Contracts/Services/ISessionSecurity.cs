// <copyright file="ISessionSecurity.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Text;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service that can be used to secure sensitive data on a per-workflow
    /// basis.
    /// </summary>
    public interface ISessionSecurity : IDisposable {

        /// <summary>
        /// Decrypts the given data with the configures session key.
        /// </summary>
        /// <param name="data">The base64-encoded encrypted data.</param>
        /// <returns>The decrypted data.</returns>
        byte[] Decrypt(string data);

        /// <summary>
        /// Decrypts the given string data with the configures session key.
        /// </summary>
        /// <param name="data">The base64-encoded encrypted data.</param>
        /// <returns>The decrypted string.</returns>
        string DecryptString(string data) {
            return Encoding.Unicode.GetString(this.Decrypt(data));
        }

        /// <summary>
        /// Encrypts the given data with the configured session key.
        /// </summary>
        /// <param name="data">The data to be encrytped.</param>
        /// <returns>The base64-encoded encrypted data.</returns>
        string Encrypt(ReadOnlySpan<byte> data);

        /// <summary>
        /// Encrypts the given string with the configured session key.
        /// </summary>
        /// <param name="data">The string to be encrypted.</param>
        /// <returns>The base64-encoded encrypted string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="data"/>
        /// is <c>null</c>.</exception>
        string Encrypt(string data) {
            return this.Encrypt(Encoding.Unicode.GetBytes(data));
        }

        /// <summary>
        /// Encrypts the given string with the configured session key.
        /// </summary>
        /// <param name="data">The string to be encrypted.</param>
        /// <returns>The base64-encoded encrypted string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="data"/>
        /// is <c>null</c>.</exception>
        string EncryptString(string data) => this.Encrypt(data);
    }
}
