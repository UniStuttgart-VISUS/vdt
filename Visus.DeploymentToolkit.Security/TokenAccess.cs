// <copyright file="TokenAccess.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Security {

    /// <summary>
    /// Possible access rights for a token.
    /// </summary>
    [Flags]
    public enum TokenAccess : int {

        /// <summary>
        /// Required to change the default owner, primary group, or DACL
        /// of an access token.
        /// </summary>
        AssignPrimary = 0x0001,

        /// <summary>
        /// Required to duplicate an access token.
        /// </summary>
        Duplicate = 0x0002,

        /// <summary>
        /// Required to attach an impersonation access token to a process.
        /// </summary>
        Impersonate = 0x0004,

        /// <summary>
        /// Required to query an access token.
        /// </summary>
        Query = 0x0008,

        /// <summary>
        /// Required to query the source of an access token.
        /// </summary>
        QuerySource = 0x0010,

        /// <summary>
        /// Required to enable or disable the privileges in an access token.
        /// </summary>
        AdjustPrivileges = 0x0020,

        /// <summary>
        /// Required to adjust the attributes of the groups in an access
        /// token.
        /// </summary>
        AdjustGroups = 0x0040,

        /// <summary>
        /// Required to change the default owner, primary group, or DACL of
        /// an access token.
        /// </summary>
        AdjustDefault = 0x0080,

        /// <summary>
        /// Required to adjust the session ID of an access token. The
        /// <c>SE_TCB_NAME</c> privilege is required.
        /// </summary>
        AdjustSessionID = 0x0100
    }
}
