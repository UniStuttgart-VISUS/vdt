// <copyright file="JoinOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Possible flags to be used with <see cref="IKerberosService.JoinDomain"/>
    /// and <see cref="NetApi.NetJoinDomain"/>.
    /// </summary>
    [Flags]
    public enum JoinOptions {
        /// <summary>
        /// Joins the computer to a domain. If this value is not
        /// specified, joins the computer to a workgroup.
        /// </summary>
        JoinDomain = 0x00000001,

        /// <summary>
        /// Creates the account on the domain.
        /// </summary>
        CreateAccount = 0x00000002,

        DeleteAccount = 0x00000004,

        /// <summary>
        /// The join operation is occurring as part of an upgrade.
        /// </summary>
        Windows9xUpdate = 0x00000010,

        /// <summary>
        /// Allows a join to a new domain even if the computer is already
        /// joined to a domain.
        /// </summary>
        JoinIfJoined = 0x00000020,

        /// <summary>
        /// Performs an unsecured join. This option requests a domain join
        /// to a pre-created account without authenticating with domain user
        /// credentials. This option can be used in conjunction with
        /// <see cref="MachinePasswordPassed"/> option. In this case,
        /// <c>password</c> parameter is the password of the pre-created
        /// machine account.
        /// </summary>
        /// <remarks>
        /// Prior to Windows Vista with SP1 and Windows Server 2008, an
        /// unsecure join did not authenticate to the domain controller. All
        /// communication was performed using a null (unauthenticated)
        /// session. Starting with Windows Vista with SP1 and Windows Server
        /// 2008, the machine account name and password are used to
        /// authenticate to the domain controller.
        /// </remarks>
        Unsecure = 0x00000040,

        /// <summary>
        /// Indicates that the <c>password</c> parameter specifies a local
        /// machine account password rather than a user password. This flag
        /// is valid only for unsecured joins, which you must indicate by
        /// also setting the <see cref="Unsecure"> flag.
        /// </summary>
        /// <remarks>
        /// If you set this flag, then after the join operation succeeds,
        /// the machine password will be set to the value of
        /// <c>password</c>, if that value is a valid machine password.
        /// </remarks>
        MachinePasswordPassed = 0x00000080,

        /// <summary>
        /// Indicates that the service principal name (SPN) and the
        /// DnsHostName properties on the computer object should not be
        /// updated at this time.
        /// </summary>
        /// <remarks>
        /// Typically, these properties are updated during the join
        /// operation. Instead, these properties should be updated during a
        /// subsequent call to the NetRenameMachineInDomain function. These
        /// properties are always updated during the rename operation..
        /// </remarks>
        DeferSetSpn = 0x00000100,

        /// <summary>
        /// Allow the domain join if existing account is a domain
        /// controller.
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows Vista and later.
        /// </remarks>
        DomainControllerAccount = 0x00000200,

        /// <summary>
        /// Join the target machine specified in <c>server</c> parameter
        /// with a new name queried from the registry on the machine
        /// specified in the <c>server</c> parameter.
        /// </summary>
        /// <remarks>
        /// <para>This option is used if SetComputerNameEx has been called
        /// prior to rebooting the machine. The new computer name will not
        /// take effect until a reboot. With this option, the caller
        /// instructs <see cref="NetJoinDomain"/> to use the new name during
        /// the domain join operation. A reboot is required after calling
        /// <see cref="NetJoinDomain"/> successfully at which time both,
        /// the computer name change and domain membership change, will have
        /// taken affect.</para>
        /// <para>This flag is supported on Windows Vista and later.</para>
        /// </remarks>
        JoinWithNewName = 0x00000400,

        /// <summary>
        /// Join the target machine specified in <c>server</c> parameter
        /// using a pre-created account without requiring a writable domain
        /// controller.
        /// </summary>
        /// <remarks>
        /// <para>This option provides the ability to join a machine to
        /// domain if an account has already been provisioned and replicated
        /// to a read-only domain controller. The target read-only domain
        /// controller is specified as part of the <c>domain</c> parameter,
        /// after the domain name delimited by a &quot;\&quot; character.
        /// This provisioning must include the machine secret. The machine
        /// account must be added via group membership into the allowed list
        /// for password replication policy, and the account password must
        /// be replicated to the read-only domain controller prior to the
        /// join operation.</para>
        /// <para>Starting with Windows 7, an alternate mechanism is to use
        /// the offline domain join mechanism. For more information, see
        /// the NetProvisionComputerAccount and NetRequestOfflineDomainJoin
        /// functions.</para>
        /// <para>This flag is supported on Windows Vista and later.</para>
        /// </remarks>
        ReadOnly = 0x00000800,

        /// <summary>
        /// When joining the domain don't try to set the preferred domain
        /// controller in the registry.
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows 7, Windows Server 2008 R2, and
        /// later.
        /// </remarks>
        AmbiguousDomainController = 0x00001000,

        /// <summary>
        /// When joining the domain don't create the Netlogon cache.
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows 7, Windows Server 2008 R2, and
        /// later.
        /// </remarks>
        NoNetlogonCache = 0x00002000,

        /// <summary>
        /// When joining the domain don't force Netlogon service to start.
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows 7, Windows Server 2008 R2, and
        /// later.
        /// </remarks>
        DoNotControlServices = 0x00004000,

        /// <summary>
        /// When joining the domain for offline join only, set target
        /// machine hostname and NetBIOS name.
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows 7, Windows Server 2008 R2, and
        /// later.
        /// </remarks>
        SetMachineName = 0x00008000,

        /// <summary>
        /// When joining the domain, override other settings during domain
        /// join and set the service principal name (SPN).
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows 7, Windows Server 2008 R2, and
        /// later.
        /// </remarks>
        ForceSetSpn = 0x00010000,

        /// <summary>
        /// When joining the domain, do not reuse an existing account.
        /// </summary>
        /// <remarks>
        /// This flag is supported on Windows 7, Windows Server 2008 R2, and
        /// later.
        /// </remarks>
        NoAccountReuse = 0x00020000,

        /// <summary>
        /// If this bit is set, unrecognised flags will be ignored by the
        /// <see cref="NetJoinDomain"/> function and
        /// <see cref="NetJoinDomain"/> will behave as if the flags were not
        /// set.
        /// </summary>
        IgnoreUnsupportedFlags = 0x10000000
    }
}
