// <copyright file="IImageServicing.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a service for modifying WIM images, for instance
    /// for injecting drivers and enabling features.
    /// </summary>
    public interface IImageServicing : IDisposable {

        /// <summary>
        /// Commits all changes to the image.
        /// </summary>
        void Commit();

        /// <summary>
        /// Injects all drivers in the specified folder into the image.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="recursive"></param>
        /// <param name="forceUnsigned"></param>
        void InjectDrivers(string folder,
            bool recursive = false,
            bool forceUnsigned = false);

        /// <summary>
        /// Opens a WIM file at the specified location.
        /// </summary>
        /// <param name="path"></param>
        void Open(string path);

        /// <summary>
        /// Rolls back all changes.
        /// </summary>
        void RollBack();
    }
}
