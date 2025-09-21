namespace RespawnTools.Features.RespawnTablet.GameContent.Items;

/// <summary>
///     Represents the template for a Respawn Tablet block, within the game.
/// </summary>
/// <seealso cref="Item" />
public class ItemRespawnTablet : Item
{
    /// <summary>
    ///     The particles to emit from the player, when this ability is triggered.
    /// </summary>
    private SimpleParticleProperties? _particles;

    /// <summary>
    ///     Called when the block is loaded by the game.
    ///     This initialises the particle properties used when the tablet is activated.
    /// </summary>
    /// <param name="coreApi">The core API.</param>
    public override void OnLoaded(ICoreAPI coreApi)
    {
        base.OnLoaded(coreApi);

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

    /// <summary>
    ///     Triggered on the server when the tablet is used on a dead player.
    ///     This method spawns particle effects, plays a sound effect, revives the player
    ///     and consumes the tablet item from the provided slot.
    /// </summary>
    /// <param name="byPlayer">The player to be revived.</param>
    /// <param name="slot">The item slot from which the tablet is consumed.</param>
    [RunsOn(EnumAppSide.Server)]
    public void OnPlayerDeath(EntityPlayer byPlayer, ItemSlot slot)
    {
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
        slot.TakeOutWhole();
        slot.MarkDirty();
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