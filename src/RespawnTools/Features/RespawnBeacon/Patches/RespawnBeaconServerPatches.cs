using Gantry.Services.HarmonyPatches.Annotations;
using JetBrains.Annotations;

// ReSharper disable UnusedType.Global// 
// ReSharper disable InconsistentNaming

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon.Patches;

/// <summary>
///     Harmony Patches for the <see cref="RespawnBeacon"/> Feature. This class cannot be inherited.
/// </summary>
[HarmonyServerSidePatch]
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public sealed partial class RespawnBeaconServerPatches;