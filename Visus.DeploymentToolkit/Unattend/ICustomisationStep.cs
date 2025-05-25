// <copyright file="ICustomisationStep.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Xml.Linq;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// The interface of a customisation step that can be applied to an
    /// unattend.xml file.
    /// </summary>
    public interface ICustomisationStep {

        #region Public methods
        /// <summary>
        /// Applies the customisation step to the given in-memory
        /// representation of an unattend.xml file.
        /// </summary>
        /// <param name="unattend">The in-memory representation of an unattend
        /// file.</param>
        void Apply(XDocument unattend);
        #endregion
    }
}
