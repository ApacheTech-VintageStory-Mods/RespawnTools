using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.GameContent.Behaviours;

[UsedImplicitly(ImplicitUseTargetFlags.All)]
public class ItemBehaviourPreventPlayerDeath : CollectibleBehavior
{
    public float HealthRecovered { get; private set; }

    public float GodModeCountdown { get; private set; }

    public ItemBehaviourPreventPlayerDeath(CollectibleObject collectible) : base(collectible)
    {
        
    }
}