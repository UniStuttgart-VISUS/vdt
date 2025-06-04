// <copyright file="VdsService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to disk management via the Virtual Disk Service VDS.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class VdsService : IDiskManagement {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="Exception">If the <see cref="VdsServiceLoader"/>
        /// does nto implement <see cref="IVdsServiceLoader"/>, which should
        /// never happen.</exception>
        public VdsService(ILogger<VdsService> logger) {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            try {
                var loader = new VdsServiceLoader() as IVdsServiceLoader
                    ?? throw new Exception(Errors.NoVdsServiceLoader);
                loader.LoadService(null, out _service);
            } catch (Exception ex) {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        #region Public methods
        /// <inheritdoc />
        public async Task<IDisk?> GetDiskAsync(Guid id,
                CancellationToken cancellationToken) {
            var disks = await this.GetDisksAsync(cancellationToken)
                .ConfigureAwait(false);
            return disks.Where(d => d.ID == id).SingleOrDefault();
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDisk>> GetDisksAsync(
                CancellationToken cancellationToken) {
            return Task<IEnumerable<IDisk>>.Factory.StartNew(
                () => GetDisks(cancellationToken));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IDisk>> GetDisksAsync(
                PartitionType partitionType,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(partitionType);
            var disks = await this.GetDisksAsync(cancellationToken)
                .ConfigureAwait(false);
            return disks.Where(d => d.Partitions.Any(
                p => partitionType.Equals(p.Type)));
        }

        /// <summary>
        /// Gets all packs registered with the software providers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IEnumerable<IVdsPack>> GetPacksAsync(
                CancellationToken cancellationToken) {
            return Task.Run(() => {
                cancellationToken.ThrowIfCancellationRequested();
                this.WaitForVds();

                cancellationToken.ThrowIfCancellationRequested();
                var retval = new List<IVdsPack>();
                var types = VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS;

                this._logger.LogTrace("Querying all sofware providers.");
                foreach (var unknown in this._service.QueryProviders(types)) {
                    cancellationToken.ThrowIfCancellationRequested();

                    if ((unknown is IVdsSwProvider sw) && (sw != null)) {
                        this._logger.LogTrace("Querying packs from provider "
                            + "{Provider}", sw);
                        retval.AddRange(sw.QueryPacks());
                    }
                }

                return retval.AsEnumerable();
            });
        }

        /// <summary>
        /// Refresh disk ownership and layout information.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RefreshAsync(bool reenumerate,
                CancellationToken cancellationToken) 
            => Task.Run(() => {
                cancellationToken.ThrowIfCancellationRequested();
                this.WaitForVds();

                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogTrace("Refreshing the VDS service.");
                this._service.Refresh();

                if (reenumerate) {
                    cancellationToken.ThrowIfCancellationRequested();
                    this._logger.LogTrace("Re-enumerating the disks.");
                    this._service.Reenumerate();
                }
            }, cancellationToken);
        #endregion

        #region Private class methods
        /// <summary>
        /// Enumerate all disks in the given <paramref name="pack"/>.
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="logger"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private IEnumerable<IDisk> GetDisks(
                IVdsPack pack,
                CancellationToken cancellation) {
            _ = pack ?? throw new ArgumentNullException(nameof(pack));
            cancellation.ThrowIfCancellationRequested();

            foreach (var d in pack.QueryDisks()) {
                cancellation.ThrowIfCancellationRequested();
                yield return new VdsDisk(d);
            }
        }

        /// <summary>
        /// Gets the disks for all providers registered with the given
        /// <paramref name="service"/>.
        /// </summary>
        /// <remarks>
        /// Currently, we only support the Windows software provider.
        /// </remarks>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="COMException"></exception>
        private IEnumerable<IDisk> GetDisks(
                CancellationToken cancellation) {
            cancellation.ThrowIfCancellationRequested();

            // First of all, make sure that the VDS is ready.
            this.WaitForVds();

            cancellation.ThrowIfCancellationRequested();

            // Enumerate all disk providers.
            var types = VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS
                //| VDS_QUERY_PROVIDER_FLAG.HARDWARE_PROVIDERS
                | VDS_QUERY_PROVIDER_FLAG.VIRTUALDISK_PROVIDERS;

            foreach (var unknown in this._service.QueryProviders(types)) {
                cancellation.ThrowIfCancellationRequested();

                if (unknown is IVdsSwProvider sw && sw != null) {
                    this._logger.LogTrace("Querying disks from the Microsoft "
                        + "software provider.");
                    foreach (var d in GetDisks(sw, cancellation)) {
                        yield return d;
                    }

                } else if (unknown is IVdsVdProvider vd && vd != null) {
                    this._logger.LogTrace("Querying virtual disks attached to "
                        + "the system.");
                    foreach (var d in GetDisks(vd, cancellation)) {
                        yield return d;
                    }
                }
            }

            // Unallocated disks need the be enumerated separately. These
            // are the most important ones for our scenario.
            {
                this._logger.LogTrace("Querying unallocated disks.");
                this._service.QueryUnallocatedDisks(out var enumerator);
                foreach (var d in enumerator.Enumerate<IVdsDisk>()) {
                    cancellation.ThrowIfCancellationRequested();
                    yield return new VdsDisk(d, DiskFlags.Uninitialised);
                }
            }
        }

        /// <summary>
        /// Gets all disks from the specified software provider.
        /// </summary>
        /// <param name="provider">The software provider for disks.</param>
        /// <param name="cancellation"></param>
        /// <returns>All disks managed by the provider.</returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="provider"/> is <c>null</c>, or if
        /// <paramref name="logger"/> is <c>null</c>.</exception>
        private IEnumerable<IDisk> GetDisks(
                IVdsSwProvider provider,
                CancellationToken cancellation) {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            cancellation.ThrowIfCancellationRequested();

            foreach (var pack in provider.QueryPacks()) {
                cancellation.ThrowIfCancellationRequested();

                foreach (var d in GetDisks(pack, cancellation)) {
                    cancellation.ThrowIfCancellationRequested();
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Enumerates all disks from the specified virtual disk provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private IEnumerable<IDisk> GetDisks(
                IVdsVdProvider provider,
                CancellationToken cancellation) {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            cancellation.ThrowIfCancellationRequested();
            foreach (var d in  provider.QueryVDisks()) {
                cancellation.ThrowIfCancellationRequested();
                yield return new VdsDisk(d);
            }
        }

        /// <summary>
        /// Blocks the calling thread until the Virtual Disk Service is ready.
        /// </summary>
        /// <exception cref="COMException"></exception>
        private void WaitForVds() {
            this._logger.LogTrace("Waiting for the Virtual Disk "
                + "Service to become ready.");
            var status = this._service.WaitForServiceReady();
            if (status != 0) {
                this._logger.LogWarning("The Virtual Disk Service did not "
                    + "become ready with status: {Status}", status);
                throw new COMException(Errors.WaitVdsFailed, (int) status);
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly IVdsService _service;
        #endregion
    }
}
