using RespawnTools.Features.RespawnBeacon.GameContent.BlockEntities;
using Vintagestory.Server;

namespace RespawnTools.Features.RespawnBeacon.Patches;

public sealed class ServerMainPatches : GantryPatch
{
    /// <summary>
    ///     Applies a <see cref="HarmonyPrefix"/> patch to the "GetSpawnPosition" method of the <see cref="ServerMain"/> class.
    /// </summary>
    /// <param name="__instance">The instance of <see cref="ServerMain"/> this patch has been applied to.</param>
    /// <param name="__result">The <see cref="FuzzyEntityPos"/> value that would be returned from the original method.</param>
    /// <param name="playerUID">The player's UID.</param>
    /// <returns>
    ///     Returns the spawn position of the nearest enabled Respawn Beacon in range, or passes back to the original method, if no Beacons meet the criteria.
    /// </returns>
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(ServerMain), "GetSpawnPosition")]
    public static bool Patch_ServerMain_GetSpawnPosition_Prefix(ServerMain __instance, ref FuzzyEntityPos __result, string playerUID)
    {
        var player = __instance.PlayerByUid(playerUID);
        if (player.Entity.Alive) return true;
        var pos = player.Entity.Pos;

        var system = __instance.Api.ModLoader.GetModSystem<RespawnBeacon>();
        var cache = system.EnabledBeacons?
            .Where(p => p.Position is not null)
            .Where(p => p.Position.InRangeHorizontally((int)pos.X, (int)pos.Z, p.Radius))
            .OrderBy(p => p.Position.DistanceTo(pos.AsBlockPos));

        if (cache is null || !cache.Any()) return true;
        foreach (var beaconInfo in cache)
        {
            if (__instance.BlockAccessor.GetBlockEntity(beaconInfo.Position) is not BlockEntityRespawnBeacon beacon) continue;
            __result = beacon.SpawnPosition.With(p =>
            {
                p.Yaw = pos.Yaw;
                p.Pitch = pos.Pitch;
                p.Roll = pos.Roll;
            });
            return false;
        }
        return true;
    }

    /// <summary>
    ///     Applies a <see cref="HarmonyPrefix"/> patch to the "OnPlayerRespawn" method of the <see cref="ServerSystemEntitySimulation"/> class.
    /// </summary>
    /// <param name="player">The player currently being re-spawned.</param>
    /// <param name="__state">The player's spawn position, which gets passed to the postfix.</param>
    /// <returns>
    ///     Returns the spawn position of the nearest enabled Respawn Beacon in range, or passes back to the original method, if no Beacons meet the criteria.
    /// </returns>
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(ServerSystemEntitySimulation), "OnPlayerRespawn")]
    public static void ServerPatch_ServerSystemEntitySimulation_OnPlayerRespawn_Prefix(IServerPlayer player, out FuzzyEntityPos __state)
    {
        __state = player.GetSpawnPosition(false);
    }

    /// <summary>
    ///     Applies a <see cref="HarmonyPostfix"/> patch to the "OnPlayerRespawn" method of the <see cref="ServerSystemEntitySimulation"/> class.
    /// </summary>
    /// <param name="player">The player currently being re-spawned.</param>
    /// <param name="__state">The player's spawn position, passed from the prefix.</param>
    /// <returns>
    ///     Returns the spawn position of the nearest enabled Respawn Beacon in range, or passes back to the original method, if no Beacons meet the criteria.
    /// </returns>
    [HarmonyPostfix]
    [HarmonyServerPatch(typeof(ServerSystemEntitySimulation), "OnPlayerRespawn")]
    public static void ServerPatch_ServerSystemEntitySimulation_OnPlayerRespawn_Postfix(IServerPlayer player, FuzzyEntityPos __state)
    {
        var sapi = player.Entity.Api.ForServer()!;
        if (__state is null) return;
        var blockPos = __state.AsBlockPos.With(p => --p.Y);
        var blockEntity = sapi.World.BlockAccessor.GetBlockEntity(blockPos);
        if (blockEntity is not BlockEntityRespawnBeacon beacon) return;
        if (!beacon.Enabled) return;
        beacon.OnSpawn();
    }
}