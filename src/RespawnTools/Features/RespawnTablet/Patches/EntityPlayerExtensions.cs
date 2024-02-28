using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet.Patches;

[UsedImplicitly(ImplicitUseTargetFlags.All)]
public static class EntityPlayerExtensions
{
    public static void Damage(this EntityPlayer player, float hp)
    {
        player.ReceiveDamage(new DamageSource
        {
            Source = EnumDamageSource.Internal
        }, hp);
    }

    public static void Heal(this EntityPlayer player, float hp)
    {
        player.ReceiveDamage(new DamageSource
        {
            Source = EnumDamageSource.Revive
        }, hp);
    }
}