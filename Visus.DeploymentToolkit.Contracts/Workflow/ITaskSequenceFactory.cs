// <copyright file="ITaskSequenceFactory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the interface of a factory class that can be used to create and
    /// manage <see cref="ITaskSequence"/>s.
    /// </summary>
    public interface ITaskSequenceFactory {

        /// <summary>
        /// Create a builder for a new <see cref="ITaskSequence"/>.
        /// </summary>
        /// <returns></returns>
        ITaskSequenceBuilder CreateBuilder();

        /// <summary>
        /// Load a <see cref="ITaskSequenceDescription"/> from the given JSON
        /// file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<ITaskSequenceDescription?> LoadDescriptionAsync(string path);
    }
}
