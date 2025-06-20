﻿// <copyright file="DiskSelectionStep.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// A single step in the sequence of selecting an installation disk.
    /// </summary>
    public sealed class DiskSelectionStep {

        #region Public properties
        /// <summary>
        /// Gets or sets a complex built-in condition to be checked on the
        /// <see cref="IDisk"/>.
        /// </summary>
        public BuiltInCondition BuiltInCondition {
            get;
            set;
        } = BuiltInCondition.None;

        /// <summary>
        /// Gets or sets the condition applied on the disks.
        /// </summary>
        public string Condition {
            get;
            set;
        } = "false";

        /// <summary>
        /// Gets or sets the action to be performed for the matching disks.
        /// </summary>
        public DiskSelectionAction Action {
            get;
            set;
        } = DiskSelectionAction.None;
        #endregion

        #region Public methods
        /// <summary>
        /// Applies the selection step on <paramref name="disks"/>, returning
        /// only the disks selected by this step.
        /// </summary>
        /// <remarks>
        /// The method executes <see cref="Condition"/> on the list of
        /// <paramref name="disks"/> and performs the specified
        /// <see cref="Action"/> on the results. If the result set would be
        /// empty, it returns <paramref name="disks"/> instead. The result set
        /// is therefor only empty if <paramref name="disks"/> is <c>null</c> or
        /// empty itself.
        /// </remarks>
        /// <param name="disks"></param>
        /// <param name="diskManagement"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<IEnumerable<IDisk>> ApplyAsync(IEnumerable<IDisk>? disks,
                IDiskManagement diskManagement,
                ILogger logger,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disks);
            ArgumentNullException.ThrowIfNull(diskManagement);
            ArgumentNullException.ThrowIfNull(logger);

            var retval = Enumerable.Empty<IDisk>();
            if (disks == null) {
                return Task.FromResult(retval);
            }

            switch (BuiltInCondition) {
                case BuiltInCondition.HasLinuxPartition:
                    logger.LogInformation("Selecting disks with a Linux "
                        + "partition on it for action {Action}.", this.Action);
                    retval = from d in disks
                             where d.Partitions.Any(p => p.IsType(PartitionType.AllLinux))
                             select d;
                    break;

                case BuiltInCondition.HasMicrosoftPartition:
                    logger.LogInformation("Selecting disks with a Microsoft "
                        + "partition on it for action {Action}.", this.Action);
                    retval = from d in disks
                             where d.Partitions.Any(p => p.IsType(PartitionType.AllMicrosoft))
                             select d;
                    break;

                case BuiltInCondition.IsEfiSystemDisk:
                    logger.LogInformation("Selecting a disk with an EFI system "
                        + "partition on it for action {Action}.", this.Action);
                    retval = from d in disks
                             where d.Partitions.Any(p => p.IsType(PartitionType.EfiSystem))
                             select d;
                    break;

                case BuiltInCondition.IsEmpty:
                    logger.LogInformation("Selecting disks without "
                        + "any partition for action {Action}.", this.Action);
                    retval = from d in disks
                             where !d.Partitions.Any()
                             select d;
                    break;

                case BuiltInCondition.IsLargest:
                    logger.LogInformation("Selecting the largest disk for "
                        + "action {Action}.", this.Action);
                    retval = (from d in disks
                              orderby d.Size descending
                              select d).Take(1);
                    break;

                case BuiltInCondition.IsMbrBootDisk:
                    logger.LogInformation("Selecting a disk with a MBR on it "
                        + " for action {Action}.", this.Action);
                    retval = from d in disks
                             where d.Partitions.Any(p => p.Flags.HasFlag(PartitionFlags.Boot))
                             select d;
                    break;

                case BuiltInCondition.IsReadOnly:
                    logger.LogInformation("Selecting read-only disks for "
                        + "action {Action}.", this.Action);
                    retval = from d in disks
                             where d.Flags.HasFlag(DiskFlags.ReadOnly)
                             select d;
                    break;

                case BuiltInCondition.IsSmallest:
                    logger.LogInformation("Selecting the smallest disk for "
                        + "action {Action}.", this.Action);
                    retval = (from d in disks
                              orderby d.Size ascending
                              select d).Take(1);
                    break;

                case BuiltInCondition.IsUninitialised:
                    logger.LogInformation("Selecting uninitialised disks for "
                        + "action {Action}.", this.Action);
                    retval = from d in disks
                             where d.Flags.HasFlag(DiskFlags.Uninitialised)
                             select d;
                    break;

                case BuiltInCondition.None:
                default:
                    logger.LogInformation("Selecting a disk that fulfils the "
                        + "condition {Condition} for action {Action}.",
                        this.Condition, this.Action);
                    retval = disks.AsQueryable().Where(this.Condition);
                    break;
            }

            var cntSelected = retval.Count();
            var selection = string.Join(", ",
                retval.Select(d => d.FriendlyName));

            switch (Action) {
                case DiskSelectionAction.Avoid:
                    logger.LogInformation("The selection should avoid {Avoid} "
                        + "disk(s): {Selection}", cntSelected, selection);
                    retval = disks.AsQueryable().Except(retval);
                    if (!retval.Any()) {
                        logger.LogInformation("The disk selection step would "
                            + "avoid all available disks, so the selection "
                            + "remains unchanged.");
                        retval = disks;
                    }
                    break;

                case DiskSelectionAction.Include:
                    logger.LogInformation("The selection includes {Included} "
                        + "disk(s): {Selection}", cntSelected, selection);
                    if (!retval.Any()) {
                        logger.LogWarning("The disk selection step resulted in "
                            + "an empty set of disks to include.");
                    }
                    break;

                case DiskSelectionAction.Exclude:
                    logger.LogInformation("The selection excludes {Excluded} "
                        + "disk(s): {Selection}", cntSelected, selection);
                    retval = disks.AsQueryable().Except(retval);
                    if (!retval.Any()) {
                        logger.LogWarning("The disk selection step excluded "
                            + "all available disks. Use the Avoid action to "
                            + "avoid this situation.");
                    }
                    break;

                case DiskSelectionAction.Prefer:
                    logger.LogInformation("The selection prefers {Preferred} "
                        + "disk(s): {Selection}", cntSelected, selection);
                    if (!retval.Any()) {
                        logger.LogInformation("The disk selection step resulted "
                            + "in an empty set of preferred disks, so the "
                            + "selection remains unchanged.");
                        retval = disks;
                    }
                    break;

                case DiskSelectionAction.None:
                    logger.LogInformation("The disk selection remains "
                        + "unchanged.");
                    retval = disks;
                    break;
            }

            return Task.FromResult(retval);
        }

        /// <summary>
        /// Applies the selection step on <paramref name="disks"/>, returning
        /// only the disks selected by this step.
        /// </summary>
        /// <param name="disks"></param>
        /// <param name="diskManagement"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public Task<IEnumerable<IDisk>> ApplyAsync(IEnumerable<IDisk>? disks,
                IDiskManagement diskManagement,
                ILogger logger)
            => this.ApplyAsync(disks, diskManagement, logger,
                CancellationToken.None);
        #endregion

        #region Nested enum
        /// <summary>
        /// Allows for selecting specific types of EFI boot partitions.
        /// </summary>
        private enum EfiPartitionType {
            /// <summary>
            /// Get any EFI boot partition, which means that the partition must
            /// be FAT32 and contain a top-level EFI folder.
            /// </summary>
            Any,

            /// <summary>
            /// Restrict the selection to EFI folders that contain a Microsoft
            /// boot loader.
            /// </summary>
            Microsoft,

            /// <summary>
            /// Restrict the selection to EFI folders that contan a
            /// non-Microsoft boot loader.
            /// </summary>
            NonMicrosoft
        }
        #endregion
    }
}
