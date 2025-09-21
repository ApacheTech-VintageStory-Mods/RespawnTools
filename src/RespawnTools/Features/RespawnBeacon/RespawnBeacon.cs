using RespawnTools.Features.RespawnBeacon.GameContent.BlockEntities;
using RespawnTools.Features.RespawnBeacon.GameContent.Blocks;
using RespawnTools.Features.RespawnBeacon.Model;

namespace RespawnTools.Features.RespawnBeacon;

/// <summary>
///     Mod Entry-point for the RespawnBeacon feature.
/// </summary>
/// <seealso cref="UniversalModSystem" />
public sealed class RespawnBeacon : UniversalModSystem<RespawnBeacon>, IClientServiceRegistrar
{
    private static IFileSystemService? _fileSystem;
    private static IJsonModFile? _beaconCacheStore;

    public List<EnabledBeacon>? EnabledBeacons { get; private set; }

    public void ConfigureClientModServices(IServiceCollection services, ICoreGantryAPI capi) 
        => services.AddTransient<Dialogue.RespawnBeaconDialogue>();

    protected override void StartPreUniversal(ICoreAPI api)
    {
        _fileSystem = Core.Services.GetRequiredService<IFileSystemService>();
    }

    protected override void StartPreServerSide(ICoreServerAPI sapi)
    {
        _fileSystem?.RegisterFile("beacon-cache-server.json", ModFileType.Data, ModFileScope.World);
        _beaconCacheStore = _fileSystem?.GetJsonFile("beacon-cache-server.json");
    }

    /// <summary>
    ///     Updates a beacon, within the beacon cache.
    /// </summary>
    /// <param name="beacon">The beacon.</param>
    /// <param name="addEnabled">if set to <c>true</c> adds the beacon to the cache, if the beacon is enabled.</param>
    [ServerSide]
    public void UpdateBeaconCache(BlockEntityRespawnBeacon beacon, bool addEnabled)
    {
        if (beacon.Pos is null) return;
        if (EnabledBeacons is null) return;
        EnabledBeacons.RemoveAll(p => p.Position == null);
        EnabledBeacons.RemoveAll(p => p.Position == beacon.Pos);
        if (beacon.Enabled && addEnabled) EnabledBeacons.Add(EnabledBeacon.FromBlockEntity(beacon));
        _beaconCacheStore?.SaveFrom(EnabledBeacons);
    }

    /// <summary>
    ///     Side agnostic Start method, called after all mods received a call to StartPre().
    /// </summary>
    /// <param name="api">The API.</param>
    public override void Start(ICoreAPI api)
    {
        api.Network
            .RegisterChannel("RespawnBeacon")
            .RegisterMessageType<RespawnBeaconPacket>();
        api.RegisterBlock<BlockRespawnBeacon>();
        api.RegisterBlockEntity<BlockEntityRespawnBeacon>();
    }

    /// <summary>
    ///     Minor convenience method to save yourself the check for/cast to ICoreServerAPI in Start()
    /// </summary>
    /// <param name="api">The API.</param>
    public override void StartServerSide(ICoreServerAPI api)
    {
        EnabledBeacons = _beaconCacheStore?.ParseAsMany<EnabledBeacon>().ToList();
    }

    /// <summary>
    ///     If this mod allows runtime reloading, you must implement this method to unregister any listeners / handlers
    /// </summary>
    public override void Dispose()
    {
        EnabledBeacons?.Clear();
        EnabledBeacons = null;
        _fileSystem = null;
        _beaconCacheStore = null;
    }
}