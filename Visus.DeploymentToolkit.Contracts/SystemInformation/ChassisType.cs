// <copyright file="ChassisType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

namespace Visus.DeploymentToolkit.SystemInformation
{

    /// <summary>
    /// Potential chassis types used by WMI.
    /// </summary>
    /// <remarks>
    /// Cf. https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-systemenclosure
    /// </remarks>
    public enum ChassisType : ushort
    {

        Undefined = 0,

        Other = 1,
        Unknown = 2,
        Desktop = 3,
        LowProfileDesktop = 4,
        PizzaBox = 5,
        MiniTower = 6,
        Tower = 7,
        Portable = 8,
        Laptop = 9,
        Notebook = 10,
        HandHeld = 11,
        DockingStation = 12,
        AllinOne = 13,
        SubNotebook = 14,
        SpaceSaving = 15,
        LunchBox = 16,
        MainSystemChassis = 17,
        ExpansionChassis = 18,
        SubChassis = 19,
        BusExpansionChassis = 20,
        PeripheralChassis = 21,
        StorageChassis = 22,
        RackMountChassis = 23,
        SealedCasePC = 24,
        Tablet = 30,
        Convertible = 31,
        Detachable = 32
    }
}
