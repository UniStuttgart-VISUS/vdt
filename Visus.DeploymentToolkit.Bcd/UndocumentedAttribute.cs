// <copyright file="UndocumentedAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Marks a property or field as undocumented API according to
    /// https://www.geoffchappell.com/notes/windows/boot/bcd/index.htm?tx=37.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false, Inherited = true)]
    public sealed class UndocumentedAttribute : Attribute {}
}
