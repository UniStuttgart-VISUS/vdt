// <copyright file="DismMount.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Manages the lifetime of a mounted WIM image.
    /// </summary>
    internal sealed class DismMount : IDismMount {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="dism">The DISM scope representing the library handle.
        /// </param>
        /// <param name="imagePath">The path of the WIM image to be mounted.
        /// </param>
        /// <param name="imageIndex">The index of the image to be mounted.
        /// </param>
        /// <param name="mountPoint">The path where the image should be mounted.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public DismMount(IDismScope dism,
                string imagePath,
                int imageIndex,
                string mountPoint) {
            this._dism = dism ?? throw new ArgumentNullException(nameof(dism));
            this.ImagePath = imagePath
                ?? throw new ArgumentNullException(nameof(imagePath));
            this.MountPoint = mountPoint
                ?? throw new ArgumentNullException(nameof(mountPoint));
            DismApi.MountImage(imagePath, mountPoint, imageIndex);
        }
        #endregion

        #region Finaliser
        /// <summary>
        /// Finalises the instance.
        /// </summary>
        ~DismMount() {
            this.Dispose(false);
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public string ImagePath { get; }

        /// <inheritdoc />
        public string MountPoint { get; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Commit() {
            if (this._dism == null) {
                throw new InvalidOperationException();
            }

            // Unmount the image and commit all changes.
            DismApi.UnmountImage(this.MountPoint, true);

            // Note: We only clear our reference, but we do not dispose the
            // DISM handle as other mounts might be still open.
            this._dism = null;
        }

        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public override string? ToString()
            => this.MountPoint ?? base.ToString();
        #endregion

        #region Private methods
        private void Dispose(bool disposing) {
            if (this._dism != null) {
                // If we are still mounted when being disposed, we discard all
                // changes, which is the safe option here.
                DismApi.UnmountImage(this.MountPoint, false);

                // Note: We only clear our reference, but we do not dispose the
                // DISM handle as other mounts might be still open.
                this._dism = null;
            }
        }
        #endregion

        #region Private fields
        private IDismScope? _dism;
        #endregion
    }
}
