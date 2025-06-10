// <copyright file="WmiDiskService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to disk management via the WMI
    /// </summary>
    /// <param name="wmi">The basic WMI service for obtaining the management
    /// objects.</param>
    /// <param name="logger">A logger for the service.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="wmi"/> is
    /// <see langword="null"/>, or if <paramref name="logger"/> is
    /// <see langword="null"/>.</exception>
    [SupportedOSPlatform("windows")]
    internal sealed class WmiDiskService(
            IManagementService wmi,
            [FromKeyedServices("VDS")] IDiskManagement vds,
            ILogger<WmiDiskService> logger)
            : IDiskManagement {

        #region Public methods
        /// <inheritdoc />
        public async Task CleanAsync(IDisk disk,
                CleanFlags flags,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disk);
            if (disk is not WmiDisk wmi) {
                throw new InvalidOperationException(Errors.NoWmiDisk);
            }

#if false
            var d = (ManagementObject) wmi;

            var force = ((flags & CleanFlags.Force) != 0);
            var forceOem = ((flags & CleanFlags.ForceOem) != 0);
            var fullClean = ((flags & CleanFlags.FullClean) != 0);

            return Task.Run(() => {
                // TODO: This call succeeds, but has no effect.
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Clearing disk {Disk} ({Name}) "
                    + "with flags {Flags}: RemoveData = {RemoveData}, "
                    + "RemoveOEM = {RemoveOEM}, ZeroOutEntireDisk = "
                    + "{ZeroOutEntireDisk}.", disk.ID, disk.FriendlyName,
                    flags, force, forceOem, fullClean);
                this.CheckResult(d.Invoke("Clear",
                    ("RemoveData", force),
                    ("RemoveOEM", forceOem),
                    ("ZeroOutEntireDisk", fullClean)));
            }, cancellationToken);
#else
            this._logger.LogTrace("Forwarding the clean operation for disk "
                + "{Disk} ({Name}) to the VDS because the WMI implementation "
                + "succeeds, but does not do anything. If anyone finds out why "
                + "this is the case, please fix this to use WMI only.", disk.ID,
                disk.FriendlyName);
            var vds = await this._vds.GetDiskAsync(disk, cancellationToken)
                .ConfigureAwait(false);
            this._logger.LogTrace("Disk {Disk} ({Name}) was found by VDS using "
                + "path {Path}.", vds.ID, vds.FriendlyName, vds.Path);
            await this._vds.CleanAsync(vds, flags, cancellationToken);
#endif
        }

        /// <inheritdoc />
        public Task ConvertAsync(IDisk disk,
                PartitionStyle style,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disk);
            if (disk is not WmiDisk wmi) {
                throw new InvalidOperationException(Errors.NoWmiDisk);
            }

            var d = (ManagementObject) wmi;

            if (disk.PartitionStyle == style) {
                this._logger.LogTrace("Disk {Disk} already has partition style "
                    + "{Style}.", disk.ID, style);
                return Task.CompletedTask;
            }

            cancellationToken.ThrowIfCancellationRequested();
            dynamic args = new ExpandoObject();
            args.PartitionStyle = (ushort) style;

            if (disk.PartitionStyle == PartitionStyle.Unknown) {
                return Task.Run(() => {
                    cancellationToken.ThrowIfCancellationRequested();
                    this._logger.LogTrace("Disk {Disk} has no partition style, "
                        + "initialising it to {Style}.", disk.ID, style);
                    var result = d.Invoke("Initialize", (ExpandoObject) args);
                    this.CheckResult(result);
                }, cancellationToken);

            } else {
                return Task.Run(() => {
                    cancellationToken.ThrowIfCancellationRequested();
                    this._logger.LogTrace("Converting disk {Disk} from "
                        + "{OldStyle} to {NewStyle}.", disk.ID,
                        disk.PartitionStyle, style);
                    var result = d.Invoke("ConvertStyle", (ExpandoObject) args);
                    this.CheckResult(result);
                }, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<IPartition> CreatePartitionAsync(IDisk disk,
                IPartition partition,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disk);
            ArgumentNullException.ThrowIfNull(partition);
            if (disk is not WmiDisk wmi) {
                throw new InvalidOperationException(Errors.NoWmiDisk);
            }

            var d = (ManagementObject) wmi;

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogInformation("Creating a partition of {Size} bytes "
                + "on disk {Disk} ({Name}) at {Offset}.", partition.Size,
                disk.ID, disk.FriendlyName, partition.Offset);
            dynamic args = new ExpandoObject();

            if (partition.Size > 0) {
                this._logger.LogTrace("Using user-provided partition size of "
                    + "{Size} bytes.", partition.Size);
                args.Size = partition.Size;
            } else {
                this._logger.LogTrace("Using all remaining space for the "
                    + "partition.");
                args.UseMaximumSize = true;
            }

            if (partition.Offset > 0) {
                this._logger.LogTrace("Using user-provided partition offset of "
                    + "{Offset} bytes.", partition.Offset);
                args.Offset = partition.Offset;
            } else {
                var offset = (from p in wmi.GetPartitions()
                              orderby p.Offset descending
                              select p.Offset + p.Size).FirstOrDefault();
                this._logger.LogTrace("Appending at an offset of {Offset} "
                    + "bytes.", offset);
                // https://github.com/pbatard/rufus/blob/688f011f317492225ded10b51edc19b39d62fbd4/src/drive.c#L2290-L2306
                if (offset == 0) {
                    offset = 1 * 1024 * 1024;
                    this._logger.LogTrace("Adjusting offset of first partition "
                        + "to {Offset}.", offset);
                }
                args.Offset = offset;
            }

            args.Alignment = 0;
            args.AssignDriveLetter = false;
            //args.AssignDriveLetter = true;
            //args.DriveLetter = 's';

            // For some reason unbeknownst to me, we cannot create any partition
            // using WMI when specifying the type at creation time. Therefore,
            // we first create a raw partition and change it later.
            //switch (wmi.PartitionStyle) {
            //    case PartitionStyle.Mbr:
            //        Debug.Assert(partition.Type.Mbr is not null);
            //        this._logger.LogTrace("The partition type is {Type}.",
            //            partition.Type.Mbr);
            //        args.MbrType = partition.Type.Mbr.Value;
            //        break;

            //    case PartitionStyle.Gpt:
            //        Debug.Assert(partition.Type.Gpt is not null);
            //        this._logger.LogTrace("The partition type is {Type}.",
            //            partition.Type.Gpt);
            //        args.GptType = partition.Type.Gpt.Value;
            //        // https://stackoverflow.com/questions/12975436/create-uefi-partition-using-vds
            //        //if (partition.Type.Equals(PartitionType.EfiSystem)) {
            //        //    args.GptType = PartitionType.MicrosoftBasicData.Gpt;
            //        //} else {
            //        //    args.GptType = partition.Type.Gpt;
            //        //}
            //        break;
            //}

            var hidden = partition.Flags.HasFlag(PartitionFlags.Hidden);
            this._logger.LogTrace("The hidden flag is {Hidden}.", hidden);
            args.IsHidden = hidden;

            var active = partition.Flags.HasFlag(PartitionFlags.Active);
            this._logger.LogTrace("The active flag is {Active}.", active);
            args.IsActive = active;

            var result = d.Invoke("CreatePartition", (ExpandoObject) args);
            this.CheckResult(result);

            var mo = result.GetProperty<ManagementBaseObject>(
                "CreatedPartition");
            var retval = new WmiPartition(mo, wmi.PartitionStyle);

            // Use VDS to change the partition type.
            var vds = await this._vds.GetDiskAsync(disk, cancellationToken)
                .ConfigureAwait(false);
            this._logger.LogTrace("Disk {Disk} ({Name}) was found by VDS using "
                + "path {Path}.", vds.ID, vds.FriendlyName, vds.Path);

            var adv = ((VdsDisk) vds).AdvancedDisk2;
            var change = new CHANGE_PARTITION_TYPE_PARAMETERS {
                Style = (VDS_PARTITION_STYLE) wmi.PartitionStyle
            };

            switch (change.Style) {
                case VDS_PARTITION_STYLE.MBR:
                    Debug.Assert(partition.Type.Mbr is not null);
                    this._logger.LogTrace("Changing the partition type to "
                        + "{Type}.", partition.Type.Mbr);
                    change.MbrType = partition.Type.Mbr.Value;
                    break;

                case VDS_PARTITION_STYLE.GPT:
                    Debug.Assert(partition.Type.Gpt is not null);
                    this._logger.LogTrace("Changing the partition type to "
                        + "{Type}.", partition.Type.Gpt);
                    change.GptType = partition.Type.Gpt.Value;
                    break;
            }

            adv.ChangePartitionType(retval.Offset, true, ref change);

            return retval;
        }

        /// <inheritdoc />
        public Task FormatAsync(IPartition partition,
                FileSystem fileSystem,
                string label,
                uint allocationUnitSize,
                FormatFlags flags,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(partition);
            ArgumentNullException.ThrowIfNull(partition);
            if (partition is not WmiPartition wmi) {
                throw new InvalidOperationException(Errors.NoWmiDisk);
            }

            var p = (ManagementObject) wmi;
            var fs = fileSystem.ToWmi();



            throw new NotImplementedException("TODO");
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDisk>> GetDisksAsync(
                CancellationToken cancellationToken)
            => Task.Run(() => {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Retrieving disks via WMI.");
                var retval = this._wmi
                    .GetInstancesOf(WmiDisk.Class, this._wmi.WindowsStorageScope)
                    .Select(d => new WmiDisk(d));

                var sb = new StringBuilder("Disks found: ");
                foreach (var d in retval) {
                    sb.AppendLine()
                        .Append(d.ID)
                        .Append(" (")
                        .Append(d.FriendlyName)
                        .AppendLine("):");

                    foreach (var p in ((ManagementObject) d).Properties) {
                        sb.Append(p.Name);
                        sb.Append(" = ");
                        sb.Append(p.Value);
                        sb.AppendLine();
                    }
                }
                this._logger.LogTrace(sb.ToString());

                return retval.AsEnumerable<IDisk>();
            }, cancellationToken);
#endregion

        #region Private methods
        /// <summary>
        /// Checks the result of a Storage Managment API operation.
        /// </summary>
        /// <param name="result">The output parameters of the API call.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        private void CheckResult(ManagementBaseObject result) {
            if (result is null) {
                this._logger.LogWarning("An invalid WMI result has been "
                    + "specified. This is a bug and should never happen.");
                throw new ArgumentNullException(nameof(result));
            }

            var sb = new StringBuilder();
            foreach (var p in result.Properties) {
                sb.Append(p.Name).Append(" = ").Append(p.Value).Append("; ");
            }
            this._logger.LogTrace("Checking WMI result: {Result}.",
                sb.ToString());

            var status = (int) result.GetProperty<uint>("ReturnValue");
            if (status == 0) {
                this._logger.LogTrace("The return value indicates success.");
                return;
            }
            
            if (result.TryGetProperty<string>("ExtendedStatus", out var msg)) {
                this._logger.LogError("The WMI operation failed with status "
                    + "{Status} and message: {Message}.", status, msg);
                throw new Win32Exception(status, msg);
            } else {
                this._logger.LogError("The WMI operation failed with status "
                    + "{Status} but without additional information.", status);
                throw new Win32Exception(status);
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        private readonly IManagementService _wmi = wmi
            ?? throw new ArgumentNullException(nameof(wmi));
        private readonly VdsService _vds = vds as VdsService
            ?? throw new ArgumentNullException(nameof(vds));
        #endregion
    }
}
