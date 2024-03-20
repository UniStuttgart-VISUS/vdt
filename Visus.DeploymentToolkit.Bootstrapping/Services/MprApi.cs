// <copyright file="MprApi.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using System;
using System.Runtime.Versioning;
using System.Linq;
using System.ComponentModel;
using System.Net;
using System.Diagnostics;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to the native APIs for mapping a network drive.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal static class MprApi {

        #region Nested types
        [Flags]
        public enum ConnectionFlags : uint {

            /// <summary>
            /// No flags are set.
            /// </summary>
            None = 0x00000000,

            /// <summary>
            /// The network resource connection should be remembered. If this
            /// bit flag is set, the operating system automatically attempts
            /// to restore the connection when the user logs on.
            /// </summary>
            UpdateProfile = 0x00000001,

            /// <summary>
            /// The network resource connection should not be put in the recent
            /// connection list. If this flag is set and the connection is
            /// successfully added, the network resource connection will be put
            /// in the recent connection list only if it has a redirected local
            /// device associated with it.
            /// </summary>
            UpdateRecent = 0x00000002,

            /// <summary>
            /// The network resource connection should not be remembered.
            /// </summary>
            Temporary = 0x00000004,

            /// <summary>
            /// If this flag is set, the operating system may interact with the
            /// user for authentication purposes.
            /// </summary>
            Interactive = 0x00000008,

            /// <summary>
            /// This flag instructs the system not to use any default settings
            /// for user names or passwords without offering the user the
            /// opportunity to supply an alternative. This flag is ignored
            /// unless <see cref="Interactive"/> is also set.
            /// </summary>
            Prompt = 0x00000010,

            /// <summary>
            /// This flag forces the redirection of a local device when making
            /// the connection.
            /// </summary>
            Redirect = 0x00000080,

            /// <summary>
            /// If this flag is set, then the operating system does not start to
            /// use a new media to try to establish the connection (initiate a
            /// new dial up connection, for example).
            /// </summary>
            CurrentMedia = 0x00000200,

            /// <summary>
            /// If this flag is set, the operating system prompts the user for
            /// authentication using the command line instead of a graphical
            /// user interface (GUI). This flag is ignored unless
            /// <see cref="Interactive"/> is also set.
            /// </summary>
            CommandLine = 0x00000800,

            /// <summary>
            /// If this flag is set, and the operating system prompts for a
            /// credential, the credential should be saved by the credential
            /// manager. If the credential manager is disabled for the caller's
            /// logon session, or if the network provider does not support
            /// saving credentials, this flag is ignored. This flag is ignored
            /// unless <see cref="Interactive"/> is also set. This flag is also
            /// ignored unless you set the <see cref="CommandLine"/> flag.
            /// </summary>
            SaveCredential = 0x00001000,

            /// <summary>
            /// If this flag is set, and the operating system prompts for a
            /// credential, the credential is reset by the credential manager.
            /// If the credential manager is disabled for the caller's logon
            /// session, or if the network provider does not support saving
            /// credentials, this flag is ignored. This flag is also ignored
            /// unless you set the <see cref="CommandLine"/> flag.
            /// </summary>
            ResetCredential = 0x00002000
        }

        public enum DisplayType : uint {
            Generic = 0x00000000,
            Domain = 0x00000001,
            Server = 0x00000002,
            Share = 0x00000003,
            File = 0x00000004,
            Group = 0x00000005,
            Network = 0x00000006,
            Root = 0x00000007,
            ShareAdmin = 0x00000008,
            Directory = 0x00000009,
            Tree = 0x0000000A,
            NdsContainer = 0x0000000B
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Resource {
            public Scope Scope;
            public Type Type;
            public DisplayType DisplayType;
            public Usage Usage;
            public string LocalName;
            public string RemoteName;
            public string Comments;
            public string Provider;
        }

        public enum Scope : uint {

            /// <summary>
            /// Enumerate all currently connected resources.
            /// </summary>
            Connected = 0x00000001,

            /// <summary>
            /// Enumerate all resources on the network.
            /// </summary>
            GlobalNetwork = 0x00000002,

            /// <summary>
            /// Enumerate all remembered (persistent) connections
            /// </summary>
            Remembered = 0x00000003,

            Recent = 0x00000004,

            /// <summary>
            /// Enumerate only resources in the network context of the caller.
            /// Specify this value for a Network Neighborhood view.
            /// </summary>
            Context = 0x00000005
        }

        public enum Type : uint {
            Any = 0x00000000,
            Disk = 0x00000001,
            Printer = 0x00000002,
            Reserved = 0x00000008,
            Unknown = 0xFFFFFFFF
        }

        public enum Usage : uint {
            Connectable = 0x00000001,
            Container = 0x00000002,
            NoLocalDevice = 0x00000004,
            Subling = 0x00000008,
            Attached = 0x00000010,
            All = Connectable | Container| Attached,
            Reserved = 0x80000000
        }
        #endregion

        #region Public constants
        /// <summary>
        /// The name of the library we are invoking from.
        /// </summary>
        public const string LibraryName = "mpr.dll";
        #endregion

        #region Public methods
        /// <summary>
        /// Maps the given network share to the given drive letter.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Connect(string letter,
                string path,
                string? userName = null,
                string? password = null,
                ConnectionFlags flags = ConnectionFlags.None) {
            _ = letter ?? throw new ArgumentNullException(nameof(letter));
            _ = path ?? throw new ArgumentNullException(nameof(path));

            if (path.Last() == '\\') {
                path = path.Substring(0, path.Length - 1);
            }

            var res = new Resource() {
                Type = Type.Disk,
                LocalName = CanonicaliseLetter(letter),
                RemoteName = path
            };

            var error = WNetAddConnection2(ref res, password, userName, flags);
            if (error != 0) {
                throw new Win32Exception(error);
            }
        }

        /// <summary>
        /// Maps the given network share to the given drive letter.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="path"></param>
        /// <param name="credential"></param>
        /// <param name="flags"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Connect(string letter,
                string path,
                NetworkCredential? credential,
                ConnectionFlags flags = ConnectionFlags.None) {
            var user = credential?.UserName;
            var password = credential?.Password;

            if ((user != null) && (credential?.Domain != null)) {
                user = $@"{credential.Domain}\{user}";
            }

            Connect(letter, path, user, password, flags);
        }

        /// <summary>
        /// Disconnects a network drive mapped to <paramref name="letter"/>.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="flags"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Disconnect(string letter,
                ConnectionFlags flags = ConnectionFlags.None,
                bool force = false) {
            _ = letter ?? throw new ArgumentNullException(nameof(letter));

            if (letter.Last() == '\\') {
                letter = letter.Substring(0, letter.Length - 1);
            }

            letter = CanonicaliseLetter(letter);
            var error = WNetCancelConnection2(letter, flags, force);
            if (error != 0) {
                throw new Win32Exception(error);
            }
        }

        /// <summary>
        /// Disconnects a network drive mapped to <paramref name="letter"/>.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="force"></param>
        public static void Disconnect(string letter, bool force)
            => Disconnect(letter, ConnectionFlags.None, force);

        /// <summary>
        /// This function makes a connection to a network resource and can
        /// redirect a local device to the network resource.
        /// </summary>
        /// <param name="networkResource"></param>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern int WNetAddConnection2(
            ref Resource networkResource,
            string? password,
            string? userName,
            ConnectionFlags flags);

        /// <summary>
        /// This function cancels an existing network connection. You can also
        /// call the function to remove remembered network connections that are
        /// not currently connected.
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="flags"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern int WNetCancelConnection2(string localName,
            ConnectionFlags flags,
            bool force);
        #endregion

        #region Private methods
        private static string CanonicaliseLetter(string letter) {
            Debug.Assert(letter != null);
            if (letter.Last() == '\\') {
                letter = letter.Substring(0, letter.Length - 1);
            }

            if (letter.Last() != ':') {
                letter += ':';
            }

            return letter;
        }
        #endregion
    }
}
