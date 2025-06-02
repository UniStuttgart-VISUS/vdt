// <copyright file="BcdTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Security.Principal;
using Visus.DeploymentToolkit.Bcd;
using Visus.DeploymentToolkit.Security;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Test {

    [TestClass]
    public sealed class BcdTest {

        [TestMethod]
        public void TestBcdObjectType() {
            // Test against constants from https://www.geoffchappell.com/notes/windows/boot/bcd/objects.htm
            Assert.AreEqual(0xF0000000u, (uint) BcdObjectType.ObjectTypeMask);
            Assert.AreEqual(0x00F00000u, (uint) BcdObjectType.ImageTypeMask);
            Assert.AreEqual(0x00F00000u, (uint) BcdObjectType.InheritableMask);
            
            Assert.AreEqual(0x000FFFFFu, (uint) BcdObjectType.ApplicationTypeMask);
            Assert.AreEqual(0x10100001u, (uint) BcdObjectType.FirmwareBootManager);
            Assert.AreEqual(0x10100002u, (uint) BcdObjectType.BootManager);
            Assert.AreEqual(0x10200003u, (uint) BcdObjectType.OperatingSystemLoader);
            Assert.AreEqual(0x10200004u, (uint) BcdObjectType.Resume);
            Assert.AreEqual(0x10200005u, (uint) BcdObjectType.MemoryDiagnostic);
            Assert.AreEqual(0x10300006u, (uint) BcdObjectType.NtLoader);
            Assert.AreEqual(0x10400008u, (uint) BcdObjectType.Bootsector);
            Assert.AreEqual(0x10400009u, (uint) BcdObjectType.Startup);
            Assert.AreEqual(0x1020000Au, (uint) BcdObjectType.BootApp);

            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.Inherit);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.BadMemory);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.DebugSettings);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.EmsSettings);
            Assert.AreEqual(0x20100000u, (uint) BcdObjectType.GlobalSettings);
            Assert.AreEqual(0x20200001u, (uint) BcdObjectType.InheritFirmwareBootManager);
            Assert.AreEqual(0x20200002u, (uint) BcdObjectType.InheritBootManager);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.InheritOperatingSystemLoader);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.BootLoaderSettings);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.HypervisorSettings);
            Assert.AreEqual(0x20200003u, (uint) BcdObjectType.KernelDebuggerSettings);
            Assert.AreEqual(0x20200004u, (uint) BcdObjectType.InheritResume);
            Assert.AreEqual(0x20200004u, (uint) BcdObjectType.ResumeLoaderSettings);
            Assert.AreEqual(0x20200005u, (uint) BcdObjectType.InheritMemoryDiagnostic);
            Assert.AreEqual(0x20200006u, (uint) BcdObjectType.InheritNtLoader);
            Assert.AreEqual(0x20200007u, (uint) BcdObjectType.InheritSetupLoader);
            Assert.AreEqual(0x20200008u, (uint) BcdObjectType.InheritBootsector);
            Assert.AreEqual(0x20200009u, (uint) BcdObjectType.InheritStartup);
            Assert.AreEqual(0x20300000u, (uint) BcdObjectType.InheritDevice);
        }

        [TestMethod]
        public void TestBcdElementType() {
            // Library elements
            Assert.AreEqual(0x11000001u, (uint) BcdElementType.LibraryApplicationDevice);
            Assert.AreEqual(0x12000002u, (uint) BcdElementType.LibraryApplicationPath);
            Assert.AreEqual(0x12000004u, (uint) BcdElementType.LibraryDescription);
            Assert.AreEqual(0x12000005u, (uint) BcdElementType.LibraryLocale);
            Assert.AreEqual(0x14000006u, (uint) BcdElementType.LibraryInheritObjects);
            Assert.AreEqual(0x15000007u, (uint) BcdElementType.LibraryTruncatePhysicalMemory);
            Assert.AreEqual(0x14000008u, (uint) BcdElementType.LibraryRecoverySequence);
            Assert.AreEqual(0x16000009u, (uint) BcdElementType.LibraryAutoRecoveryEnabled);
            Assert.AreEqual(0x1700000Au, (uint) BcdElementType.LibraryBadMemoryList);
            Assert.AreEqual(0x1600000Bu, (uint) BcdElementType.LibraryAllowBadMemoryAccess);
            Assert.AreEqual(0x1500000Cu, (uint) BcdElementType.LibraryFirstMegabytePolicy);
            Assert.AreEqual(0x1500000Du, (uint) BcdElementType.LibraryRelocatePhysicalMemory);
            Assert.AreEqual(0x1500000Eu, (uint) BcdElementType.LibraryAvoidLowPhysicalMemory);
            Assert.AreEqual(0x1600000Fu, (uint) BcdElementType.LibraryTraditionalKsegMappings);
            Assert.AreEqual(0x16000010u, (uint) BcdElementType.LibraryDebuggerEnabled);
            Assert.AreEqual(0x15000011u, (uint) BcdElementType.LibraryDebuggerType);
            Assert.AreEqual(0x15000012u, (uint) BcdElementType.LibrarySerialDebuggerPortAddress);
            Assert.AreEqual(0x15000013u, (uint) BcdElementType.LibrarySerialDebuggerPort);
            Assert.AreEqual(0x15000014u, (uint) BcdElementType.LibrarySerialDebuggerBaudRate);
            Assert.AreEqual(0x15000015u, (uint) BcdElementType.Library1394DebuggerChannel);
            Assert.AreEqual(0x12000016u, (uint) BcdElementType.LibraryUsbDebuggerTargetName);
            Assert.AreEqual(0x16000017u, (uint) BcdElementType.LibraryDebuggerIgnoreUsermodeExceptions);
            Assert.AreEqual(0x15000018u, (uint) BcdElementType.LibraryDebuggerStartPolicy);
            Assert.AreEqual(0x12000019u, (uint) BcdElementType.LibraryDebuggerBusParameters);
            Assert.AreEqual(0x1500001Au, (uint) BcdElementType.LibraryDebuggerNetHostIP);
            Assert.AreEqual(0x1500001Bu, (uint) BcdElementType.LibraryDebuggerNetPort);
            Assert.AreEqual(0x1600001Cu, (uint) BcdElementType.LibraryDebuggerNetDhcp);
            Assert.AreEqual(0x1200001Du, (uint) BcdElementType.LibraryDebuggerNetKey);
            Assert.AreEqual(0x1600001Eu, (uint) BcdElementType.LibraryDebuggerNetVm);
            Assert.AreEqual(0x1200001Fu, (uint) BcdElementType.LibraryDebuggerNetHostIPv6);
            Assert.AreEqual(0x16000020u, (uint) BcdElementType.LibraryEmsEnabled);
            Assert.AreEqual(0x15000022u, (uint) BcdElementType.LibraryEmsPortNumber);
            Assert.AreEqual(0x15000023u, (uint) BcdElementType.LibraryEmsBaudRate);
            Assert.AreEqual(0x12000030u, (uint) BcdElementType.LibraryLoadOptionsString);
            Assert.AreEqual(0x16000031u, (uint) BcdElementType.LibraryAttemptNonBcdStart);
            Assert.AreEqual(0x16000040u, (uint) BcdElementType.LibraryDisplayAdvancedOptions);
            Assert.AreEqual(0x16000041u, (uint) BcdElementType.LibraryDisplayOptionsEdit);
            Assert.AreEqual(0x15000042u, (uint) BcdElementType.LibraryFveKeyRingAddress);
            Assert.AreEqual(0x11000043u, (uint) BcdElementType.LibraryBsdLogDevice);
            Assert.AreEqual(0x12000044u, (uint) BcdElementType.LibraryBsdLogPath);
            Assert.AreEqual(0x16000045u, (uint) BcdElementType.LibraryBsdPreserveLog);
            Assert.AreEqual(0x16000046u, (uint) BcdElementType.LibraryGraphicsModeDisabled);
            Assert.AreEqual(0x15000047u, (uint) BcdElementType.LibraryConfigAccessPolicy);
            Assert.AreEqual(0x16000048u, (uint) BcdElementType.LibraryDisableIntegrityChecks);
            Assert.AreEqual(0x16000049u, (uint) BcdElementType.LibraryAllowPrereleaseSignatures);
            Assert.AreEqual(0x1200004Au, (uint) BcdElementType.LibraryFontPath);
            Assert.AreEqual(0x1500004Bu, (uint) BcdElementType.LibraryIntegrityServicesPolicy);
            Assert.AreEqual(0x1500004Cu, (uint) BcdElementType.LibraryFveBandID);
            Assert.AreEqual(0x16000050u, (uint) BcdElementType.LibraryConsoleExtendedInput);
            Assert.AreEqual(0x15000051u, (uint) BcdElementType.LibraryInitialConsoleInput);
            Assert.AreEqual(0x15000052u, (uint) BcdElementType.LibraryGraphicsResolution);
            Assert.AreEqual(0x16000053u, (uint) BcdElementType.LibraryRestartOnFailure);
            Assert.AreEqual(0x16000054u, (uint) BcdElementType.LibraryGraphicsForceHighestMode);
            Assert.AreEqual(0x16000060u, (uint) BcdElementType.LibraryIsolatedExecutionContext);
            Assert.AreEqual(0x15000065u, (uint) BcdElementType.LibraryBootUxDisplayMessage);
            Assert.AreEqual(0x15000066u, (uint) BcdElementType.LibraryBootUxDisplayMessageOverride);
            Assert.AreEqual(0x16000067u, (uint) BcdElementType.LibraryBootUxLogoDisable);
            Assert.AreEqual(0x16000068u, (uint) BcdElementType.LibraryBootUxTextDisable);
            Assert.AreEqual(0x16000069u, (uint) BcdElementType.LibraryBootUxProgressDisable);
            Assert.AreEqual(0x1600006Au, (uint) BcdElementType.LibraryBootUxFadeDisable);
            Assert.AreEqual(0x1600006Bu, (uint) BcdElementType.LibraryBootUxReservePoolDebug);
            Assert.AreEqual(0x1600006Cu, (uint) BcdElementType.LibraryBootUxDisable);
            Assert.AreEqual(0x1500006Du, (uint) BcdElementType.LibraryBootUxFadeFrames);
            Assert.AreEqual(0x1600006Eu, (uint) BcdElementType.LibraryBootUxDumpStats);
            Assert.AreEqual(0x1600006Fu, (uint) BcdElementType.LibraryBootUxShowStats);
            Assert.AreEqual(0x16000071u, (uint) BcdElementType.LibraryMultiBootSystem);
            Assert.AreEqual(0x16000072u, (uint) BcdElementType.LibraryForceNoKeyboard);
            Assert.AreEqual(0x15000073u, (uint) BcdElementType.LibraryAliasWindowsKey);
            Assert.AreEqual(0x16000074u, (uint) BcdElementType.LibraryBootShutdownDisabled);
            Assert.AreEqual(0x15000075u, (uint) BcdElementType.LibraryPerformanceFrequency);
            Assert.AreEqual(0x15000076u, (uint) BcdElementType.LibrarySecureBootRawPolicy);
            Assert.AreEqual(0x17000077u, (uint) BcdElementType.LibraryAllowedInMemorySettings);
            Assert.AreEqual(0x16000078u, (uint) BcdElementType.X16000078);
            Assert.AreEqual(0x15000079u, (uint) BcdElementType.LibraryBootUxBitmapTransitionTime);
            Assert.AreEqual(0x1600007Au, (uint) BcdElementType.LibraryTwoBootImages);
            Assert.AreEqual(0x1600007Bu, (uint) BcdElementType.LibraryForceFipsCrypto);
            Assert.AreEqual(0x1500007Du, (uint) BcdElementType.LibraryBootErrorUx);
            Assert.AreEqual(0x1600007Eu, (uint) BcdElementType.LibraryAllowFlightSignatures);
            Assert.AreEqual(0x1500007Fu, (uint) BcdElementType.LibraryBootMeasurementLogFormat);
            Assert.AreEqual(0x15000080u, (uint) BcdElementType.LibraryDisplayRotation);
            Assert.AreEqual(0x15000081u, (uint) BcdElementType.LibraryLogControl);
            Assert.AreEqual(0x16000082u, (uint) BcdElementType.LibraryNoFirmwareSync);
            Assert.AreEqual(0x11000083u, (uint) BcdElementType.X11000083);
            Assert.AreEqual(0x11000084u, (uint) BcdElementType.LibraryWindowsSystemDevice);
            Assert.AreEqual(0x16000085u, (uint) BcdElementType.X16000085);
            Assert.AreEqual(0x15000086u, (uint) BcdElementType.X15000086);
            Assert.AreEqual(0x16000087u, (uint) BcdElementType.LibraryNumLockOn);
            Assert.AreEqual(0x12000088u, (uint) BcdElementType.LibraryAdditionalCiPolicy);
            Assert.AreEqual(0x15000088u, (uint) BcdElementType.LibraryLinearAddress57Policy);

            // Application elements
            Assert.AreEqual(0x26000202u, (uint) BcdElementType.FfuLoaderTypeOneShotSkipFfuUpdate);
            Assert.AreEqual(0x26000203u, (uint) BcdElementType.FfuLoaderTypeForceFfu);
            Assert.AreEqual(0x25000510u, (uint) BcdElementType.ChargingTypeBootThreshold);
            Assert.AreEqual(0x26000512u, (uint) BcdElementType.ChargingTypeOffModeCharging);
            Assert.AreEqual(0x25000AAAu, (uint) BcdElementType.GlobalTypeBootFlow);

            // Template elements
            Assert.AreEqual(0x45000001u, (uint) BcdElementType.DeviceType);
            Assert.AreEqual(0x42000002u, (uint) BcdElementType.ApplicationRelativePath);
            Assert.AreEqual(0x42000003u, (uint) BcdElementType.RamDiskDeviceRelativePath);
            Assert.AreEqual(0x46000004u, (uint) BcdElementType.OmitOsLoaderElements);
            Assert.AreEqual(0x47000006u, (uint) BcdElementType.ElementsToMigrateList);
            Assert.AreEqual(0x46000010u, (uint) BcdElementType.RecoveryOs);

            // FWBOOTMGR and BOOTMGR elements
            Assert.AreEqual(0x24000001u, (uint) BcdElementType.DisplayOrder);
            Assert.AreEqual(0x24000002u, (uint) BcdElementType.BootSequence);
            Assert.AreEqual(0x23000003u, (uint) BcdElementType.DefaultObject);
            Assert.AreEqual(0x25000004u, (uint) BcdElementType.Timeout);
            Assert.AreEqual(0x26000005u, (uint) BcdElementType.AttemptResume);
            Assert.AreEqual(0x23000006u, (uint) BcdElementType.ResumeObject);
            Assert.AreEqual(0x24000007u, (uint) BcdElementType.StartupSequence);
            Assert.AreEqual(0x24000010u, (uint) BcdElementType.ToolDisplayOrder);
            Assert.AreEqual(0x26000020u, (uint) BcdElementType.DisplayBootMenu);
            Assert.AreEqual(0x26000021u, (uint) BcdElementType.NoErrorDisplay);
            Assert.AreEqual(0x21000022u, (uint) BcdElementType.BcdDevice);
            Assert.AreEqual(0x22000023u, (uint) BcdElementType.BcdFilePath);
            Assert.AreEqual(0x26000024u, (uint) BcdElementType.HormEnabled);
            Assert.AreEqual(0x26000025u, (uint) BcdElementType.HiberBoot);
            Assert.AreEqual(0x22000026u, (uint) BcdElementType.PasswordOverride);
            Assert.AreEqual(0x22000027u, (uint) BcdElementType.PinpassPhraseOverride);
            Assert.AreEqual(0x26000028u, (uint) BcdElementType.ProcessCustomActionsFirst);
            Assert.AreEqual(0x2600002Au, (uint) BcdElementType.X2600002A);
            Assert.AreEqual(0x27000030u, (uint) BcdElementType.CustomActionsList);
            Assert.AreEqual(0x26000031u, (uint) BcdElementType.PersistBootSequence);
            Assert.AreEqual(0x26000032u, (uint) BcdElementType.SkipStartupSequence);
            Assert.AreEqual(0x22000040u, (uint) BcdElementType.FveRecoveryUrl);
            Assert.AreEqual(0x22000041u, (uint) BcdElementType.FveRecoveryMessage);
            Assert.AreEqual(0x26000042u, (uint) BcdElementType.BootFlightBootMgr);
            Assert.AreEqual(0x25000043u, (uint) BcdElementType.FveUnlockRetryCountIPv4);
            Assert.AreEqual(0x25000044u, (uint) BcdElementType.FveUnlockRetryCountIPv6);
            Assert.AreEqual(0x22000045u, (uint) BcdElementType.FveServerAddressIPv4);
            Assert.AreEqual(0x22000046u, (uint) BcdElementType.FveServerAddressIPv6);
            Assert.AreEqual(0x22000047u, (uint) BcdElementType.FveStationAddressIPv4);
            Assert.AreEqual(0x22000048u, (uint) BcdElementType.FveStationAddressIPv6);
            Assert.AreEqual(0x22000049u, (uint) BcdElementType.FveStationAddressSubnetMaskIPv4);
            Assert.AreEqual(0x2200004Au, (uint) BcdElementType.FveStationAddressPrefixIPv6);
            Assert.AreEqual(0x2200004Bu, (uint) BcdElementType.FveGatewayIPv4);
            Assert.AreEqual(0x2200004Cu, (uint) BcdElementType.FveGatewayIPv6);
            Assert.AreEqual(0x2500004Du, (uint) BcdElementType.FveNetworkTimeout);
            Assert.AreEqual(0x2500004Eu, (uint) BcdElementType.FveRemotePortIPv4);
            Assert.AreEqual(0x2500004Fu, (uint) BcdElementType.FveRemotePortIPv6);
            Assert.AreEqual(0x25000050u, (uint) BcdElementType.FveStationPortIPv4);
            Assert.AreEqual(0x25000051u, (uint) BcdElementType.FveStationPortIPv6);

            // OSLOADER elements
            Assert.AreEqual(0x21000001u, (uint) BcdElementType.OperatingSystemDevice);
            Assert.AreEqual(0x22000002u, (uint) BcdElementType.SystemRoot);
            Assert.AreEqual(0x23000003u, (uint) BcdElementType.AssociatedResumeObject);
            Assert.AreEqual(0x26000004u, (uint) BcdElementType.StampDisks);
            Assert.AreEqual(0x21000005u, (uint) BcdElementType.X21000005);
            Assert.AreEqual(0x25000008u, (uint) BcdElementType.X25000008);
            Assert.AreEqual(0x26000010u, (uint) BcdElementType.DetectKernelAndHal);
            Assert.AreEqual(0x22000011u, (uint) BcdElementType.KernelPath);
            Assert.AreEqual(0x22000012u, (uint) BcdElementType.HalPath);
            Assert.AreEqual(0x22000013u, (uint) BcdElementType.DebugTransportPath);
            Assert.AreEqual(0x25000020u, (uint) BcdElementType.NxPolicy);
            Assert.AreEqual(0x25000021u, (uint) BcdElementType.PaePolicy);
            Assert.AreEqual(0x26000022u, (uint) BcdElementType.WinPeMode);
            Assert.AreEqual(0x26000024u, (uint) BcdElementType.DisableCrashAutoReboot);
            Assert.AreEqual(0x26000025u, (uint) BcdElementType.UseLastGoodSettings);
            Assert.AreEqual(0x26000026u, (uint) BcdElementType.DisableCodeIntegrityChecks);
            Assert.AreEqual(0x26000027u, (uint) BcdElementType.AllowPrereleaseSignatures);
            Assert.AreEqual(0x26000030u, (uint) BcdElementType.NoLowMemory);
            Assert.AreEqual(0x25000031u, (uint) BcdElementType.RemoveMemory);
            Assert.AreEqual(0x25000032u, (uint) BcdElementType.IncreaseUserVa);
            Assert.AreEqual(0x25000033u, (uint) BcdElementType.PerformanceDataMemory);
            Assert.AreEqual(0x26000040u, (uint) BcdElementType.UseVgaDriver);
            Assert.AreEqual(0x26000041u, (uint) BcdElementType.DisableBootDisplay);
            Assert.AreEqual(0x26000042u, (uint) BcdElementType.DisableVesaBios);
            Assert.AreEqual(0x26000043u, (uint) BcdElementType.DisableVgaMode);
            Assert.AreEqual(0x25000050u, (uint) BcdElementType.ClusterModeAddressing);
            Assert.AreEqual(0x26000051u, (uint) BcdElementType.UsePhysicalDestination);
            Assert.AreEqual(0x25000052u, (uint) BcdElementType.RestrictApicCluster);
            Assert.AreEqual(0x22000053u, (uint) BcdElementType.EvStore);
            Assert.AreEqual(0x26000054u, (uint) BcdElementType.UseLegacyApicMode);
            Assert.AreEqual(0x25000055u, (uint) BcdElementType.X2ApicPolicy);
            Assert.AreEqual(0x26000060u, (uint) BcdElementType.UseBootProcessorOnly);
            Assert.AreEqual(0x25000061u, (uint) BcdElementType.NumberOfProcessors);
            Assert.AreEqual(0x26000062u, (uint) BcdElementType.ForceMaximumProcessors);
            Assert.AreEqual(0x25000063u, (uint) BcdElementType.ProcessorConfigurationFlags);
            Assert.AreEqual(0x26000064u, (uint) BcdElementType.MaximiseGroupsCreated);
            Assert.AreEqual(0x26000065u, (uint) BcdElementType.ForceGroupAwareness);
            Assert.AreEqual(0x25000066u, (uint) BcdElementType.GroupSize);
            Assert.AreEqual(0x26000070u, (uint) BcdElementType.UseFirmwarePciSettings);
            Assert.AreEqual(0x25000071u, (uint) BcdElementType.MsiPolicy);
            Assert.AreEqual(0x25000072u, (uint) BcdElementType.PciExpressPolicy);
            Assert.AreEqual(0x25000080u, (uint) BcdElementType.SafeBoot);
            Assert.AreEqual(0x26000081u, (uint) BcdElementType.SafeBootAlternateShell);
            Assert.AreEqual(0x26000090u, (uint) BcdElementType.BootLogInitialisation);
            Assert.AreEqual(0x26000091u, (uint) BcdElementType.VerboseObjectLoadMode);
            Assert.AreEqual(0x260000A0u, (uint) BcdElementType.KernelDebuggerEnabled);
            Assert.AreEqual(0x260000A1u, (uint) BcdElementType.DebuggerHalBreakpoint);
            Assert.AreEqual(0x260000A2u, (uint) BcdElementType.UsePlatformClock);
            Assert.AreEqual(0x260000A3u, (uint) BcdElementType.ForceLegacyPlatform);
            Assert.AreEqual(0x260000A4u, (uint) BcdElementType.UsePlatformTick);
            Assert.AreEqual(0x260000A5u, (uint) BcdElementType.DisableDynamicTick);
            Assert.AreEqual(0x250000A6u, (uint) BcdElementType.TscSyncPolicy);
            Assert.AreEqual(0x260000B0u, (uint) BcdElementType.EmsEnabled);
            Assert.AreEqual(0x250000C0u, (uint) BcdElementType.ForceFailure);
            Assert.AreEqual(0x250000C1u, (uint) BcdElementType.DriverLoadFailurePolicy);
            Assert.AreEqual(0x250000C2u, (uint) BcdElementType.BootMenuPolicy);
            Assert.AreEqual(0x260000C3u, (uint) BcdElementType.OneTimeAdvancedOptions);
            Assert.AreEqual(0x260000C4u, (uint) BcdElementType.EditOptionsOneTime);
            Assert.AreEqual(0x250000E0u, (uint) BcdElementType.BootStatusPolicy);
            Assert.AreEqual(0x260000E1u, (uint) BcdElementType.DisableElamDrivers);
            Assert.AreEqual(0x250000F0u, (uint) BcdElementType.HypervisorLaunchType);
            Assert.AreEqual(0x220000F1u, (uint) BcdElementType.HypervisorPath);
            Assert.AreEqual(0x260000F2u, (uint) BcdElementType.HypervisorDebuggerEnabled);
            Assert.AreEqual(0x250000F3u, (uint) BcdElementType.HypervisorDebuggerType);
            Assert.AreEqual(0x250000F4u, (uint) BcdElementType.HypervisorDebuggerPortNumber);
            Assert.AreEqual(0x250000F5u, (uint) BcdElementType.HypervisorDebuggerBaudRate);
            Assert.AreEqual(0x250000F6u, (uint) BcdElementType.HypervisorDebugger1394Channel);
            Assert.AreEqual(0x250000F7u, (uint) BcdElementType.BootUxPolicy);
            Assert.AreEqual(0x260000F8u, (uint) BcdElementType.HypervisorSlatDisabled);
            Assert.AreEqual(0x220000F9u, (uint) BcdElementType.HypervisorDebuggerBusParams);
            Assert.AreEqual(0x250000FAu, (uint) BcdElementType.HypervisorNumProc);
            Assert.AreEqual(0x250000FBu, (uint) BcdElementType.HypervisorRootProcPerNode);
            Assert.AreEqual(0x260000FCu, (uint) BcdElementType.HypervisorUseLargeVtlb);
            Assert.AreEqual(0x250000FDu, (uint) BcdElementType.HypervisorDebuggerNetHostIp);
            Assert.AreEqual(0x250000FEu, (uint) BcdElementType.HypervisorDebuggerNetHostPort);
            Assert.AreEqual(0x250000FFu, (uint) BcdElementType.HypervisorDebuggerPages);
            Assert.AreEqual(0x25000100u, (uint) BcdElementType.TpmBootEntropy);
            Assert.AreEqual(0x22000110u, (uint) BcdElementType.HypervisorDebuggerNetKey);
            Assert.AreEqual(0x22000112u, (uint) BcdElementType.HypervisorProductSkuType);
            Assert.AreEqual(0x25000113u, (uint) BcdElementType.HypervisorRootProc);
            Assert.AreEqual(0x26000114u, (uint) BcdElementType.HypervisorDebuggerNetDhcp);
            Assert.AreEqual(0x25000115u, (uint) BcdElementType.HypervisorIommuPolicy);
            Assert.AreEqual(0x26000116u, (uint) BcdElementType.HypervisorUseVapic);
            Assert.AreEqual(0x22000117u, (uint) BcdElementType.HypervisorLoadOptions);
            Assert.AreEqual(0x25000118u, (uint) BcdElementType.HypervisorMsrFilterPolicy);
            Assert.AreEqual(0x25000119u, (uint) BcdElementType.HypervisorMmioNxPolicy);
            Assert.AreEqual(0x2500011Au, (uint) BcdElementType.HypervisorSchedulerType);
            Assert.AreEqual(0x2200011Bu, (uint) BcdElementType.HypervisorRootProcNumaNodes);
            Assert.AreEqual(0x2500011Cu, (uint) BcdElementType.HypervisorPerfMon);
            Assert.AreEqual(0x2500011Du, (uint) BcdElementType.HypervisorRootProcPerCore);
            Assert.AreEqual(0x2200011Eu, (uint) BcdElementType.HypervisorRootProcNumaNodeLps);
            Assert.AreEqual(0x25000120u, (uint) BcdElementType.XSavePolicy);
            Assert.AreEqual(0x25000121u, (uint) BcdElementType.XSaveAddFeature0);
            Assert.AreEqual(0x25000122u, (uint) BcdElementType.XSaveAddFeature1);
            Assert.AreEqual(0x25000123u, (uint) BcdElementType.XSaveAddFeature2);
            Assert.AreEqual(0x25000124u, (uint) BcdElementType.XSaveAddFeature3);
            Assert.AreEqual(0x25000125u, (uint) BcdElementType.XSaveAddFeature4);
            Assert.AreEqual(0x25000126u, (uint) BcdElementType.XSaveAddFeature5);
            Assert.AreEqual(0x25000127u, (uint) BcdElementType.XSaveAddFeature6);
            Assert.AreEqual(0x25000128u, (uint) BcdElementType.XSaveAddFeature7);
            Assert.AreEqual(0x25000129u, (uint) BcdElementType.XSaveRemoveFeature);
            Assert.AreEqual(0x2500012Au, (uint) BcdElementType.XSaveProcessorsMask);
            Assert.AreEqual(0x2500012Bu, (uint) BcdElementType.XSaveDisable);
            Assert.AreEqual(0x2500012Cu, (uint) BcdElementType.KernelDebuggerType);
            Assert.AreEqual(0x2200012Du, (uint) BcdElementType.KernelDebuggerBusParameters);
            Assert.AreEqual(0x2500012Eu, (uint) BcdElementType.KernelDebuggerPortAddress);
            Assert.AreEqual(0x2500012Fu, (uint) BcdElementType.KernelDebuggerPortNumber);
            Assert.AreEqual(0x25000130u, (uint) BcdElementType.KernelDebuggerClaimedDeviceLockCounter);
            Assert.AreEqual(0x25000131u, (uint) BcdElementType.KernelDebugger1394Channel);
            Assert.AreEqual(0x22000132u, (uint) BcdElementType.KernelDebuggerUsbTargetName);
            Assert.AreEqual(0x25000133u, (uint) BcdElementType.KernelDebuggerNetHostIp);
            Assert.AreEqual(0x25000134u, (uint) BcdElementType.KernelDebuggerNetHostPort);
            Assert.AreEqual(0x26000135u, (uint) BcdElementType.KernelDebuggerNetDhcp);
            Assert.AreEqual(0x22000136u, (uint) BcdElementType.KernelDebuggerNetKey);
            Assert.AreEqual(0x22000137u, (uint) BcdElementType.ImcHiveName);
            Assert.AreEqual(0x21000138u, (uint) BcdElementType.ImcDevice);
            Assert.AreEqual(0x25000139u, (uint) BcdElementType.KernelDebuggerBaudRate);
            Assert.AreEqual(0x22000140u, (uint) BcdElementType.ManufacturingMode);
            Assert.AreEqual(0x26000141u, (uint) BcdElementType.EventLoggingEnabled);
            Assert.AreEqual(0x25000142u, (uint) BcdElementType.VsmLaunchType);
            Assert.AreEqual(0x25000144u, (uint) BcdElementType.HypervisorEnforcedCodeIntegrity);
            Assert.AreEqual(0x26000145u, (uint) BcdElementType.DTraceEnabled);
            Assert.AreEqual(0x21000150u, (uint) BcdElementType.SystemDataDevice);
            Assert.AreEqual(0x21000151u, (uint) BcdElementType.OperatingSystemArcDevice);
            Assert.AreEqual(0x21000152u, (uint) BcdElementType.X21000152);
            Assert.AreEqual(0x21000153u, (uint) BcdElementType.OperatingSystemDataDevice);
            Assert.AreEqual(0x21000154u, (uint) BcdElementType.BspDevice);
            Assert.AreEqual(0x21000155u, (uint) BcdElementType.BspFilePath);
            Assert.AreEqual(0x22000156u, (uint) BcdElementType.KernelDebuggerNetHostIPv6);
            Assert.AreEqual(0x22000161u, (uint) BcdElementType.HypervisorDebuggerNetHostIpv6);

            // RESUME elements
            Assert.AreEqual(0x21000001u, (uint) BcdElementType.HiberFileDevice);
            Assert.AreEqual(0x22000002u, (uint) BcdElementType.HiberFilePath);
            Assert.AreEqual(0x26000003u, (uint) BcdElementType.UseCustomSettings);
            Assert.AreEqual(0x26000004u, (uint) BcdElementType.X86PaeMode);
            Assert.AreEqual(0x21000005u, (uint) BcdElementType.AssociatedOsDevice);
            Assert.AreEqual(0x26000006u, (uint) BcdElementType.DebugOptionEnabled);
            Assert.AreEqual(0x25000007u, (uint) BcdElementType.ResumeBootUxPolicy);
            Assert.AreEqual(0x25000008u, (uint) BcdElementType.ResumeBootMenuPolicy);
            Assert.AreEqual(0x26000024u, (uint) BcdElementType.ResumeHormEnabled);

            // MEMDIAG elements
            Assert.AreEqual(0x25000001u, (uint) BcdElementType.PassCount);
            Assert.AreEqual(0x25000002u, (uint) BcdElementType.TestMix);
            Assert.AreEqual(0x25000003u, (uint) BcdElementType.FailureCount);
            Assert.AreEqual(0x26000003u, (uint) BcdElementType.CacheEnable);
            Assert.AreEqual(0x25000004u, (uint) BcdElementType.TestToFail);
            Assert.AreEqual(0x26000004u, (uint) BcdElementType.FailuresEnabled);
            Assert.AreEqual(0x26000005u, (uint) BcdElementType.CacheEnableLegacy);
            Assert.AreEqual(0x25000005u, (uint) BcdElementType.StrideFailureCount);
            Assert.AreEqual(0x25000006u, (uint) BcdElementType.InvcFailureCount);
            Assert.AreEqual(0x25000007u, (uint) BcdElementType.MatsFailureCount);
            Assert.AreEqual(0x25000008u, (uint) BcdElementType.RandFailureCount);
            Assert.AreEqual(0x25000009u, (uint) BcdElementType.ChckrFailureCount);

            // NTLDR and SETUPLDR elements
            Assert.AreEqual(0x22000001u, (uint) BcdElementType.BpbString);

            // STARTUP elements
            Assert.AreEqual(0x26000001u, (uint) BcdElementType.PxeSoftReboot);
            Assert.AreEqual(0x22000002u, (uint) BcdElementType.PxeApplicationName);

            // BOOTAPP elements
            Assert.AreEqual(0x26000145u, (uint) BcdElementType.EnableBootDebugPolicy);
            Assert.AreEqual(0x26000146u, (uint) BcdElementType.EnableBootOrderClean);
            Assert.AreEqual(0x26000147u, (uint) BcdElementType.EnableDeviceID);
            Assert.AreEqual(0x26000148u, (uint) BcdElementType.EnableFfuLoader);
            Assert.AreEqual(0x26000149u, (uint) BcdElementType.EnableIuLoader);
            Assert.AreEqual(0x2600014Au, (uint) BcdElementType.EnableMassStorage);
            Assert.AreEqual(0x2600014Bu, (uint) BcdElementType.EnableRpmbProvisioning);
            Assert.AreEqual(0x2600014Cu, (uint) BcdElementType.EnableSecureBootPolicy);
            Assert.AreEqual(0x2600014Du, (uint) BcdElementType.EnableStartCharge);
            Assert.AreEqual(0x2600014Eu, (uint) BcdElementType.EnableResetTpm);

            // Device elements
            Assert.AreEqual(0x35000001u, (uint) BcdElementType.RamdiskImageOffset);
            Assert.AreEqual(0x35000002u, (uint) BcdElementType.RamdiskTftpClientPort);
            Assert.AreEqual(0x31000003u, (uint) BcdElementType.RamdiskSdiDevice);
            Assert.AreEqual(0x32000004u, (uint) BcdElementType.RamdiskSdiPath);
            Assert.AreEqual(0x35000005u, (uint) BcdElementType.RamdiskImageLength);
            Assert.AreEqual(0x36000006u, (uint) BcdElementType.RamdiskExportAsCd);
            Assert.AreEqual(0x35000007u, (uint) BcdElementType.RamdiskTftpBlockSize);
            Assert.AreEqual(0x35000008u, (uint) BcdElementType.RamdiskTftpWindowSize);
            Assert.AreEqual(0x36000009u, (uint) BcdElementType.RamdiskMulticastEnabled);
            Assert.AreEqual(0x3600000Au, (uint) BcdElementType.RamdiskMulticastTftpFallback);
            Assert.AreEqual(0x3600000Bu, (uint) BcdElementType.RamdiskTftpVarWindow);
            Assert.AreEqual(0x3600000Cu, (uint) BcdElementType.VhdRamdiskBoot);
            Assert.AreEqual(0x3500000Du, (uint) BcdElementType.X3500000D);

        }

        [TestMethod]
        public void TestWellKnownBcdObjects() {
            Assert.AreEqual(Guid.Parse("{0CE4991B-E6B3-4B16-B23C-5E0D9250E5D9}"), WellKnownBcdObject.EmsSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{1AFA9C49-16AB-4A5C-4A90-212802DA9460}"), WellKnownBcdObject.ResumeLoaderSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{1CAE1EB7-A0DF-4D4D-9851-4860E34EF535}"), WellKnownBcdObject.DefaultBootEntry.GetGuid());
            Assert.AreEqual(Guid.Parse("{313E8EED-7098-4586-A9BF-309C61F8D449}"), WellKnownBcdObject.KernelDebuggerSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{4636856E-540F-4170-A130-A84776F4C654}"), WellKnownBcdObject.DebuggerSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{466F5A88-0AF2-4F76-9038-095B170DC21C}"), WellKnownBcdObject.WindowsLegacyNtldr.GetGuid());
            Assert.AreEqual(Guid.Parse("{5189B25C-5558-4BF2-BCA4-289B11BD29E2}"), WellKnownBcdObject.BadMemoryGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{6EFB52BF-1766-41DB-A6B3-0EE5EFF72BD7}"), WellKnownBcdObject.BootLoaderSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{7254A080-1510-4E85-AC0F-E7FB3D444736}"), WellKnownBcdObject.WindowsSetupEfi.GetGuid());
            Assert.AreEqual(Guid.Parse("{7EA2E1AC-2E61-4728-AAA3-896D9D0A9F0E}"), WellKnownBcdObject.GlobalSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{7FF607E0-4395-11DB-B0DE-0800200C9A66}"), WellKnownBcdObject.HypervisorSettingsGroup.GetGuid());
            Assert.AreEqual(Guid.Parse("{9DEA862C-5CDD-4E70-ACC1-F32B344D4795}"), WellKnownBcdObject.WindowsBootmgr.GetGuid());
            Assert.AreEqual(Guid.Parse("{A1943BBC-EA85-487C-97C7-C9EDE908A38A}"), WellKnownBcdObject.WindowsOsTargetTemplatePcat.GetGuid());
            Assert.AreEqual(Guid.Parse("{A5A30FA2-3D06-4E9F-B5F4-A01DF9D1FCBA}"), WellKnownBcdObject.FirmwareBootmgr.GetGuid());
            Assert.AreEqual(Guid.Parse("{AE5534E0-A924-466C-B836-758539A3EE3A}"), WellKnownBcdObject.WindowsSetupRamdiskOptions.GetGuid());
            Assert.AreEqual(Guid.Parse("{B012B84D-C47C-4ED5-B722-C0C42163E569}"), WellKnownBcdObject.WindowsOsTargetTemplateEfi.GetGuid());
            Assert.AreEqual(Guid.Parse("{B2721D73-1DB4-4C62-BF78-C548A880142D}"), WellKnownBcdObject.WindowsMemoryTester.GetGuid());
            Assert.AreEqual(Guid.Parse("{CBD971BF-B7B8-4885-951A-FA03044F5D71}"), WellKnownBcdObject.WindowsSetupPcat.GetGuid());
            Assert.AreEqual(Guid.Parse("{FA926493-6F1C-4193-A414-58F0B2456D1E}"), WellKnownBcdObject.CurrentBootEntry.GetGuid());

            Assert.IsTrue(WellKnownBcdObject.EmsSettingsGroup.GetNames().Contains("{emssettings}"));
            Assert.IsTrue(WellKnownBcdObject.ResumeLoaderSettingsGroup.GetNames().Contains("{resumeloadersettings}"));
            Assert.IsTrue(WellKnownBcdObject.DefaultBootEntry.GetNames().Contains("{default}"));
            Assert.IsTrue(WellKnownBcdObject.KernelDebuggerSettingsGroup.GetNames().Contains("{kerneldbgsettings}"));
            Assert.IsTrue(WellKnownBcdObject.DebuggerSettingsGroup.GetNames().Contains("{dbgsettings}"));
            Assert.IsTrue(WellKnownBcdObject.DebuggerSettingsGroup.GetNames().Contains("{eventsettings}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsLegacyNtldr.GetNames().Contains("{legacy}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsLegacyNtldr.GetNames().Contains("{ntldr}"));
            Assert.IsTrue(WellKnownBcdObject.BadMemoryGroup.GetNames().Contains("{badmemory}"));
            Assert.IsTrue(WellKnownBcdObject.BootLoaderSettingsGroup.GetNames().Contains("{bootloadersettings}"));
            Assert.IsTrue(WellKnownBcdObject.GlobalSettingsGroup.GetNames().Contains("{globalsettings}"));
            Assert.IsTrue(WellKnownBcdObject.HypervisorSettingsGroup.GetNames().Contains("{hypervisorsettings}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsBootmgr.GetNames().Contains("{bootmgr}"));
            Assert.IsTrue(WellKnownBcdObject.FirmwareBootmgr.GetNames().Contains("{fwbootmgr}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsSetupRamdiskOptions.GetNames().Contains("{ramdiskoptions}"));
            Assert.IsTrue(WellKnownBcdObject.WindowsMemoryTester.GetNames().Contains("{memdiag}"));
            Assert.IsTrue(WellKnownBcdObject.CurrentBootEntry.GetNames().Contains("{current}"));
        }

        [TestMethod]
        public void TestCreateRegistryBcdStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                path = $"{Path.GetPathRoot(path)}{Path.GetFileName(Path.GetTempFileName())}";

                using (var seBackup = new TokenPrivilege("SeBackupPrivilege"))
                using (var seRestore = new TokenPrivilege("SeRestorePrivilege")) {
                    try {
                        using var bcd = RegistryBcdStore.Create(path);
                        Assert.IsNotNull(bcd);
                    } finally {
                        File.Delete(path);
                    }
                }
            }
        }

        [TestMethod]
        public void TestOpenRegistrySystemStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                using var store = new RegistryBcdStore();
                Assert.IsNotNull(store);
                Assert.IsTrue(store.Any());

                {
                    var obj = store.FirstOrDefault(o => o.Type == BcdObjectType.OperatingSystemLoader);
                    Assert.IsNotNull(obj);
                    var img = obj.FirstOrDefault(e => e.Type == BcdElementType.LibraryApplicationPath);
                    Assert.IsNotNull(img);
                }
            }
        }

        [TestMethod]
        public void TestCreateBcdStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var wmi = new ManagementService(GetLogger<ManagementService>());
                var boot = new BootService(wmi, GetLogger<BootService>());

                var path = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                path = $"{Path.GetPathRoot(path)}{Path.GetFileName(Path.GetTempFileName())}";

                var store = boot.CreateBcdStore(path);
                Assert.IsNotNull(store);
                Assert.IsTrue(File.Exists(path));

                try {
                    var opened = boot.OpenBcdStore(path);
                    Assert.IsNotNull(opened);
                } finally {
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void TestOpenSystemStore() {
            if (WindowsIdentity.GetCurrent().IsAdministrator()) {
                var wmi = new ManagementService(GetLogger<ManagementService>());
                var boot = new BootService(wmi, GetLogger<BootService>());

                var store = boot.OpenBcdStore(null);
                Assert.IsNotNull(store);

                var p = store.GetMethodParameters("EnumerateObjects");
                p["Type"] = 0x10200003;
                var r = store.InvokeMethod("EnumerateObjects", p, null);
                var retval = r["ReturnValue"];
                var objs = r["Objects"];
            }
        }

        private static ILogger<T> GetLogger<T>() => Loggers.CreateLogger<T>();
        private static readonly ILoggerFactory Loggers = LoggerFactory.Create(static l => l.AddDebug());
    }
}
