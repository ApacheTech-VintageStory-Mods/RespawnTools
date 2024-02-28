using ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.GameContent.Items;
using Gantry.Services.HarmonyPatches.Annotations;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Common;

// ReSharper disable InconsistentNaming

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.Patches;

[HarmonySidedPatch(EnumAppSide.Server)]
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public class PlayerDeathMechanicsServerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityPlayer), nameof(EntityPlayer.Die))]
    public static bool UniversalPatch_EntityPlayer_Die_Prefix(EntityPlayer __instance)
    {
        if (!Helpers.TryGetItemInUsableSlot<ItemRespawnTablet>(__instance.Player, out var slot, out var tablet))
        {
            return true;
        }
        tablet!.OnPlayerDeath(__instance, slot!);
        return false;
    }
}