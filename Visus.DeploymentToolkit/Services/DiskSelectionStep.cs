// <copyright file="DiskSelectionStep.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

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
        /// <param name="logger"></param>
        /// <returns></returns>
        public IEnumerable<IDisk> Apply(IEnumerable<IDisk>? disks,
                ILogger logger) {
            _ = logger ?? throw new ArgumentNullException(nameof(logger));

            var retval = Enumerable.Empty<IDisk>();
            if (disks == null) {
                return retval;
            }

            switch (this.BuiltInCondition) {
                case BuiltInCondition.IsLargest:
                    logger.LogInformation(Resources.DiskSelectionLargest);
                    retval = (from d in disks
                              orderby d.Size descending
                              select d).Take(1);
                    break;

                case BuiltInCondition.IsSmallest:
                    logger.LogInformation(Resources.DiskSelectionSmallest);
                    retval = (from d in disks
                              orderby d.Size ascending
                              select d).Take(1);
                    break;

                case BuiltInCondition.IsEfiBootDisk:
                    logger.LogInformation(Resources.DiskSelectionEfiBootDisk);
                    retval = SelectEfiBootDisks(disks, EfiPartitionType.Any);
                    break;

                case BuiltInCondition.IsMbrBootDisk:
                    logger.LogInformation(Resources.DiskSelectionMbrBootDisk);
                    retval = from d in disks
                             where d.Partitions.Any(p => p.IsBoot)
                             select d;
                    break;

                case BuiltInCondition.None:
                default:
                    logger.LogInformation(Resources.DiskSelectionCondition,
                        this.Condition, this.Action);
                    retval = disks.AsQueryable().Where(this.Condition);
                    break;
            }

            switch (this.Action) {
                case DiskSelectionAction.Include:
                    logger.LogInformation(Resources.DiskSelectionInclude,
                        retval.Count());
                    if (!retval.Any()) {
                        logger.LogWarning(Resources.DiskSelectionEmpty);
                        retval = disks;
                    }
                    break;

                case DiskSelectionAction.Exclude:
                    logger.LogInformation(Resources.DiskSelectionExclude,
                        retval.Count());
                    retval = disks.AsQueryable().Except(retval);
                    if (!retval.Any()) {
                        logger.LogWarning(Resources.DiskSelectionEmpty);
                        retval = disks;
                    }
                    break;

                case DiskSelectionAction.None:
                    logger.LogInformation(Resources.DiskSelectionNone);
                    retval = disks;
                    break;
            }

            return retval;
        }
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

        #region Private methods
        private static IEnumerable<IDisk> SelectEfiBootDisks(
                IEnumerable<IDisk> disks, EfiPartitionType type) {
            Debug.Assert(disks != null);
            // First, filter for GPT disks with at least one partition.
            var retval = from d in disks
                         where d.PartitionStyle == PartitionStyle.Gpt
                         where d.Partitions.Any()
                         select d;

            // TODO: Need to find out the file system
            // TODO: Need to assign a letter if necessary.

            return retval;
        }
        #endregion
    }
}
