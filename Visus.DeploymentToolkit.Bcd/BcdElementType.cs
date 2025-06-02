// <copyright file="BcdElementType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Possible &quot;datatypes&quot; of BCD elements as described on
    /// https://www.geoffchappell.com/notes/windows/boot/bcd/elements.htm?tx=37.
    /// </summary>
    [Flags]
    public enum BcdElementType : uint {

        /// <summary>
        /// Selects the bits defining a class.
        /// </summary>
        ClassMask = 0xF0000000,

        /// <summary>
        /// Selects the bits defining a format.
        /// </summary>
        FormatMask = 0x0F000000,

        /// <summary>
        /// Identifies the class as a library.
        /// </summary>
        Library = 0x10000000,

        /// <summary>
        /// Identifies the class as an application.
        /// </summary>
        Application = 0x20000000,

        /// <summary>
        /// Identifies the class as a device.
        /// </summary>
        Device = 0x30000000,

        /// <summary>
        /// Identifies the class as a template.
        /// </summary>
        Template = 0x40000000,

        /// <summary>
        /// Identifies the format as a device.
        /// </summary>
        DevicePath = 0x01000000,

        /// <summary>
        /// Identifies the format as a string.
        /// </summary>
        String = 0x02000000,

        /// <summary>
        /// Identifies the format as a GUID.
        /// </summary>
        Guid = 0x03000000,

        /// <summary>
        /// Identifies the format as a list of GUIDs.
        /// </summary>
        GuidList = 0x04000000,

        /// <summary>
        /// Identifies the format as an integral number.
        /// </summary>
        Integer = 0x05000000,

        /// <summary>
        /// Identifies the format as a Boolean value.
        /// </summary>
        Boolean = 0x06000000,

        /// <summary>
        /// Identifies the format as a list of integral values.
        /// </summary>
        IntegerList = 0x07000000,

        [BcdEditName("device")]
        LibraryApplicationDevice = Library | DevicePath | 0x00000001,

        [BcdEditName("path")]
        LibraryApplicationPath = Library | String | 0x00000002,

        [BcdEditName("description")]
        LibraryDescription = Library | String | 0x00000004,

        [BcdEditName("locale")]
        LibraryLocale = Library | String | 0x00000005,

        [BcdEditName("inherit")]
        LibraryInheritObjects = Library | GuidList| 0x00000006,

        [BcdEditName("truncatememory")]
        LibraryTruncatePhysicalMemory = Library | Integer | 0x00000007,

        [BcdEditName("recoverysequence")]
        LibraryRecoverySequence = Library | GuidList | 0x00000008,

        [BcdEditName("recoveryenabled")]
        LibraryAutoRecoveryEnabled = Library | Boolean | 0x00000009,

        [BcdEditName("badmemorylist")]
        LibraryBadMemoryList = Library | IntegerList | 0x0000000A,

        [BcdEditName("badmemoryaccess")]
        LibraryAllowBadMemoryAccess = Library | Boolean | 0x0000000B,

        /// <remarks>
        /// 1 = UseAll
        /// 2 = UsePrivate
        /// </remarks>
        [BcdEditName("firstmegabytepolicy")]
        LibraryFirstMegabytePolicy = Library | Integer | 0x0000000C,

        [BcdEditName("relocatephysicalmemory")]
        LibraryRelocatePhysicalMemory = Library | Integer | 0x0000000D,

        [BcdEditName("avoidlowmemory", Major = 6, Minor = 1)]
        LibraryAvoidLowPhysicalMemory = Library | Integer | 0x0000000E,

        [BcdEditName("traditionalkseg", Major = 6, Minor = 1)]
        [Undocumented]
        LibraryTraditionalKsegMappings = Library | Boolean | 0x0000000F,

        [BcdEditName("bootdebug")]
        LibraryDebuggerEnabled = Library | Boolean | 0x00000010,

        /// <remarks>
        /// 0 = Serial
        /// 1 = 1394
        /// 2 = USB
        /// 3 = NET
        /// 4 = Local
        /// </remarks>
        [BcdEditName("debugtype")]
        LibraryDebuggerType = Library | Integer | 0x00000011,

        [BcdEditName("debugaddress")]
        LibrarySerialDebuggerPortAddress = Library | Integer | 0x00000012,

        [BcdEditName("debugport")]
        LibrarySerialDebuggerPort = Library | Integer | 0x00000013,

        [BcdEditName("baudrate")]
        LibrarySerialDebuggerBaudRate = Library | Integer | 0x00000014,

        [BcdEditName("channel")]
        Library1394DebuggerChannel = Library | Integer | 0x00000015,

        [BcdEditName("targetname")]
        LibraryUsbDebuggerTargetName = Library | String | 0x00000016,

        [BcdEditName("noumex")]
        LibraryDebuggerIgnoreUsermodeExceptions = Library | Boolean | 0x00000017,

        /// <remarks>
        /// 0 = Active
        /// 1 = AutoEnable
        /// 2 = Disable
        /// </remarks>
        [BcdEditName("debugstart")]
        LibraryDebuggerStartPolicy = Library | Integer | 0x00000018,

        [BcdEditName("busparams", Major = 6, Minor = 1)]
        LibraryDebuggerBusParameters = Library | String | 0x00000019,

        [BcdEditName("hostip", Major = 6, Minor = 2)]
        LibraryDebuggerNetHostIP = Library | Integer | 0x0000001A,

        [BcdEditName("port", Major = 6, Minor = 2)]
        LibraryDebuggerNetPort = Library | Integer | 0x0000001B,

        [BcdEditName("dhcp", Major = 6, Minor = 2)]
        LibraryDebuggerNetDhcp = Library | Boolean | 0x0000001C,

        [BcdEditName("key", Major = 6, Minor = 2)]
        LibraryDebuggerNetKey = Library | String | 0x0000001D,

        [BcdEditName("vm", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryDebuggerNetVm = Library | Boolean | 0x0000001E,

        [BcdEditName("hostipv6", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryDebuggerNetHostIPv6 = Library | String | 0x0000001F,

        [BcdEditName("bootems")]
        LibraryEmsEnabled = Library | Boolean | 0x00000020,

        [BcdEditName("emsport")]
        LibraryEmsPortNumber = Library | Integer | 0x00000022,

        [BcdEditName("emsbaudrate")]
        LibraryEmsBaudRate = Library | Integer | 0x00000023,

        [BcdEditName("loadoptions")]
        LibraryLoadOptionsString = Library | String | 0x00000030,

        [Undocumented]
        LibraryAttemptNonBcdStart = Library | Boolean | 0x00000031,

        [BcdEditName("advancedoptions")]
        LibraryDisplayAdvancedOptions = Library | Boolean | 0x00000040,

        [BcdEditName("optionsedit")]
        LibraryDisplayOptionsEdit = Library | Boolean | 0x00000041,

        [BcdEditName("keyringaddress")]
        LibraryFveKeyRingAddress = Library | Integer | 0x00000042,

        [BcdEditName("bootstatdevice")]
        [Undocumented]
        LibraryBsdLogDevice = Library | DevicePath | 0x00000043,

        [BcdEditName("bootstatfilepath")]
        [Undocumented]
        LibraryBsdLogPath = Library | String | 0x00000044,

        [BcdEditName("preservebootstat")]
        [Undocumented]
        LibraryBsdPreserveLog = Library | Boolean | 0x00000045,

        [BcdEditName("graphicsmodedisabled")]
        LibraryGraphicsModeDisabled = Library | Boolean | 0x00000046,

        /// <remarks>
        /// 0 = Default
        /// 1 = DisallowMmConfig
        /// </remarks>
        [BcdEditName("configaccesspolicy")]
        LibraryConfigAccessPolicy = Library | Integer | 0x00000047,

        [BcdEditName("nointegritychecks")]
        [Undocumented]
        LibraryDisableIntegrityChecks = Library | Boolean | 0x00000048,

        [BcdEditName("testsigning")]
        LibraryAllowPrereleaseSignatures = Library | Boolean | 0x00000049,

        [BcdEditName("fontpath")]
        [Undocumented]
        LibraryFontPath = Library | String | 0x0000004A,

        /// <remarks>
        /// 0 = Default
        /// 1 = Enable
        /// 2 = Disable
        /// </remarks>
        [BcdEditName("integrityservices", Major = 6, Minor = 1)]
        [Undocumented]
        LibraryIntegrityServicesPolicy = Library | Integer | 0x0000004B,

        [BcdEditName("volumebandid", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryFveBandID = Library | Integer | 0x0000004C,

        [BcdEditName("extendedinput")]
        LibraryConsoleExtendedInput = Library | Boolean | 0x00000050,

        [BcdEditName("initialconsoleinput")]
        [Undocumented]
        LibraryInitialConsoleInput = Library | Integer | 0x00000051,

        /// <remarks>
        /// 0 = 1024x768
        /// 1 = 800x600
        /// 2 = 1024x600
        /// </remarks>
        [BcdEditName("graphicsresolution")]
        LibraryGraphicsResolution = Library | Integer | 0x00000052,

        [BcdEditName("restartonfailure", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryRestartOnFailure = Library | Boolean | 0x00000053,

        [BcdEditName("highestmode", Major = 6, Minor = 2)]
        LibraryGraphicsForceHighestMode = Library | Boolean | 0x00000054,

        [BcdEditName("isolatedcontext", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryIsolatedExecutionContext = Library | Boolean | 0x00000060,

        /// <remarks>
        /// 0 = Default
        /// 1 = Resume
        /// 2 = HyperV
        /// 3 = Recovery
        /// 4 = StartupRepair
        /// 5 = SystemImageRecovery
        /// 6 = CommandPrompt
        /// 7 = SystemRestore
        /// 8 = PushButtonReset
        /// </remarks>
        [BcdEditName("displaymessage", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryBootUxDisplayMessage = Library | Integer | 0x00000065,

        /// <remarks>
        /// 0 = Default
        /// 1 = Resume
        /// 2 = HyperV
        /// 3 = Recovery
        /// 4 = StartupRepair
        /// 5 = SystemImageRecovery
        /// 6 = CommandPrompt
        /// 7 = SystemRestore
        /// 8 = PushButtonReset
        /// </remarks>
        [BcdEditName("displaymessageoverride", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryBootUxDisplayMessageOverride = Library | Integer | 0x00000066,

        [Undocumented]
        LibraryBootUxLogoDisable = Library | Boolean | 0x00000067,

        [BcdEditName("nobootuxtext", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryBootUxTextDisable = Library | Boolean | 0x00000068,

        [BcdEditName("nobootuxprogress", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryBootUxProgressDisable = Library | Boolean | 0x00000069,

        [BcdEditName("nobootuxfade", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryBootUxFadeDisable = Library | Boolean | 0x0000006A,

        [Undocumented]
        LibraryBootUxReservePoolDebug = Library | Boolean | 0x0000006B,

        [BcdEditName("bootuxdisabled", Major = 6, Minor = 2)]
        LibraryBootUxDisable = Library | Boolean | 0x0000006C,

        [Undocumented]
        LibraryBootUxFadeFrames = Library | Integer | 0x0000006D,

        [Undocumented]
        LibraryBootUxDumpStats = Library | Boolean | 0x0000006E,

        [Undocumented]
        LibraryBootUxShowStats = Library | Boolean | 0x0000006F,

        [Undocumented]
        LibraryMultiBootSystem = Library | Boolean | 0x00000071,

        [BcdEditName("nokeyboard", Major = 6, Minor = 2)]
        LibraryForceNoKeyboard = Library | Boolean | 0x00000072,

        [Undocumented]
        LibraryAliasWindowsKey = Library | Integer | 0x00000073,

        [BcdEditName("bootshutdowndisabled", Major = 6, Minor = 2)]
        LibraryBootShutdownDisabled = Library | Boolean | 0x00000074,

        [Undocumented]
        LibraryPerformanceFrequency = Library | Integer | 0x00000075,

        [Undocumented]
        LibrarySecureBootRawPolicy = Library | Integer | 0x00000076,

        [BcdEditName("allowedinmemorysettings", Major = 6, Minor = 2)]
        [Undocumented]
        LibraryAllowedInMemorySettings = Library | IntegerList | 0x00000077,

        [Undocumented]
        X16000078 = Library | Boolean | 0x00000078,

        [BcdEditName("bootuxtransitiontime", Major = 10)]
        [Undocumented]
        LibraryBootUxBitmapTransitionTime = Library | Integer | 0x00000079,

        [BcdEditName("mobilegraphics", Major = 10)]
        [Undocumented]
        LibraryTwoBootImages = Library | Boolean | 0x0000007A,

        [BcdEditName("forcefipscrypto", Minor = 6, Major = 2)]
        [Undocumented]
        LibraryForceFipsCrypto = Library | Boolean | 0x0000007B,

        /// <remarks>
        /// 0 = Legacy
        /// 1 = Standard
        /// 2 = Simple
        /// </remarks>
        [BcdEditName("booterrorux", Major = 6, Minor = 3)]
        LibraryBootErrorUx = Library | Integer | 0x0000007D,


        [BcdEditName("flightsigning", Major = 10)]
        LibraryAllowFlightSignatures = Library | Boolean | 0x0000007E,

        /// <remarks>
        /// 0 = Default
        /// 1 = Sha1
        /// </remarks>
        [BcdEditName("measuredbootlogformat", Major = 10)]
        [Undocumented]
        LibraryBootMeasurementLogFormat = Library | Integer | 0x0000007F,

        /// <remarks>
        /// displayrotation	0 = 0
        /// 1 = 90
        /// 2 = 180
        /// 3 = 270
        /// </remarks>
        [BcdEditName("displayrotation")]
        [Undocumented]
        LibraryDisplayRotation = Library | Integer | 0x00000080,

        [BcdEditName("logcontrol")]
        [Undocumented]
        LibraryLogControl = Library | Integer | 0x00000081,

        [BcdEditName("nofirmwaresync")]
        [Undocumented]
        LibraryNoFirmwareSync = Library | Boolean | 0x00000082,

        [Undocumented]
        X11000083 = Library | DevicePath | 0x00000083,

        [BcdEditName("windowssyspart")]
        [Undocumented]
        LibraryWindowsSystemDevice = Library | DevicePath | 0x00000084,

        [Undocumented]
        X16000085 = Library | Boolean | 0x00000085,

        [Undocumented]
        X15000086 = Library | Integer | 0x00000086,

        [BcdEditName("numlock")]
        [Undocumented]
        LibraryNumLockOn = Library | Boolean | 0x00000087,

        [BcdEditName("additionalcipolicy")]
        LibraryAdditionalCiPolicy = Library | String | 0x00000088,

        /// <remarks>
        /// 0 = default
        /// 1 = optout
        /// 2 = optin
        /// </remarks>
        [BcdEditName("linearaddress57")]
        LibraryLinearAddress57Policy = Library | Integer | 0x00000088,

        [BcdEditName("skipffumode", Major = 10, Minor = 0)]
        [Undocumented]
        FfuLoaderTypeOneShotSkipFfuUpdate = Application |Boolean | 0x00000202,

        [BcdEditName("forceffumode", Major = 10, Minor = 0)]
        [Undocumented]
        FfuLoaderTypeForceFfu = Application | Boolean | 0x00000203,

        [BcdEditName("chargethreshold", Major = 10, Minor = 0)]
        [Undocumented]
        ChargingTypeBootThreshold = Application | Integer | 0x00000510,

        [BcdEditName("offmodecharging", Major = 10, Minor = 0)]
        [Undocumented]
        ChargingTypeOffModeCharging = Application | Boolean | 0x00000512,

        [BcdEditName("bootflow", Major = 10, Minor = 0)]
        [Undocumented]
        GlobalTypeBootFlow = Application | Integer | 0x00000AAA,

        [Undocumented]
        DeviceType = Template | Integer | 0x00000001,

        [Undocumented]
        ApplicationRelativePath = Template | String | 0x00000002,

        [Undocumented]
        RamDiskDeviceRelativePath = Template | String | 0x00000003,

        [Undocumented]
        OmitOsLoaderElements = Template | Boolean | 0x00000004,

        [BcdEditName("elementstomigrate", Major = 10)]
        [Undocumented]
        ElementsToMigrateList = Template | IntegerList | 0x00000006,

        [Undocumented]
        RecoveryOs = Template | Boolean | 0x00000010,

        [BcdEditName("displayorder")]
        DisplayOrder = Application | GuidList | 0x00000001,

        [BcdEditName("bootsequence")]
        BootSequence = Application | GuidList | 0x00000002,

        [BcdEditName("default")]
        DefaultObject = Application | Guid | 0x00000003,

        [BcdEditName("timeout")]
        Timeout = Application | Integer | 0x00000004,

        [BcdEditName("resume")]
        AttemptResume = Application | Boolean | 0x00000005,

        [BcdEditName("resumeobject")]
        ResumeObject = Application | Guid | 0x00000006,

        [BcdEditName("startupsequence")]
        StartupSequence = Application | GuidList | 0x00000007,

        [BcdEditName("toolsdisplayorder")]
        ToolDisplayOrder = Application | GuidList | 0x00000010,

        [BcdEditName("displaybootmenu")]
        DisplayBootMenu = Application | Boolean | 0x00000020,

        [BcdEditName("noerrordisplay")]
        [Undocumented]
        NoErrorDisplay = Application | Boolean | 0x00000021,

        [BcdEditName("device")]
        [Undocumented]
        BcdDevice = Application | DevicePath | 0x00000022,

        [BcdEditName("bcdfilepath")]
        [Undocumented]
        BcdFilePath = Application | String | 0x00000023,

        [Undocumented]
        HormEnabled = Application | Boolean | 0x00000024,

        [BcdEditName("hiberboot", Major = 6, Minor = 2)]
        HiberBoot = Application | Boolean | 0x00000025,

        [Undocumented]
        PasswordOverride = Application | String | 0x00000026,

        [Undocumented]
        PinpassPhraseOverride = Application | String | 0x00000027,

        [BcdEditName("processcustomactionsfirst", Major = 6, Minor = 2)]
        [Undocumented]
        ProcessCustomActionsFirst = Application | Boolean | 0x00000028,

        [Undocumented]
        X2600002A = Application | Boolean | 0x0000002A,

        [BcdEditName("customactions", Major = 6, Minor = 2)]
        [Undocumented]
        CustomActionsList = Application | IntegerList | 0x00000030,

        [BcdEditName("persistbootsequence", Major = 6, Minor = 2)]
        PersistBootSequence = Application | Boolean | 0x00000031,

        [BcdEditName("skipstartupsequence", Major = 6, Minor = 2)]
        [Undocumented]
        SkipStartupSequence = Application | Boolean | 0x00000032,

        [BcdEditName("fverecoveryurl")]
        [Undocumented]
        FveRecoveryUrl = Application | String | 0x00000040,

        [BcdEditName("fverecoverymessage")]
        [Undocumented]
        FveRecoveryMessage = Application | String | 0x00000041,

        [BcdEditName("flightedbootmgr")]
        [Undocumented]
        BootFlightBootMgr = Application | Boolean | 0x00000042,

        [Undocumented]
        FveUnlockRetryCountIPv4 = Application | Integer | 0x00000043,

        [Undocumented]
        FveUnlockRetryCountIPv6 = Application | Integer | 0x00000044,

        [Undocumented]
        FveServerAddressIPv4 = Application | String | 0x00000045,

        [Undocumented]
        FveServerAddressIPv6 = Application | String | 0x00000046,

        [Undocumented]
        FveStationAddressIPv4 = Application | String | 0x00000047,

        [Undocumented]
        FveStationAddressIPv6 = Application | String | 0x00000048,

        [Undocumented]
        FveStationAddressSubnetMaskIPv4 = Application | String | 0x00000049,

        [Undocumented]
        FveStationAddressPrefixIPv6 = Application | String | 0x0000004A,

        [Undocumented]
        FveGatewayIPv4 = Application | String | 0x0000004B,

        [Undocumented]
        FveGatewayIPv6 = Application | String | 0x0000004C,

        [Undocumented]
        FveNetworkTimeout = Application | Integer | 0x0000004D,

        [Undocumented]
        FveRemotePortIPv4 = Application | Integer | 0x0000004E,

        [Undocumented]
        FveRemotePortIPv6 = Application | Integer | 0x0000004F,

        [Undocumented]
        FveStationPortIPv4 = Application | Integer | 0x00000050,

        [Undocumented]
        FveStationPortIPv6 = Application | Integer | 0x00000051,

        [BcdEditName("osdevice")]
        OperatingSystemDevice = Application | DevicePath | 0x00000001,

        [BcdEditName("systemroot")]
        SystemRoot = Application | String | 0x00000002,

        [BcdEditName("resumeobject")]
        AssociatedResumeObject = Application | Guid | 0x00000003,

        [BcdEditName("stampdisks")]
        StampDisks = Application | Boolean | 0x00000004,

        [Undocumented]
        X21000005 = Application | DevicePath | 0x00000005,

        [Undocumented]
        X25000008 = Application | Integer | 0x00000008,

        [BcdEditName("detecthal")]
        DetectKernelAndHal = Application | Boolean | 0x00000010,

        [BcdEditName("kernel")]
        KernelPath = Application | String | 0x00000011,

        [BcdEditName("hal")]
        HalPath = Application | String | 0x00000012,

        [BcdEditName("dbgtransport")]
        DebugTransportPath = Application | String | 0x00000013,

        /// <remarks>
        /// 0 = OptIn
        /// 1 = OptOut
        /// 2 = AlwaysOff
        /// 3 = AlwaysOn
        /// </remarks>
        [BcdEditName("nx")]
        NxPolicy = Application | Integer | 0x00000020,

        /// <remarks>
        /// 0 = Default
        /// 1 = ForceEnable
        /// 2 = ForceDisable
        /// </remarks>
        [BcdEditName("pae")]
        PaePolicy = Application | Integer | 0x00000021,

        [BcdEditName("winpe")]
        WinPeMode = Application | Boolean | 0x00000022,

        [BcdEditName("nocrashautoreboot")]
        DisableCrashAutoReboot = Application | Boolean | 0x00000024,

        [BcdEditName("lastknowngood")]
        UseLastGoodSettings = Application | Boolean | 0x00000025,

        [BcdEditName("oslnointegritychecks")]
        [Undocumented]
        DisableCodeIntegrityChecks = Application | Boolean | 0x00000026,

        [BcdEditName("osltestsigning")]
        [Undocumented]
        AllowPrereleaseSignatures = Application | Boolean | 0x00000027,

        [BcdEditName("nolowmem")]
        NoLowMemory = Application | Boolean | 0x00000030,

        [BcdEditName("removememory")]
        RemoveMemory = Application | Integer | 0x00000031,

        [BcdEditName("increaseuserva")]
        IncreaseUserVa = Application | Integer | 0x00000032,

        [BcdEditName("perfmem")]
        PerformanceDataMemory = Application | Integer | 0x00000033,

        [BcdEditName("vga")]
        UseVgaDriver = Application | Boolean | 0x00000040,

        [BcdEditName("quietboot")]
        DisableBootDisplay = Application | Boolean | 0x00000041,

        [BcdEditName("novesa")]
        DisableVesaBios = Application | Boolean | 0x00000042,

        [BcdEditName("novga")]
        DisableVgaMode = Application | Boolean | 0x00000043,

        [BcdEditName("clustermodeaddressing")]
        ClusterModeAddressing = Application | Integer | 0x00000050,

        [BcdEditName("usephysicaldestination")]
        UsePhysicalDestination = Application | Boolean | 0x00000051,

        [BcdEditName("restrictapiccluster")]
        RestrictApicCluster = Application | Integer | 0x00000052,

        [BcdEditName("evstore", Major = 6, Minor = 1)]
        [Undocumented]
        EvStore = Application | String | 0x00000053,

        [BcdEditName("uselegacyapicmode", Major = 6, Minor = 1)]
        UseLegacyApicMode = Application | Boolean | 0x00000054,

        /// <remarks>
        /// 0 = Default
        /// 1 = Disable
        /// 2 = Enable
        /// </remarks>
        [BcdEditName("x2apicpolicy", Major = 6, Minor = 1)]
        X2ApicPolicy = Application | Integer | 0x00000055,

        [BcdEditName("onecpu")]
        UseBootProcessorOnly = Application | Boolean | 0x00000060,

        [BcdEditName("numproc")]
        NumberOfProcessors = Application | Integer | 0x00000061,

        [BcdEditName("maxproc")]
        ForceMaximumProcessors = Application | Boolean | 0x00000062,

        [BcdEditName("configflags")]
        ProcessorConfigurationFlags = Application | Integer | 0x00000063,

        [BcdEditName("maxgroup", Major = 6, Minor = 1)]
        MaximiseGroupsCreated = Application | Boolean | 0x00000064,

        [BcdEditName("groupaware", Major = 6, Minor = 1)]
        ForceGroupAwareness = Application | Boolean | 0x00000065,

        [BcdEditName("groupsize", Major = 6, Minor = 1)]
        GroupSize = Application | Integer | 0x00000066,

        [BcdEditName("usefirmwarepcisettings")]
        UseFirmwarePciSettings = Application | Boolean | 0x00000070,

        /// <remarks>
        /// 0 = Default
        /// 1 = ForceDisable
        /// </remarks>
        [BcdEditName("msi")]
        MsiPolicy = Application | Integer | 0x00000071,

        /// <remarks>
        /// 0 = Default
        /// 1 = ForceDisable
        /// </remarks>
        [BcdEditName("pciexpress")]
        [Undocumented]
        PciExpressPolicy = Application | Integer | 0x00000072,

        /// <remarks>
        /// 0 = Minimal
        /// 1 = Network
        /// 2 = DsRepair
        /// </remarks>
        [BcdEditName("safeboot")]
        SafeBoot = Application | Integer | 0x00000080,

        [BcdEditName("safebootalternateshell")]
        SafeBootAlternateShell = Application | Boolean | 0x00000081,

        [BcdEditName("bootlog")]
        BootLogInitialisation = Application | Boolean | 0x00000090,

        [BcdEditName("sos")]
        VerboseObjectLoadMode = Application | Boolean | 0x00000091,

        [BcdEditName("debug")]
        KernelDebuggerEnabled = Application | Boolean | 0x000000A0,

        [BcdEditName("halbreakpoint")]
        DebuggerHalBreakpoint = Application | Boolean | 0x000000A1,

        [BcdEditName("useplatformclock", Major = 6, Minor = 1)]
        UsePlatformClock = Application | Boolean | 0x000000A2,

        [BcdEditName("forcelegacyplatform", Major = 6, Minor = 2)]
        ForceLegacyPlatform = Application | Boolean | 0x000000A3,

        [BcdEditName("useplatformtick", Major = 6, Minor = 2)]
        [Undocumented]
        UsePlatformTick = Application | Boolean | 0x000000A4,

        [BcdEditName("disabledynamictick", Major = 6, Minor = 2)]
        [Undocumented]
        DisableDynamicTick = Application | Boolean | 0x000000A5,

        /// <remarks>
        /// 0 = Default
        /// 1 = Legacy
        /// 2 = Enhanced
        /// </remarks>
        [BcdEditName("tscsyncpolicy", Major = 6, Minor = 2)]
        TscSyncPolicy = Application | Integer | 0x000000A6,

        [BcdEditName("ems")]
        EmsEnabled = Application | Boolean | 0x000000B0,

        [Undocumented]
        ForceFailure = Application | Integer | 0x000000C0,

        /// <remarks>
        /// 0 = Fatal
        /// 1 = UseErrorControl
        /// </remarks>
        [BcdEditName("driverloadfailurepolicy")]
        DriverLoadFailurePolicy = Application | Integer | 0x000000C1,

        /// <remarks>
        /// 0 = Legacy
        /// 1 = Standard
        /// </remarks>
        [BcdEditName("bootmenupolicy", Major = 6, Minor = 2)]
        BootMenuPolicy = Application | Integer | 0x000000C2,

        [BcdEditName("onetimeadvancedoptions", Major = 6, Minor = 2)]
        [Undocumented]
        OneTimeAdvancedOptions = Application | Boolean | 0x000000C3,

        [BcdEditName("onetimeoptionsedit", Major = 6, Minor = 2)]
        [Undocumented]
        EditOptionsOneTime = Application | Boolean | 0x000000C4,

        /// <remarks>
        /// 0 = DisplayAllFailures
        /// 1 = IgnoreAllFailures
        /// 2 = IgnoreShutdownFailures
        /// 3 = IgnoreBootFailures
        /// 4 = IgnoreCheckpointFailures
        /// 5 = DisplayShutdownFailures
        /// 6 = DisplayBootFailures
        /// 7 = DisplayCheckpointFailures
        /// 8 = AlwaysDisplayStartupFailures
        /// </remarks>
        [BcdEditName("bootstatuspolicy")]
        BootStatusPolicy = Application | Integer | 0x000000E0,

        [BcdEditName("disableelamdrivers", Major = 6, Minor = 2)]
        DisableElamDrivers = Application | Boolean | 0x000000E1,

        /// <remarks>
        /// 0 = Off
        /// 1 = Auto
        /// </remarks>
        [BcdEditName("hypervisorlaunchtype")]
        HypervisorLaunchType = Application | Integer | 0x000000F0,

        [BcdEditName("hypervisorpath", Major = 6, Minor = 1)]
        HypervisorPath = Application | String | 0x000000F1,

        [BcdEditName("hypervisordebug")]
        HypervisorDebuggerEnabled = Application | Boolean | 0x000000F2,

        /// <remarks>
        /// 0 = Serial
        /// 1 = 1394
        /// 2 = None
        /// 3 = Net
        /// </remarks>
        [BcdEditName("hypervisordebugtype")]
        HypervisorDebuggerType = Application | Integer | 0x000000F3,

        [BcdEditName("hypervisordebugport")]
        HypervisorDebuggerPortNumber = Application | Integer | 0x000000F4,

        [BcdEditName("hypervisorbaudrate")]
        HypervisorDebuggerBaudRate = Application | Integer | 0x000000F5,

        [BcdEditName("hypervisorchannel")]
        HypervisorDebugger1394Channel = Application | Integer | 0x000000F6,

        /// <remarks>
        /// 0 = Disabled
        /// 1 = Basic
        /// 2 = Standard
        /// </remarks>
        [BcdEditName("bootux", Major = 6, Minor = 1)]
        BootUxPolicy = Application | Integer | 0x000000F7,

        [BcdEditName("hypervisordisableslat", Major = 6, Minor = 1)]
        [Undocumented]
        HypervisorSlatDisabled = Application | Boolean | 0x000000F8,

        [BcdEditName("hypervisorbusparams", Major = 6, Minor = 2)]
        HypervisorDebuggerBusParams = Application | String | 0x000000F9,

        [BcdEditName("hypervisornumproc", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorNumProc = Application | Integer | 0x000000FA,

        [BcdEditName("hypervisorrootprocpernode", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorRootProcPerNode = Application | Integer | 0x000000FB,

        [BcdEditName("hypervisoruselargevtlb", Major = 6, Minor = 1)]
        HypervisorUseLargeVtlb = Application | Boolean | 0x000000FC,

        [BcdEditName("hypervisorhostip", Major = 6, Minor = 2)]
        HypervisorDebuggerNetHostIp = Application | Integer | 0x000000FD,

        [BcdEditName("hypervisorhostport", Major = 6, Minor = 2)]
        HypervisorDebuggerNetHostPort = Application | Integer | 0x000000FE,

        [BcdEditName("hypervisordebugpages", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorDebuggerPages = Application | Integer | 0x000000FF,

        /// <remarks>
        /// 0 = Default
        /// 1 = ForceDisable
        /// 2 = ForceEnable
        /// </remarks>
        [BcdEditName("tpmbootentropy", Major = 6, Minor = 1)]
        TpmBootEntropy = Application | Integer | 0x00000100,

        [BcdEditName("hypervisorusekey", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorDebuggerNetKey = Application | String | 0x00000110,

        [Undocumented]
        HypervisorProductSkuType = Application | String | 0x00000112,

        [BcdEditName("hypervisorrootproc", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorRootProc = Application | Integer | 0x00000113,

        [BcdEditName("hypervisordhcp", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorDebuggerNetDhcp = Application | Boolean | 0x00000114,

        /// <remarks>
        /// 0 = Default
        /// 1 = Enable
        /// 2 = Disable
        /// </remarks>
        [BcdEditName("hypervisoriommupolicy", Major = 6, Minor = 2)]
        HypervisorIommuPolicy = Application | Integer | 0x00000115,

        [BcdEditName("hypervisorusevapic", Major = 6, Minor = 2)]
        [Undocumented]
        HypervisorUseVapic = Application | Boolean | 0x00000116,

        [BcdEditName("hypervisorloadoptions", Major = 6, Minor = 3)]
        [Undocumented]
        HypervisorLoadOptions = Application | String | 0x00000117,

        /// <remarks>
        /// 0 = Disable
        /// 1 = Enable
        /// </remarks>
        [BcdEditName("hypervisormsrfilterpolicy", Major = 10)]
        [Undocumented]
        HypervisorMsrFilterPolicy = Application | Integer | 0x00000118,

        /// <remarks>
        /// 0 = Disable
        /// 1 = Enable
        /// </remarks>
        [BcdEditName("hypervisormmionxpolicy", Major = 10)]
        [Undocumented]
        HypervisorMmioNxPolicy = Application | Integer | 0x00000119,

        /// <remarks>
        /// 0 = Classic
        /// 1 = Core
        /// 2 = Root
        /// </remarks>
        [BcdEditName("hypervisorschedulertype")]
        [Undocumented]
        HypervisorSchedulerType = Application | Integer | 0x0000011A,

        [BcdEditName("hypervisorrootprocnumanodes")]
        [Undocumented]
        HypervisorRootProcNumaNodes = Application | String | 0x0000011B,

        /// <remarks>
        /// 0 = System
        /// 1 = Hypervisor
        /// </remarks>
        [BcdEditName("hypervisorperfmon")]
        [Undocumented]
        HypervisorPerfMon = Application | Integer | 0x0000011C,

        [BcdEditName("hypervisorrootprocpercore")]
        [Undocumented]
        HypervisorRootProcPerCore = Application | Integer | 0x0000011D,

        [BcdEditName("hypervisorrootprocnumanodelps")]
        [Undocumented]
        HypervisorRootProcNumaNodeLps = Application | String | 0x0000011E,

        [BcdEditName("xsavepolicy", Major = 6, Minor = 1)]
        [Undocumented]
        XSavePolicy = Application | Integer | 0x00000120,

        [BcdEditName("xsaveaddfeature0", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature0 = Application | Integer | 0x00000121,

        [BcdEditName("xsaveaddfeature1", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature1 = Application | Integer | 0x00000122,

        [BcdEditName("xsaveaddfeature2", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature2 = Application | Integer | 0x00000123,

        [BcdEditName("xsaveaddfeature3", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature3 = Application | Integer | 0x00000124,

        [BcdEditName("xsaveaddfeature4", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature4 = Application | Integer | 0x00000125,

        [BcdEditName("xsaveaddfeature5", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature5 = Application | Integer | 0x00000126,

        [BcdEditName("xsaveaddfeature6", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature6 = Application | Integer | 0x00000127,

        [BcdEditName("xsaveaddfeature7", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveAddFeature7 = Application | Integer | 0x00000128,

        [BcdEditName("xsaveremovefeature", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveRemoveFeature = Application | Integer | 0x00000129,

        [BcdEditName("xsaveprocessorsmask", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveProcessorsMask = Application | Integer | 0x0000012A,

        [BcdEditName("xsavedisable", Major = 6, Minor = 1)]
        [Undocumented]
        XSaveDisable = Application | Integer | 0x0000012B,

        /// <remarks>
        /// 0 = Serial
        /// 1 = 1394
        /// 2 = USB
        /// 3 = NET
        /// </remarks>
        [BcdEditName("kerneldebugtype", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerType = Application | Integer | 0x0000012C,

        [BcdEditName("kernelbusparams", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerBusParameters = Application | String | 0x0000012D,

        [BcdEditName("kerneldebugaddress", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerPortAddress = Application | Integer | 0x0000012E,

        [BcdEditName("kerneldebugport", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerPortNumber = Application | Integer | 0x0000012F,

        [BcdEditName("claimedtpmcounter", Major = 6, Minor = 2)]
        [Undocumented]
        KernelDebuggerClaimedDeviceLockCounter = Application | Integer | 0x00000130,

        [BcdEditName("kernelchannel", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebugger1394Channel = Application | Integer | 0x00000131,

        [BcdEditName("kerneltargetname", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerUsbTargetName = Application | String | 0x00000132,

        [BcdEditName("kernelhostip", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerNetHostIp = Application | Integer | 0x00000133,

        [BcdEditName("kernelport", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerNetHostPort = Application | Integer | 0x00000134,

        [BcdEditName("kerneldhcp", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerNetDhcp = Application | Boolean | 0x00000135,

        [BcdEditName("kernelkey", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerNetKey = Application | String | 0x00000136,

        [BcdEditName("imchivename", Major = 6, Minor = 3)]
        [Undocumented]
        ImcHiveName = Application | String | 0x00000137,

        [BcdEditName("imcdevice", Major = 6, Minor = 3)]
        [Undocumented]
        ImcDevice = Application | DevicePath | 0x00000138,

        [BcdEditName("kernelbaudrate", Major = 6, Minor = 3)]
        [Undocumented]
        KernelDebuggerBaudRate = Application | Integer | 0x00000139,

        [BcdEditName("mfgmode", Major = 10, Minor = 0)]
        [Undocumented]
        ManufacturingMode = Application | String | 0x00000140,

        [BcdEditName("event", Major = 10, Minor = 0)]
        EventLoggingEnabled = Application | Boolean | 0x00000141,

        /// <remarks>
        /// 0 = Off
        /// 1 = Auto
        /// </remarks>
        [BcdEditName("vsmlaunchtype", Major = 10, Minor = 0)]
        VsmLaunchType = Application | Integer | 0x00000142,

        /// <remarks>
        /// 	0 = Disable
        /// 1 = Enable
        /// 2 = Strict
        /// </remarks>
        [BcdEditName("hypervisorenforcedcodeintegrity", Major = 10, Minor = 0)]
        [Undocumented]
        HypervisorEnforcedCodeIntegrity = Application | Integer | 0x00000144,

        [BcdEditName("dtrace", Major = 10, Minor = 0)]
        [Undocumented]
        DTraceEnabled = Application | Boolean | 0x00000145,

        [BcdEditName("systemdatadevice")]
        [Undocumented]
        SystemDataDevice = Application | DevicePath | 0x00000150,

        [BcdEditName("osarcdevice", Major = 10, Minor = 0)]
        [Undocumented]
        OperatingSystemArcDevice = Application | DevicePath | 0x00000151,

        [Undocumented]
        X21000152 = Application | DevicePath | 0x00000152,

        [BcdEditName("osdatadevice")]
        [Undocumented]
        OperatingSystemDataDevice = Application | DevicePath | 0x00000153,

        [BcdEditName("bspdevice")]
        [Undocumented]
        BspDevice = Application | DevicePath | 0x00000154,

        [BcdEditName("bspfilepath")]
        [Undocumented]
        BspFilePath = Application | DevicePath | 0x00000155,

        [BcdEditName("kernelhostipv6")]
        [Undocumented]
        KernelDebuggerNetHostIPv6 = Application | String | 0x00000156,

        [BcdEditName("hypervisorhostipv6")]
        [Undocumented]
        HypervisorDebuggerNetHostIpv6 = Application | String | 0x00000161,

        [BcdEditName("filedevice")]
        HiberFileDevice = Application | DevicePath | 0x00000001,

        [BcdEditName("filepath")]
        HiberFilePath = Application | String | 0x00000002,

        [BcdEditName("customsettings")]
        UseCustomSettings = Application | Boolean | 0x00000003,

        [BcdEditName("pae")]
        X86PaeMode = Application | Boolean | 0x00000004,

        [BcdEditName("associatedosdevice")]
        AssociatedOsDevice = Application | DevicePath | 0x00000005,

        [BcdEditName("debugoptionenabled")]
        [Undocumented]
        DebugOptionEnabled = Application | Boolean | 0x00000006,

        /// <remarks>
        /// 0 = Disabled
        /// 1 = Basic
        /// 2 = Standard
        /// </remarks>
        [BcdEditName("bootux")]
        ResumeBootUxPolicy = Application | Integer | 0x00000007,

        /// <remarks>
        /// 0 = Legacy
        /// 1 = Standard
        /// </remarks>
        [BcdEditName("bootmenupolicy")]
        ResumeBootMenuPolicy = Application | Integer | 0x00000008,

        [Undocumented]
        ResumeHormEnabled = Application | Boolean | 0x00000024,

        [BcdEditName("passcount")]
        [Undocumented]
        PassCount = Application | Integer | 0x00000001,

        /// <remarks>
        /// 0 = Basic
        /// 1 = Extended
        /// </remarks>
        [BcdEditName("testmix")]
        TestMix = Application | Integer | 0x00000002,

        [BcdEditName("failurecount")]
        [Undocumented]
        FailureCount = Application | Integer | 0x00000003,

        [BcdEditName("cacheenable", Major = 6, Minor = 2)]
        CacheEnable = Application | Boolean | 0x00000003,

        /// <remarks>
        /// 0 = Stride
        /// 1 = Mats
        /// 2 = InverseCoupling
        /// 3 = RandomPattern
        /// 4 = Checkerboard
        /// </remarks>
        [BcdEditName("testtofail")]
        [Undocumented]
        TestToFail = Application | Integer | 0x00000004,

        [BcdEditName("failuresenabled", Major = 6, Minor = 2)]
        [Undocumented]
        FailuresEnabled = Application | Boolean | 0x00000004,

        [BcdEditName("cacheenable", Major = 6, Minor = 1)]
        CacheEnableLegacy = Application | Boolean | 0x00000005,

        [BcdEditName("stridefailcount", Major = 6, Minor = 2)]
        [Undocumented]
        StrideFailureCount = Application | Integer | 0x00000005,

        [BcdEditName("invcfailcount", Major = 6, Minor = 2)]
        [Undocumented]
        InvcFailureCount = Application | Integer | 0x00000006,

        [BcdEditName("matsfailcount", Major = 6, Minor = 2)]
        [Undocumented]
        MatsFailureCount = Application | Integer | 0x00000007,

        [BcdEditName("randfailcount", Major = 6, Minor = 2)]
        [Undocumented]
        RandFailureCount = Application | Integer | 0x00000008,

        [BcdEditName("chckrfailcount", Major = 6, Minor = 2)]
        [Undocumented]
        ChckrFailureCount = Application | Integer | 0x00000009,

        [BcdEditName("bpbstring")]
        [Undocumented]
        BpbString = Application | String | 0x00000001,

        [BcdEditName("pxesoftreboot")]
        [Undocumented]
        PxeSoftReboot = Application | Boolean | 0x00000001,

        [BcdEditName("applicationname")]
        [Undocumented]
        PxeApplicationName = Application | String | 0x00000002,

        [BcdEditName("enablebootdebugpolicy")]
        [Undocumented]
        EnableBootDebugPolicy = Application | Boolean | 0x00000145,

        [BcdEditName("enablebootorderclean")]
        [Undocumented]
        EnableBootOrderClean = Application | Boolean | 0x00000146,

        [BcdEditName("enabledeviceid")]
        [Undocumented]
        EnableDeviceID = Application | Boolean | 0x00000147,

        [BcdEditName("enableffuloader")]
        [Undocumented]
        EnableFfuLoader = Application | Boolean | 0x00000148,

        [BcdEditName("enableiuloader")]
        [Undocumented]
        EnableIuLoader = Application | Boolean | 0x00000149,

        [BcdEditName("enablemassstorage")]
        [Undocumented]
        EnableMassStorage = Application | Boolean | 0x0000014A,

        [BcdEditName("enablerpmbprovisioning")]
        [Undocumented]
        EnableRpmbProvisioning = Application | Boolean | 0x0000014B,

        [BcdEditName("enablesecurebootpolicy")]
        [Undocumented]
        EnableSecureBootPolicy = Application | Boolean | 0x0000014C,

        [BcdEditName("enablestartcharge")]
        [Undocumented]
        EnableStartCharge = Application | Boolean | 0x0000014D,

        [BcdEditName("enableresettpm")]
        [Undocumented]
        EnableResetTpm = Application | Boolean | 0x0000014E,

        [BcdEditName("ramdiskimageoffset")]
        RamdiskImageOffset = Device | Integer | 0x00000001,

        [BcdEditName("ramdisktftpclientport")]
        RamdiskTftpClientPort = Device | Integer | 0x00000002,

        [BcdEditName("ramdisksdidevice")]
        RamdiskSdiDevice = Device | DevicePath | 0x00000003,

        [BcdEditName("ramdisksdipath")]
        RamdiskSdiPath = Device | String | 0x00000004,

        [BcdEditName("ramdiskimagelength")]
        RamdiskImageLength = Device | Integer | 0x00000005,

        [BcdEditName("exportascd")]
        RamdiskExportAsCd = Device | Boolean | 0x00000006,

        [BcdEditName("ramdisktftpblocksize")]
        RamdiskTftpBlockSize = Device | Integer | 0x00000007,

        [BcdEditName("ramdisktftpwindowsize")]
        RamdiskTftpWindowSize = Device | Integer | 0x00000008,

        [BcdEditName("ramdiskmcenabled", Major = 6, Minor = 1)]
        RamdiskMulticastEnabled = Device | Boolean | 0x00000009,

        [BcdEditName("ramdiskmctftpfallback", Major = 6, Minor = 1)]
        RamdiskMulticastTftpFallback = Device | Boolean | 0x0000000A,

        [BcdEditName("ramdisktftpvarwindow", Major = 6, Minor = 2)]
        RamdiskTftpVarWindow = Device | Boolean | 0x0000000B,

        [BcdEditName("vhdramdiskboot")]
        [Undocumented]
        VhdRamdiskBoot = Device | Boolean | 0x0000000C,

        [Undocumented]
        X3500000D = Device | Integer | 0x0000000D,
    }
}
