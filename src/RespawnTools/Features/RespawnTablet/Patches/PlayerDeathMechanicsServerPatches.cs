using RespawnTools.Features.RespawnTablet.GameContent.Items;

namespace RespawnTools.Features.RespawnTablet.Patches;

[HarmonyServerPatch]
public class PlayerDeathMechanicsServerPatches
{
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(EntityPlayer), nameof(EntityPlayer.Die))]
    public static bool UniversalPatch_EntityPlayer_Die_Prefix(EntityPlayer __instance)
    {
        if (!__instance.Player.TryGetItemInUsableSlot<ItemRespawnTablet>(out var slot, out var tablet))
            return true;
        tablet!.OnPlayerDeath(__instance, slot!);
        return false;
    }
}