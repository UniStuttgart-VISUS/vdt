// <copyright file="TokenPrivilege.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// This class implements a RAII scope for holding a token privilege.
    /// </summary>
    public sealed class TokenPrivilege : IDisposable {

        #region Public constructors
        /// <summary>
        /// Obtains the specified <paramref name="privilege"/> for the given
        /// process <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The process token to be adjusted. In case
        /// of success, the new object takes ownership of the token and will
        /// close it once it is disposed.</param>
        /// <param name="privilege">The name of the privilege to obtain.</param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="privilege"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">If the
        /// operation failed, which is typically because the caller was not
        /// elevated or lacks the requested privilege.</exception>
        public TokenPrivilege(nint token, string privilege) {
            ArgumentNullException.ThrowIfNull(privilege);
            Advapi32.AdjustTokenPrivileges(token, privilege, true);
            this._privilege = privilege;
            this._token = token;
        }

        /// <summary>
        /// Obtains the specified <paramref name="privilege"/> for the calling
        /// process.
        /// </summary>
        /// <param name="privilege">The name of the privilege to obtain.</param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="privilege"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">If the
        /// operation failed, which is typically because the caller was not
        /// elevated or lacks the requested privilege.</exception>
        public TokenPrivilege(string privilege) : this(Advapi32.OpenProcessToken(
            TokenAccess.AdjustPrivileges | TokenAccess.Query), privilege) { }
        #endregion

        #region Finaliser
        /// <summary>
        /// Finalises the instance.
        /// </summary>
        ~TokenPrivilege() => this.Dispose(false);
        #endregion

        #region Public methods
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Removes the configured privileges and closes the token.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing) {
            if (this._privilege is not null) {
                try {
                    Advapi32.AdjustTokenPrivileges(this._token,
                        this._privilege,
                        false);
                } finally {
                    Kernel32.CloseHandle(this._token);
                    this._privilege = null;
                }
            }
        }
        #endregion

        #region Private fields
        private string? _privilege;
        private nint _token;
        #endregion
    }
}
