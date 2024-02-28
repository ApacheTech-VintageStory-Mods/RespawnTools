using System;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.RespawnTools.Core;

/// <summary>
///     Shows that a method runs on a specific app-side. This class cannot be inherited.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class RunsOnAttribute : Attribute
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="RunsOnAttribute"/> class.
    /// </summary>
    /// <param name="side">The side that the method runs on.</param>
    public RunsOnAttribute(EnumAppSide side) => Side = side;

    /// <summary>
    ///     The side that the method runs on.
    /// </summary>
    public EnumAppSide Side { get; }
}