﻿// <copyright file="IVdsVolumeMF.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VDS_REPARSE_POINT_PROP {
        public Guid SourceVolumeID;
        public string Path;
    }
}
