using ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.GameContent.Behaviours;
using ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.GameContent.Items;
using Gantry.Core.Extensions;
using Gantry.Core.ModSystems;
using Vintagestory.API.Common;

// ReSharper disable StringLiteralTypo
// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet;

/// <summary>
///     Mod Entry-point for the RespawnTablet feature.
/// </summary>
/// <seealso cref="UniversalModSystem" />
public sealed class RespawnTablet : UniversalModSystem
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