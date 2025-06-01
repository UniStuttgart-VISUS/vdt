// <copyright file="BcdObjectType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Diagnostics.CodeAnalysis;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Possible BCD object types stored in the &quot;Description&quot;
    /// sub-key as described on
    /// https://www.geoffchappell.com/notes/windows/boot/bcd/objects.htm.
    /// </summary>
    [Flags]
    public enum BcdObjectType : uint {

        /// <summary>
        /// A bitmask for isolating the object type from a
        /// <see cref="BcdObjectType"/>.
        /// </summary>
        ObjectTypeMask = 0xF0000000,

        /// <summary>
        /// A bitmask for isolating the image type from a
        /// <see cref="BcdObjectType"/>.
        /// </summary>
        ImageTypeMask = 0x00F00000,

        /// <summary>
        /// A bitmask for isolating the inheritance type from a
        /// <see cref="BcdObjectType"/>.
        /// </summary>
        InheritableMask = 0x00F00000,

        /// <summary>
        /// A bitmask for isolating the application type from a
        /// <see cref="BcdObjectType"/>.
        /// </summary>
        ApplicationTypeMask = 0x000FFFFF,

        /// <summary>
        /// Identifies an object as an application.
        /// </summary>
        ApplicationObject = 0x10000000,

        /// <summary>
        /// Identifies an object as inherited.
        /// </summary>
        InheritObject = 0x20000000,

        /// <summary>
        /// Identifies an object as a device.
        /// </summary>
        DeviceObject = 0x20000000,

        FirmwareApplication = 0x00100000,

        WindowsBootApplication = 0x00200000,

        LegacyLoaderApplication = 0x00300000,

        RealModeApplication = 0x00400000,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "Different semantics depending on object type mask.")]
        InheritableByAll = 0x00100000,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "Different semantics depending on object type mask.")]
        InheritableByApplication = 0x00200000,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "Different semantics depending on object type mask.")]
        InheritableByDevice = 0x00300000,

        FirmwareBootManagerApplication = 0x00000001,

        BootManagerApplication = 0x00000002,

        OperatingSystemLoaderApplication = 0x00000003,

        ResumeApplication = 0x00000004,

        MemoryDiagnosticApplication = 0x00000005,

        NtLoaderApplication = 0x00000006,

        SetupLoaderApplication = 0x00000007,

        BootsectorApplication = 0x00000008,

        StartupApplication = 0x00000009,

        BootAppApplication = 0x0000000A,

        [BcdEditName("{fwbootmgr}")]
        FirmwareBootManager
            = ApplicationObject
            | FirmwareApplication
            | FirmwareBootManagerApplication,

        [BcdEditName("{bootmgr}")]
        BootManager
            = ApplicationObject
            | FirmwareApplication
            | BootManagerApplication,

        OperatingSystemLoader
            = ApplicationObject
            | WindowsBootApplication
            | OperatingSystemLoaderApplication,

        Resume
            = ApplicationObject
            | WindowsBootApplication
            | ResumeApplication,

        [BcdEditName("{memdiag}")]
        MemoryDiagnostic
            = ApplicationObject
            | WindowsBootApplication
            | MemoryDiagnosticApplication,

        [BcdEditName("{ntldr}")]
        NtLoader
            = ApplicationObject
            | LegacyLoaderApplication
            | NtLoaderApplication,

        SetupLoader
            = ApplicationObject
            | RealModeApplication
            | SetupLoaderApplication,

        Bootsector
            = ApplicationObject
            | RealModeApplication
            | BootsectorApplication,

        Startup
            = ApplicationObject
            | RealModeApplication
            | StartupApplication,

        BootApp
            = ApplicationObject
            | WindowsBootApplication
            | BootAppApplication,

        Inherit = InheritObject |FirmwareApplication,

        [BcdEditName("{badmemory}")]
        BadMemory = InheritObject | FirmwareApplication,

        [BcdEditName("{dbgsettings}")]
        DebugSettings = InheritObject | FirmwareApplication,

        [BcdEditName("{emssettings}")]
        EmsSettings = InheritObject | FirmwareApplication,

        [BcdEditName("{globalsettings}")]
        GlobalSettings = InheritObject | FirmwareApplication,

        InheritFirmwareBootManager
            = InheritObject
            | WindowsBootApplication
            | FirmwareBootManagerApplication,

        InheritBootManager
            = InheritObject
            | WindowsBootApplication
            | BootManagerApplication,

        InheritOperatingSystemLoader
            = InheritObject
            | WindowsBootApplication
            | OperatingSystemLoaderApplication,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "This mask is context-dependent.")]
        [BcdEditName("{bootloadersettings}")]
        BootLoaderSettings
            = InheritObject
            | WindowsBootApplication
            | OperatingSystemLoaderApplication,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "This mask is context-dependent.")]
        [BcdEditName("{hypervisorsettings}")]
        HypervisorSettings
            = InheritObject
            | WindowsBootApplication
            | OperatingSystemLoaderApplication,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "This mask is context-dependent.")]
        [BcdEditName("{kerneldbgsettings}", Major = 6, Minor = 3)]
        KernelDebuggerSettings
            = InheritObject
            | WindowsBootApplication
            | OperatingSystemLoaderApplication,

        InheritResume
            = InheritObject
            | WindowsBootApplication
            | ResumeApplication,

        [SuppressMessage("Design",
            "CA1069:Enums values should not be duplicated",
            Justification = "This mask is context-dependent.")]
        [BcdEditName("{resumeloadersettings}")]
        ResumeLoaderSettings
            = InheritObject
            | WindowsBootApplication
            | ResumeApplication,

        InheritMemoryDiagnostic
            = InheritObject
            | WindowsBootApplication
            | MemoryDiagnosticApplication,

        InheritNtLoader
            = InheritObject
            | WindowsBootApplication
            | NtLoaderApplication,

        InheritSetupLoader
            = InheritObject
            | WindowsBootApplication
            | SetupLoaderApplication,

        InheritBootsector
            = InheritObject
            | WindowsBootApplication
            | BootsectorApplication,

        InheritStartup
            = InheritObject
            | WindowsBootApplication
            | StartupApplication,

        InheritDevice
            = InheritObject
            | LegacyLoaderApplication,

        [BcdEditName("{ramdiskoptions}")]
        Device = DeviceObject

    }
}
