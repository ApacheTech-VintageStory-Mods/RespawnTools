using RespawnTools.Features.RespawnTablet.Settings;
using System.Runtime.InteropServices;
using System.Text;
using Vintagestory.ServerMods;

namespace RespawnTools.Features.RespawnTablet.GameContent.Items;

/// <summary>
///     Represents the template for a Respawn Tablet item, within the game.
/// </summary>
/// <seealso cref="Item" />
public class ItemRespawnTablet : Item
{
    private ICoreGantryAPI _core = default!;
    private RespawnTablet _system = default!;
    private static readonly SimpleParticleProperties? _particles;


    static ItemRespawnTablet()
    {
        _particles = new SimpleParticleProperties(
            minQuantity: 10f,
            maxQuantity: 100f,
            color: 0,
            minPos: new Vec3d(),
            maxPos: new Vec3d(),
            minVelocity: new Vec3f(-0.5f, -0.5f, -0.5f),
            maxVelocity: new Vec3f(0.5f, 1.5f, 0.5f),
            lifeLength: 6.0f,
            gravityEffect: 0f,
            minSize: 0.3f,
            maxSize: 1.25f)
        {
            SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -0.6f)
        };
        _particles.AddPos.Set(1.0, 2.0, 1.0);
        _particles.addLifeLength = 0.5f;
    }

    public override void OnCreatedByCrafting(ItemSlot[] allInputSlots, ItemSlot outputSlot, IRecipeBase byRecipe)
    {
        base.OnCreatedByCrafting(allInputSlots, outputSlot, byRecipe);
        SyncCharges(outputSlot);
    }

    private int SyncCharges(ItemSlot slot)
    {
        var charges = slot.Itemstack!.Attributes.GetAsInt("charges", -1);
        if (charges == -1)
        {
            charges = _system.Settings.Charges;
            slot.Itemstack!.Attributes.SetInt("charges", charges);
        }
        return charges;
    }

    /// <summary>
    ///     Called when the block is loaded by the game.
    ///     This initialises the particle properties used when the tablet is activated.
    /// </summary>
    /// <param name="api">The core API.</param>
    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        _core = api.GantryCore();
        _system = api.ModLoader.GetModSystem<RespawnTablet>();
    }

    /// <summary>
    ///     Triggered on the server when the tablet is used on a dead player.
    ///     This method spawns particle effects, plays a sound effect, revives the player
    ///     and consumes the tablet item from the provided slot.
    /// </summary>
    /// <param name="byPlayer">The player to be revived.</param>
    /// <param name="slot">The item slot from which the tablet is consumed.</param>
    [ServerSide]
    public void OnPlayerDeath(EntityPlayer byPlayer, ItemSlot slot)
    {
        if (!byPlayer.Player.HasPrivilege(RespawnTabletPrivileges.UseTablets)) return;

        // Particles.
        var listenerId = api.World.RegisterGameTickListener(_ => SpawnParticlesAtPlayer(byPlayer), 40);
        var timer = new System.Timers.Timer(TimeSpan.FromSeconds(6));
        timer.Elapsed += (_, _) =>
        {
            api.World.UnregisterGameTickListener(listenerId);
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();

        // SFX.
        var soundFile = AssetLocation.Create("respawntools:sounds/respawntablet-use");
        byPlayer.Api.World.PlaySoundAt(soundFile, byPlayer);

        // Action.
        byPlayer.Revive();

        // Consume.
        var charges = SyncCharges(slot);
        if (--charges == 0)
            slot.TakeOutWhole();
        else
            slot.Itemstack!.Attributes.SetInt("charges", charges);
        slot.MarkDirty();
    }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        var itemstack = inSlot.Itemstack!;
        var lightHsv = GetLightHsv(world.BlockAccessor, null, itemstack);
        dsc.Append((!withDebugInfo) ? "" : ((lightHsv[2] > 0) ? (Lang.Get("light-hsv") + lightHsv[0] + ", " + lightHsv[1] + ", " + lightHsv[2] + "\n") : ""));
        dsc.Append(withDebugInfo ? "" : ((lightHsv[2] > 0) ? (Lang.Get("light-level") + lightHsv[2] + "\n") : ""));
        if (withDebugInfo)
        {
            dsc.AppendLine("<font color=\"#bbbbbb\">Id:" + Id + "</font>");
            dsc.AppendLine(string.Concat("<font color=\"#bbbbbb\">Code: ", Code, "</font>"));
            if (world.Api is ICoreClientAPI capi)
            {
                if (capi.Input.KeyboardKeyStateRaw[1])
                    dsc.AppendLine("<font color=\"#bbbbbb\">Attributes: " + inSlot.Itemstack!.Attributes.ToJsonToken() + "</font>\n");
                
                var sb = new StringBuilder(1024);
                sb.AppendJoin(", ", capi.CollectibleTagRegistry.SlowEnumerateTagNames(GetTags(inSlot.Itemstack)));
                if (sb.Length > 0)
                    dsc.AppendLine($"<font color=\"#bbbbbb\">Tags: {sb.ToString()}</font>");                
            }
        }
        dsc.AppendLine(Lang.Get("Mod: {0}", "Respawn Tools"));
        var charges = SyncCharges(inSlot);
        var message = _core.Lang.Translate("RespawnTablet", "Charges", charges);
        dsc.AppendLine(message);
    }

    /// <summary>
    ///     Spawns the configured particle effect at the player's current position.
    /// </summary>
    /// <param name="byPlayer">The player at whose position the particles will be spawned.</param>
    private void SpawnParticlesAtPlayer(EntityPlayer byPlayer)
    {
        var pos = byPlayer.Pos;
        var particles = _particles!.With(p =>
        {
            p.UseLighting();
            p.MinPos = new Vec3d(pos.X, pos.Y, pos.Z);
            p.Color = ColorUtil.ColorFromRgba(
                GameMath.Clamp(RandomEx.RandomValueAround(204, 20), 0, 255),
                GameMath.Clamp(RandomEx.RandomValueAround(235, 20), 0, 255),
                GameMath.Clamp(RandomEx.RandomValueAround(178, 20), 0, 255),
                GameMath.Clamp(RandomEx.RandomValueAround(128, 127), 0, 255));
        });
        api.World.SpawnParticles(particles);
    }
}