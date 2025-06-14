// <copyright file="NetApi.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Xml.Linq;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to the native APIs for creating SMB shares.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal static class NetApi {

        #region Nested types
        /// <summary>
        /// Possible values for <see cref="SHARE_INFO_2.shi2_permissions" />.
        /// </summary>
        [Flags]
        public enum Access : uint {
            None = 0x00,
            Read = 0x01,
            Write = 0x02,
            Create = 0x04,
            Execute = 0x08,
            Delete = 0x10,
            Attribute = 0x20,
            Permission = 0x40,
            All = Read
                | Write
                | Create
                | Execute
                | Delete
                | Attribute
                | Permission
        }

        /// <summary>
        /// Constants from lm.h that specify the type of shared resource.
        /// </summary>
        [Flags]
        public enum ShareType : uint {
            DiskTree = 0,
            PrintQueue = 1,
            Device = 2,
            Ipc = 3,

            /// <summary>
            /// Bitwise AND with <see cref="SHARE_INFO_2.shi2_type"/> to get
            /// the type.
            /// </summary>
            Mask = 0x000000FF,

            Temporary = 0x40000000,
            Special = 0x80000000,
            UsesUnlimited = 0xFFFFFFFF
        }

        /// <summary>
        /// The most basic strucuture describing a shared resource which should
        /// be available on all supported versions of Windows.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHARE_INFO_2 {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_netname;
            public ShareType shi2_type;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_remark;
            public Access shi2_permissions;
            public uint shi2_max_uses;
            public uint shi2_current_uses;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_path;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string shi2_passwd;
        }
        #endregion

        #region Public constants
        /// <summary>
        /// The name of the library we are invoking from.
        /// </summary>
        public const string LibraryName = "Netapi32.dll";
        #endregion

        #region Public methods
        /// <summary>
        /// Joins a computer to a workgroup or domain.
        /// </summary>
        /// <param name="server">A pointer to a constant string that specifies
        /// the DNS or NetBIOS name of the computer on which to execute the
        /// domain join operation. If this parameter is <see langword="null"/>,
        /// the local computer is used.</param>
        /// <param name="domain">A pointer to a constant null-terminated
        /// character string that specifies the name of the domain or workgroup
        /// to join.</param>
        /// <param name="organisationalUnit">Optionally specifies the pointer to
        /// a constant null-terminated character string that contains the
        /// RFC-1779 format name of the organisational unit (OU) for the
        /// computer account. If you specify this parameter, the string must
        /// contain a full path lik OU=testOU,DC=domain,DC=Domain,DC=com.
        /// Otherwise, this parameter must be <see langword="null"/>.</param>
        /// <param name="account">A pointer to a constant null-terminated
        /// character string that specifies the account name to use when
        /// connecting to the domain controller. The string must specify either
        /// a domain NetBIOS name and user account (for example, REDMOND\user)
        /// or the user principal name (UPN) of the user in the form of an
        /// Internet-style login name (&quot;user@remond.com&quot;). If this
        /// parameter is <see langword="null"/>, the caller's context is used.
        /// </param>
        /// <param name="password">If the <paramref name="account"/>  parameter
        /// specifies an account name, this parameter must point to the
        /// password to use when connecting to the domain controller. Otherwise,
        /// this parameter must be <see langword="null"/>. You can specify a
        /// local machine account password rather than a user password for
        /// unsecured joins.</param>
        /// <param name="joinOptions">A set of bit flags defining the join
        /// options.</param>
        /// <returns>Zero in case of success, an error code otherwise.</returns>
        [DllImport(LibraryName)]
        public static extern int NetJoinDomain(string? server, string domain,
          string? organisationalUnit, string? account, string? password,
          JoinOptions joinOptions);

        /// <summary>
        /// Shares a server resource.
        /// </summary>
        /// <param name="server">The name of the server to share the resource,
        /// or <see langword="null"/> for the local machine.</param>
        /// <param name="level">Specifies the information level of the data.
        /// This <i>must</i> be 2 for this overload.</param>
        /// <param name="buffer">A <see cref="SHARE_INFO_2"/> structure
        /// describing the shared resource.</param>
        /// <param name="invalidParamIndex">Receives the index of the first
        /// member of the share information structure that causes an invalid
        /// parameter error. </param>
        /// <returns>0 in case of success, a Windows error code otherwise.
        /// </returns>
        [DllImport(LibraryName)]
        public static extern int NetShareAdd(string? server, int level,
            ref SHARE_INFO_2 buffer, out int invalidParamIndex);

        /// <summary>
        /// Deletes a share name from a server's list of shared resources, which
        /// disconnects all connections to that share.
        /// </summary>
        /// <param name="server">The name of the server to share the resource,
        /// or <c>null</c> for the local machine.</param>
        /// <param name="share">The name of the share to be deleted.</param>
        /// <returns>0 in case of success, a Windows error code otherwise.
        /// </returns>
        [DllImport(LibraryName)]
        public static extern int NetShareDel(string? server, string share);

        /// <summary>
        /// Shares a server resource.
        /// </summary>
        /// <param name="server">The name of the server to share the resource,
        /// or <c>null</c> for the local machine.</param>
        /// <param name="share">A <see cref="SHARE_INFO_2"/> structure
        /// describing the shared resource.</param>
        /// <exception cref="ArgumentException">If any of the properties in
        /// <paramref name="share"/> are invalid.</exception>
        /// <exception cref="Win32Exception">If the call failed for another
        /// reason than an invalid property.</exception>
        public static void Share(string? server, ref SHARE_INFO_2 share) {
            var status = NetShareAdd(server, 2, ref share, out var invalid);

            switch (status) {
                case 0:
                    // NERR_Success
                    break;

                case 87:
                    // ERROR_INVALID_PARAMETER
                    throw new ArgumentException("The share property at "
                        + $"position {invalid} "
                        + $"({GetField<SHARE_INFO_2>(invalid)?.Name}) is "
                        + "invalid.", nameof(share));

                default:
                    throw new Win32Exception(status);
            }
        }

        /// <summary>
        /// Share the specified <paramref name="folder"/> as
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <param name="maxUses"></param>
        /// <param name="permissions"></param>
        /// <param name="remark"></param>
        public static void ShareFolder(string? server,
                string? name,
                string folder,
                uint maxUses = uint.MaxValue,
                Access permissions = Access.All,
                string? remark = null) {
            var share = new SHARE_INFO_2 {
                shi2_netname = name ?? Path.GetFileName(folder),
                shi2_type = ShareType.DiskTree,
                shi2_remark = remark ?? string.Empty,
                shi2_path = folder,
                shi2_permissions = permissions,
                shi2_max_uses = maxUses
            };
            Share(server, ref share);
        }

        /// <summary>
        /// Deletes the share named <paramref name="shareName"/> from the
        /// <paramref name="server"/>.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="shareName"></param>
        public static void Unshare(string? server, string shareName)
            => NetShareDel(server, shareName);
        #endregion

        #region Private methods
        private static FieldInfo? GetField<TType>(int index)
                where TType : struct {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var fields = typeof(TType).GetFields(flags);
            return fields.Skip(index).FirstOrDefault();
        }
        #endregion
    }

}
