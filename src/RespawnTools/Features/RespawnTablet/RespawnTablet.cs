using Gantry.GameContent.Behaviours;
using RespawnTools.Features.RespawnTablet.GameContent.Items;

namespace RespawnTools.Features.RespawnTablet;

/// <summary>
///     Mod Entry-point for the RespawnTablet feature.
/// </summary>
/// <seealso cref="UniversalModSystem" />
public sealed class RespawnTablet : UniversalModSystem<RespawnTablet>
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviour<ItemBehaviourConsumeOnDeath>("ConsumeOnDeath");
        api.RegisterCollectibleBehaviour<ItemBehaviourRespawnPlayerOnDeath>("RespawnPlayerOnDeath");
        api.RegisterCollectibleBehaviour<ItemBehaviourPreventPlayerDeath>("PreventPlayerDeath");
        api.RegisterItem<ItemRespawnTablet>("RespawnTablet");
        base.Start(api);
    }
}