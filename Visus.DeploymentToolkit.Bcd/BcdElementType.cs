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

        //0x12000016	BcdLibraryString_UsbDebuggerTargetName
        //BCDE_LIBRARY_TYPE_DEBUGGER_USB_TARGETNAME	targetname	string	6.0 and higher
        //0x16000017	BcdLibraryBoolean_DebuggerIgnoreUsermodeExceptions
        //BCDE_LIBRARY_TYPE_DEBUGGER_IGNORE_USERMODE_EXCEPTIONS	noumex	boolean	6.0 and higher
        //0x15000018	BcdLibraryInteger_DebuggerStartPolicy
        //BCDE_LIBRARY_TYPE_DEBUGGER_START_POLICY	debugstart	0 = Active
        //1 = AutoEnable
        //2 = Disable	6.0 and higher
        //0x12000019	BCDE_LIBRARY_TYPE_DEBUGGER_BUS_PARAMETERS	busparams	string	6.1 and higher
        //BcdLibraryString_DebuggerBusParameters	 	 	6.2 and higher
        //0x1500001A	BcdLibraryInteger_DebuggerNetHostIP
        //BCDE_LIBRARY_TYPE_DEBUGGER_NET_HOST_IP	hostip	integer	6.2 and higher
        //0x1500001B	BcdLibraryInteger_DebuggerNetPort
        //BCDE_LIBRARY_TYPE_DEBUGGER_NET_PORT	port	integer	6.2 and higher
        //0x1600001C	BcdLibraryBoolean_DebuggerNetDhcp
        //BCDE_LIBRARY_TYPE_DEBUGGER_NET_DHCP	dhcp	boolean	6.2 and higher
        //0x1200001D	BcdLibraryString_DebuggerNetKey
        //BCDE_LIBRARY_TYPE_DEBUGGER_NET_KEY	key	string	6.2 and higher
        //0x1600001E	BCDE_LIBRARY_TYPE_DEBUGGER_NET_VM	vm	boolean	6.2 and higher
        //0x1200001F	BCDE_LIBRARY_TYPE_DEBUGGER_NET_HOST_IPV6	hostipv6	string	1809 and higher
        //0x16000020	BcdLibraryBoolean_EmsEnabled
        //BCDE_LIBRARY_TYPE_EMS_ENABLED	bootems	boolean	6.0 and higher
        //0x15000022	BcdLibraryInteger_EmsPort
        //BCDE_LIBRARY_TYPE_EMS_PORT_NUMBER	emsport	integer	6.0 and higher
        //0x15000023	BcdLibraryInteger_EmsBaudRate
        //BCDE_LIBRARY_TYPE_EMS_BAUDRATE	emsbaudrate	integer	6.0 and higher
        //0x12000030	BcdLibraryString_LoadOptionsString
        //BCDE_LIBRARY_TYPE_LOAD_OPTIONS_STRING	loadoptions	string	6.0 and higher
        //0x16000031	BcdLibraryBoolean_AttemptNonBcdStart	 	boolean	6.0 to 6.1
        //0x16000040	BcdLibraryBoolean_DisplayAdvancedOptions
        //BCDE_LIBRARY_TYPE_DISPLAY_ADVANCED_OPTIONS	advancedoptions	boolean	6.0 and higher
        //0x16000041	BcdLibraryBoolean_DisplayOptionsEdit
        //BCDE_LIBRARY_TYPE_DISPLAY_OPTIONS_EDIT	optionsedit	boolean	6.0 and higher
        //0x15000042	BcdLibraryInteger_FVEKeyRingAddress
        //BCDE_LIBRARY_TYPE_FVE_KEYRING_ADDRESS	keyringaddress	integer	6.0 and higher
        //0x11000043	device for boot status data log	 	device	6.0 and higher
        //BcdLibraryDevice_BsdLogDevice	 	 	6.2 and higher
        //BCDE_LIBRARY_TYPE_BSD_LOG_DEVICE	bootstatdevice	 	1803 and higher
        //0x12000044	file path for boot status data log	 	string	6.0 and higher
        //BcdLibraryString_BsdLogPath	 	 	6.2 and higher
        //BCDE_LIBRARY_TYPE_BSD_LOG_PATH	bootstatfilepath	 	1803 and higher
        //0x16000045	append to boot status data log	 	boolean	6.0 and higher
        //BcdLibraryBoolean_BsdPreserveLog
        // 	 	6.2 and higher
        //BCDE_LIBRARY_TYPE_BSD_PRESERVE_LOG	preservebootstat	 	10.0 and higher
        //0x16000046	BcdLibraryBoolean_GraphicsModeDisabled
        //BCDE_LIBRARY_TYPE_GRAPHICS_MODE_DISABLED	graphicsmodedisabled	boolean	6.0 and higher
        //0x15000047	BcdLibraryInteger_ConfigAccessPolicy
        //BCDE_LIBRARY_TYPE_CONFIG_ACCESS_POLICY	configaccesspolicy	0 = Default
        //1 = DisallowMmConfig	6.0 and higher
        //0x16000048	BcdLibraryBoolean_DisableIntegrityChecks
        //BCDE_LIBRARY_TYPE_DISABLE_INTEGRITY_CHECKS	nointegritychecks	boolean	6.0 and higher
        //0x16000049	BcdLibraryBoolean_AllowPrereleaseSignatures
        //BCDE_LIBRARY_TYPE_ALLOW_PRERELEASE_SIGNATURES	testsigning	boolean	6.0 and higher
        //0x1200004A	BCDE_LIBRARY_TYPE_FONT_PATH	fontpath	string	6.0 SP1 and higher
        //BcdLibraryString_FontPath	 	 	6.2 and higher
        //0x1500004B	BcdLibraryInteger_SiPolicy
        //BCDE_LIBRARY_TYPE_SI_POLICY	integrityservices	0 = Default
        //1 = Enable
        //2 = Disable	6.1 and higher
        //0x1500004C	BcdLibraryInteger_FveBandId
        //BCDE_LIBRARY_TYPE_FVE_BAND_ID	volumebandid	integer	6.2 and higher
        //0x16000050	BCDE_LIBRARY_TYPE_CONSOLE_EXTENDED_INPUT	extendedinput	boolean	6.0 and higher
        //BcdLibraryBoolean_ConsoleExtendedInput	 	 	6.2 and higher
        //0x15000051	BCDE_LIBRARY_TYPE_INITIAL_CONSOLE_INPUT	initialconsoleinput	integer	6.0 and higher
        //BcdLibraryInteger_InitialConsoleInput	 	 	6.2 and higher
        //0x15000052	BCDE_LIBRARY_TYPE_GRAPHICS_RESOLUTION	graphicsresolution	0 = 1024x768
        //1 = 800x600	6.0 SP1 and higher
        //2 = 1024x600	6.2 and higher
        //BcdLibraryInteger_GraphicsResolution	 	 	6.2 and higher
        //0x16000053	BcdLibraryBoolean_RestartOnFailure
        //BCDE_LIBRARY_TYPE_RESTART_ON_FAILURE	restartonfailure	boolean	6.2 and higher
        //0x16000054	BcdLibraryBoolean_GraphicsForceHighestMode
        //BCDE_LIBRARY_TYPE_GRAPHICS_FORCE_HIGHEST_MODE	highestmode	boolean	6.2 and higher
        //0x16000060	BcdLibraryBoolean_IsolatedExecutionContext
        //BCDE_LIBRARY_TYPE_ISOLATED_EXECUTION_CONTEXT	isolatedcontext	boolean	6.2 and higher
        //0x15000065	BcdLibraryInteger_BootUxDisplayMessage
        //BCDE_LIBRARY_TYPE_BOOTUX_DISPLAY_MESSAGE	displaymessage	0 = Default
        //1 = Resume
        //2 = HyperV
        //3 = Recovery
        //4 = StartupRepair
        //5 = SystemImageRecovery
        //6 = CommandPrompt
        //7 = SystemRestore
        //8 = PushButtonReset	6.2 and higher
        //0x15000066	BcdLibraryInteger_BootUxDisplayMessageOverride
        //BCDE_LIBRARY_TYPE_BOOTUX_DISPLAY_MESSAGE_OVERRIDE	displaymessageoverride	0 = Default
        //1 = Resume
        //2 = HyperV
        //3 = Recovery
        //4 = StartupRepair
        //5 = SystemImageRecovery
        //6 = CommandPrompt
        //7 = SystemRestore
        //8 = PushButtonReset	6.2 and higher
        //0x16000067	BcdLibraryBoolean_BootUxLogoDisable	 	boolean	6.2 and higher
        //0x16000068	BcdLibraryBoolean_BootUxTextDisable	 	boolean	6.2 and higher
        //BCDE_LIBRARY_TYPE_BOOTUX_TEXT_DISABLE	nobootuxtext	 	10.0 and higher
        //0x16000069	BcdLibraryBoolean_BootUxProgressDisable	 	boolean	6.2 and higher
        //BCDE_LIBRARY_TYPE_BOOTUX_PROGRESS_DISABLE	nobootuxprogress	 	10.0 and higher
        //0x1600006A	BcdLibraryBoolean_BootUxFadeDisable	 	boolean	6.2 and higher
        //BCDE_LIBRARY_TYPE_BOOTUX_FADE_DISABLE	nobootuxfade	 	10.0 and higher
        //0x1600006B	BcdLibraryBoolean_BootUxReservePoolDebug	 	boolean	6.2 and higher
        //0x1600006C	BcdLibraryBoolean_BootUxDisable
        //BCDE_LIBRARY_TYPE_BOOTUX_DISABLE	bootuxdisabled	boolean	6.2 and higher
        //0x1500006D	BcdLibraryInteger_BootUxFadeFrames	 	integer	6.2 and higher
        //0x1600006E	BcdLibraryBoolean_BootUxDumpStats	 	boolean	6.2 and higher
        //0x1600006F	BcdLibraryBoolean_BootUxShowStats	 	boolean	6.2 and higher
        //0x16000071	BcdLibraryBoolean_MultiBootSystem	 	boolean	6.2 and higher
        //0x16000072	BcdLibraryBoolean_ForceNoKeyboard
        //BCDE_LIBRARY_TYPE_FORCE_NO_KEYBOARD	nokeyboard	boolean	6.2 and higher
        //0x15000073	BcdLibraryInteger_AliasWindowsKey	 	integer	6.2 and higher
        //0x16000074	BcdLibraryBoolean_BootShutdownDisabled
        //BCDE_LIBRARY_TYPE_BOOT_SHUTDOWN_DISABLED	bootshutdowndisabled	boolean	6.2 and higher
        //0x15000075	BcdLibraryInteger_PerformanceFrequency	 	integer	6.2 and higher
        //0x15000076	BcdLibraryInteger_SecurebootRawPolicy	 	integer	6.2 and higher
        //0x17000077	BcdLibraryIntegerList_AllowedInMemorySettings
        //BCDE_LIBRARY_TYPE_ALLOWED_IN_MEMORY_SETTINGS	allowedinmemorysettings	integer list	6.2 and higher
        //0x16000078	 	 	boolean	10.0 and higher
        //0x15000079	BCDE_LIBRARY_TYPE_BOOTUX_BITMAP_TRANSITION_TIME	bootuxtransitiontime	integer	10.0 and higher
        //0x1600007A	BCDE_LIBRARY_TYPE_TWO_BOOT_IMAGES	mobilegraphics	boolean	10.0 and higher
        //0x1600007B	BcdLibraryBoolean_ForceFipsCrypto
        //BCDE_LIBRARY_TYPE_FORCE_FIPS_CRYPTO	forcefipscrypto	boolean	6.2 and higher
        //0x1500007D	BcdLibraryInteger_BootErrorUx
        //BCDE_LIBRARY_TYPE_BOOT_ERROR_UX	booterrorux	0 = Legacy
        //1 = Standard
        //2 = Simple	6.3 and higher
        //0x1600007E	BcdLibraryBoolean_AllowFlightSignatures
        //BCDE_LIBRARY_TYPE_ALLOW_FLIGHT_SIGNATURES	flightsigning	boolean	10.0 and higher
        //0x1500007F	BcdLibraryInteger_BootMeasurementLogFormat
        //BCDE_LIBRARY_TYPE_BOOT_LOG_FORMAT	measuredbootlogformat	0 = Default
        //1 = Sha1	10.0 and higher
        //0x15000080	BCDE_LIBRARY_TYPE_DISPLAY_ROTATION	displayrotation	0 = 0
        //1 = 90
        //2 = 180
        //3 = 270	1607 and higher
        //0x15000081	BCDE_LIBRARY_TYPE_LOG_CONTROL	logcontrol	integer	1703 and higher
        //0x16000082	BCDE_LIBRARY_TYPE_NO_FIRMWARE_SYNC	nofirmwaresync	boolean	1709 and higher
        //0x11000083	 	 	device	1709 and higher
        //0x11000084	BCDE_LIBRARY_TYPE_WINDOWS_SYSTEM_DEVICE	windowssyspart	device	1803 and higher
        //0x16000085	 	 	boolean	1803 and higher
        //0x15000086	 	 	integer	1809 and higher (x64 only)
        //0x16000087	BCDE_LIBRARY_TYPE_NUM_LOCK_ON	numlock	boolean	1803 and higher
        //0x12000088	BcdLibraryBoolean_AdditionalCiPolicy
        //BCDE_LIBRARY_TYPE_ADDITIONAL_CIPOLICY	additionalcipolicy	string	1809 and higher
        //0x15000088	BCDE_LIBRARY_TYPE_LINEAR_ADDRESS_57_POLICY	linearaddress57	0 = default
        //1 = optout
        //2 = optin	1809 and higher

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
    }
}
