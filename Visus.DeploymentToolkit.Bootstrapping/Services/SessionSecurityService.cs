// <copyright file="SessionSecurityService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides per-session encryption.
    /// </summary>
    internal sealed class SessionSecurityService : ISessionSecurity {

        #region Public constructors
        public SessionSecurityService(IState state,
                ILogger<SessionSecurityService> logger) {
            this._state = state
                ?? throw new ArgumentNullException(nameof(state));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Finaliser
        ~SessionSecurityService() {
            this.Dispose();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the hash algorithm that is used to derive
        /// the session key from the user input.
        /// </summary>
        public HashAlgorithmName HashAlgorithm {
            get;
            set;
        } = HashAlgorithmName.SHA256;

        /// <summary>
        /// Gets or sets the number of iterations used to derive the session key
        /// from the user input. This should be at least 10000.
        /// </summary>
        public int Iterations { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the number of characters in a randomly generated
        /// password.
        /// </summary>
        public int PasswordLength { get; set; } = 32;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public byte[] Decrypt(string data) {
            using var aes = this.GetAes();
            using var ms = new MemoryStream(Convert.FromBase64String(data));
            using var cs = new CryptoStream(ms,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            using var retval = new MemoryStream();
            cs.CopyTo(retval);
            return retval.ToArray();
        }

        /// <inheritdoc />
        public void Dispose() {
            // Make sure that the key is overwritten.
            if (this._iv != null) {
                this._logger.LogTrace("Disposing the initialisation vector.");
                Array.Fill(this._iv, (byte) 0);
                this._iv = null;
            }

            if (this._key != null) {
                this._logger.LogTrace("Disposing the session key.");
                Array.Fill(this._key, (byte) 0);
                this._key = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public string Encrypt(ReadOnlySpan<byte> data) {
            using var aes = this.GetAes();
            using var ms = new MemoryStream();

            using (var cs = new CryptoStream(ms,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write)) {
                cs.Write(data);
            }

            return Convert.ToBase64String(ms.ToArray());
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Create an <see cref="Aes"/> instance and configure it with
        /// <see cref="_key"/> and <see cref="_iv"/>, which are both created
        /// as necessary.
        /// </summary>
        private Aes GetAes() {
            var retval = Aes.Create();

            if ((this._iv == null) || (this._key == null)) {
                if (string.IsNullOrEmpty(this._state.SessionKey)) {
                    this._logger.LogInformation("Generating session key for "
                        + "encrypting sensitive state.");
                    var chars = (from i in Enumerable.Range(0, 1 << 8)
                                 let c = (char) i
                                 where !char.IsControl(c)
                                 select c).ToArray();
                    this._state.SessionKey = RandomNumberGenerator.GetString(
                        chars, this.PasswordLength);
                }

                this._logger.LogTrace("Creating salt for deriving the actual "
                    + "session key from the session password.");
                var salt = Encoding.UTF8.GetBytes(this.GetType().Name);
                salt = Rfc2898DeriveBytes.Pbkdf2(
                    this._state.SessionKey,
                    salt,
                    this.Iterations,
                    this.HashAlgorithm,
                    retval.Key.Length);

                this._logger.LogTrace("Deriving the session key.");
                salt = this._key = Rfc2898DeriveBytes.Pbkdf2(
                    this._state.SessionKey,
                    salt,
                    this.Iterations,
                    this.HashAlgorithm,
                    retval.Key.Length);

                this._logger.LogTrace("Deriving the initialisation vector.");
                this._iv = Rfc2898DeriveBytes.Pbkdf2(
                    this._state.SessionKey,
                    salt,
                    this.Iterations,
                    this.HashAlgorithm,
                    retval.IV.Length);
            }

            retval.Key = this._key;
            retval.IV = this._iv;

            return retval;
        }
        #endregion

        #region Private fields
        private byte[]? _iv;
        private byte[]? _key;
        private readonly ILogger _logger;
        private readonly IState _state;
        #endregion
    }

}
