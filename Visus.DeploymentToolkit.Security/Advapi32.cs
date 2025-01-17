// <copyright file="Advapi32.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// Holds functions imported from advapi32.dll.
    /// </summary>
    public static class Advapi32 {

        /// <summary>
        /// Function enables or disables privileges in the specified access token.
        /// Enabling or disabling privileges in an access token requires
        /// <c>TOKEN_ADJUST_PRIVILEGES</c> access.
        /// </summary>
        /// <param name="tokenHandle"></param>
        /// <param name="disableAllPrivileges"></param>
        /// <param name="newState"></param>
        /// <param name="bufferLength"></param>
        /// <param name="previousState"></param>
        /// <param name="returnLength"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(
            nint tokenHandle,
            bool disableAllPrivileges,
            ref TokenPrivileges newState,
            uint bufferLength,
            nint previousState,
            ref int returnLength);

        public static bool AdjustTokenPrivileges(
                nint tokenHandle,
                LuidAndAttributes newState) {
            var privileges = new TokenPrivileges() {
                PrivilegeCount = 1,
                Privileges = newState
            };
            var returnedLength = 0;
            return AdjustTokenPrivileges(tokenHandle,
                false,
                ref privileges,
                0,
                nint.Zero,
                ref returnedLength);
        }

        /// <summary>
        /// Retrieves the locally unique identifier (<see cref="Luid">) used on
        /// a specified system to locally represent the specified privilege
        /// name.
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="name"></param>
        /// <param name="luid"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(
            string? systemName,
            string name,
            out Luid luid);

        /// <summary>
        /// Retrieves the locally unique identifier (<see cref="Luid">) used on
        /// a specified system to locally represent the specified privilege
        /// name.
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public static Luid LookupPrivilegeValue(
                string? systemName,
                string name) {
            if (LookupPrivilegeValue(systemName, name, out var retval)) {
                return retval;
            } else {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Retrieves the locally unique identifier (<see cref="Luid">) used on
        /// the current system system to locally represent the specified
        /// privilege name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public static Luid LookupPrivilegeValue(string name)
            => LookupPrivilegeValue(null, name);

        /// <summary>
        /// Opens the access token associated with a process.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="desiredAccess"></param>
        /// <param name="tokenHandle"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(
            nint processHandle,
            int desiredAccess,
            out nint tokenHandle);

        /// <summary>
        /// Opens the access token associated with a process.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="desiredAccess"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public static nint OpenProcessToken(
                nint processHandle,
                int desiredAccess) {
            if (OpenProcessToken(processHandle, desiredAccess, out var retval)) {
                return retval;
            } else {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Opens the access token associated with the current process.
        /// </summary>
        /// <param name="desiredAccess"></param>
        /// <returns></returns>
        public static nint OpenProcessToken(int desiredAccess) {
            return OpenProcessToken(
                Process.GetCurrentProcess().Handle,
                desiredAccess);
        }

    }
}
