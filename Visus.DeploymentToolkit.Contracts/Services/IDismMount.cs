// <copyright file="IDismMount.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Manages the lifetime of a mounted WIM image.
    /// </summary>
    public interface IDismMount : IDisposable {

        #region Public properties
        /// <summary>
        /// Gets the path to the image to be mounted.
        /// </summary>
        string ImagePath { get; }

        /// <summary>
        /// Gets the path where the image is mounted.
        /// </summary>
        string MountPoint { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Commits all changes and unmounts the image.
        /// </summary>
        void Commit();
        #endregion
    }
}
