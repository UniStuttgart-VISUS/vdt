// <copyright file="WellKnownBcdObject.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Enumerate the types of BCD object as described on
    /// https://www.geoffchappell.com/notes/windows/boot/bcd/objects.htm.
    /// </summary>
    public enum WellKnownBcdObject {

        [BcdID("{0CE4991B-E6B3-4B16-B23C-5E0D9250E5D9}")]
        [BcdEditName("{emssettings}")]
        EmsSettingsGroup,

        [BcdID("{1AFA9C49-16AB-4A5C-4A90-212802DA9460}")]
        [BcdEditName("{resumeloadersettings}")]
        ResumeLoaderSettingsGroup,

        [BcdID("{1CAE1EB7-A0DF-4D4D-9851-4860E34EF535}")]
        [BcdEditName("{default}")]
        DefaultBootEntry,

        [BcdID("{313E8EED-7098-4586-A9BF-309C61F8D449}")]
        [BcdEditName("{kerneldbgsettings}")]
        KernelDebuggerSettingsGroup,

        [BcdID("{4636856E-540F-4170-A130-A84776F4C654}")]
        [BcdEditName("{dbgsettings}")]
        [BcdEditName("{eventsettings}", Major = 10)]
        DebuggerSettingsGroup,

        [BcdID("{466F5A88-0AF2-4F76-9038-095B170DC21C}")]
        [BcdEditName("{legacy}")]
        [BcdEditName("{ntldr}")]
        WindowsLegacyNtldr,

        [BcdID("{5189B25C-5558-4BF2-BCA4-289B11BD29E2}")]
        [BcdEditName("{badmemory}")]
        BadMemoryGroup,

        [BcdID("{6EFB52BF-1766-41DB-A6B3-0EE5EFF72BD7}")]
        [BcdEditName("{bootloadersettings}")]
        BootLoaderSettingsGroup,

        [BcdID("{7254A080-1510-4E85-AC0F-E7FB3D444736}")]
        [Undocumented]
        WindowsSetupEfi,

        [BcdID("{7EA2E1AC-2E61-4728-AAA3-896D9D0A9F0E}")]
        [BcdEditName("{globalsettings}")]
        GlobalSettingsGroup,

        [BcdID("{7FF607E0-4395-11DB-B0DE-0800200C9A66}")]
        [BcdEditName("{hypervisorsettings}")]
        HypervisorSettingsGroup,

        [BcdID("{9DEA862C-5CDD-4E70-ACC1-F32B344D4795}")]
        [BcdEditName("{bootmgr}")]
        WindowsBootmgr,

        [BcdID("{A1943BBC-EA85-487C-97C7-C9EDE908A38A}")]
        [Undocumented]
        WindowsOsTargetTemplatePcat,

        [BcdID("{A5A30FA2-3D06-4E9F-B5F4-A01DF9D1FCBA}")]
        [BcdEditName("{fwbootmgr}")]
        FirmwareBootmgr,

        [BcdID("{AE5534E0-A924-466C-B836-758539A3EE3A}")]
        [BcdEditName("{ramdiskoptions}")]
        WindowsSetupRamdiskOptions,

        [BcdID("{B012B84D-C47C-4ED5-B722-C0C42163E569}")]
        WindowsOsTargetTemplateEfi,

        [BcdID("{B2721D73-1DB4-4C62-BF78-C548A880142D}")]
        [BcdEditName("{memdiag}")]
        WindowsMemoryTester,

        [BcdID("{CBD971BF-B7B8-4885-951A-FA03044F5D71}")]
        [Undocumented]
        WindowsSetupPcat,

        [BcdID("{FA926493-6F1C-4193-A414-58F0B2456D1E}")]
        [BcdEditName("{current}")]
        CurrentBootEntry
    }
}
