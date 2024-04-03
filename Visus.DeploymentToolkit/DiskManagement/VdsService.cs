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
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.DiskManagement {

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
            var disks = await GetDisksAsync(cancellationToken);
            return disks.Where(d => d.ID == id).SingleOrDefault();
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDisk>> GetDisksAsync(
                CancellationToken cancellationToken) {
            return Task<IEnumerable<IDisk>>.Factory.StartNew(
                () => GetDisks(cancellationToken));
        }
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

            IEnumVdsObject enumerator;
            pack.QueryDisks(out enumerator);

            while (true) {
                cancellation.ThrowIfCancellationRequested();

                enumerator.Next(1, out var unknown, out var cnt);
                if (cnt == 0) {
                    break;
                }

                if (unknown is IVdsDisk disk && disk != null) {
                    yield return new VdsDisk(disk);
                }
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
            {
                _logger.LogInformation(Resources.WaitingVds);
                var status = _service.WaitForServiceReady();
                if (status != 0) {
                    throw new COMException(Errors.WaitVdsFailed, (int) status);
                }
            }

            cancellation.ThrowIfCancellationRequested();

            // Enumerate all disk providers.
            IEnumVdsObject enumerator;
            _service.QueryProviders(
                VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS
                //| VDS_QUERY_PROVIDER_FLAG.HARDWARE_PROVIDERS
                | VDS_QUERY_PROVIDER_FLAG.VIRTUALDISK_PROVIDERS,
                out enumerator);
            enumerator.Reset();

            while (true) {
                cancellation.ThrowIfCancellationRequested();

                enumerator.Next(1, out var unknown, out uint cnt);
                if (cnt == 0) {
                    break;
                }

                if (unknown is IVdsSwProvider sw && sw != null) {
                    // This is the default software provider in Windows.
                    foreach (var d in GetDisks(sw, cancellation)) {
                        yield return d;
                    }

                } else if (unknown is IVdsVdProvider vd && vd != null) {
                    // Get virtual disks attached to the system.
                    foreach (var d in GetDisks(vd, cancellation)) {
                        yield return d;
                    }
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

            IEnumVdsObject enumerator;
            provider.QueryPacks(out enumerator);

            while (true) {
                cancellation.ThrowIfCancellationRequested();

                enumerator.Next(1, out var unknown, out var cnt);
                if (cnt == 0) {
                    break;
                }

                if (unknown is IVdsPack pack && pack != null) {
                    foreach (var d in GetDisks(pack, cancellation)) {
                        yield return d;
                    }
                }
            }
        }

        private IEnumerable<IDisk> GetDisks(
                IVdsVdProvider provider,
                CancellationToken cancellation) {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            cancellation.ThrowIfCancellationRequested();
            yield break;
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly IVdsService _service;
        #endregion
    }
}
