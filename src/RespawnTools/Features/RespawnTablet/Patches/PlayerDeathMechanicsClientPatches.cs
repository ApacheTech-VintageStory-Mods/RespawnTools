using Gantry.Services.HarmonyPatches.Annotations;
using JetBrains.Annotations;
using Vintagestory.API.Common;

// ReSharper disable InconsistentNaming

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.Patches;

[HarmonySidedPatch(EnumAppSide.Universal)]
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public class PlayerDeathMechanicsClientPatches;