using Gantry.Features.NexusModMenu.Abstractions;
using Gantry.Services.IO.Configuration.Synchonisation.Abstractions;
using Gantry.Services.IO.DataStructures;
using RespawnTools.Features.RespawnBeacon.Dialogue;
using RespawnTools.Features.RespawnBeacon.GameContent.BlockEntities;
using RespawnTools.Features.RespawnBeacon.GameContent.Blocks;
using RespawnTools.Features.RespawnBeacon.Model;
using RespawnTools.Features.RespawnBeacon.Settings;

namespace RespawnTools.Features.RespawnBeacon;

/// <summary>
///     Mod Entry-point for the RespawnBeacon feature.
/// </summary>
/// <seealso cref="UniversalModSystem{RespawnBeacon}" />
public sealed class RespawnBeacon : UniversalModSystem<RespawnBeacon>, IClientServiceRegistrar, IServerServiceRegistrar
{
    private static IFileSystemService? _fileSystem;
    private static IJsonModFile? _beaconCacheStore;

    public List<EnabledBeacon>? EnabledBeacons { get; private set; }


    private RespawnBeaconSettings _settings = new();
    private IDisposable? _craftableSub;


    public void ConfigureServerModServices(IServiceCollection services, ICoreGantryAPI capi)
    {
        services.AddFeatureWorldSettings<RespawnBeaconSettings>();
    }

    public void ConfigureClientModServices(IServiceCollection services, ICoreGantryAPI capi)
    {
        services.AddTransient<RespawnBeaconAdminSettingsDialogue>();
        services.AddTransient<RespawnBeaconDialogue>();
    }

    protected override void StartPreUniversal(ICoreAPI api)
    {
        _fileSystem = Core.Services.GetRequiredService<IFileSystemService>();
    }

    protected override void StartPreServerSide(ICoreServerAPI sapi)
    {
        _fileSystem?.RegisterFile("beacon-cache-server.json", ModFileType.Data, ModFileScope.World);
        _beaconCacheStore = _fileSystem?.GetJsonFile("beacon-cache-server.json");

        sapi.Permissions.RegisterPrivilege(RespawnBeaconPrivileges.PlaceBeacons, T($"Privilieges.Description.{RespawnBeaconPrivileges.PlaceBeacons}"), adminAutoGrant: true);
        sapi.Permissions.RegisterPrivilege(RespawnBeaconPrivileges.ConfigureBeacons, T($"Privilieges.Description.{RespawnBeaconPrivileges.ConfigureBeacons}"), adminAutoGrant: true);
        sapi.Permissions.RegisterPrivilege(RespawnBeaconPrivileges.UseBeacons, T($"Privilieges.Description.{RespawnBeaconPrivileges.UseBeacons}"), adminAutoGrant: true);
        sapi.Permissions.RegisterPrivilege(RespawnBeaconPrivileges.BreakBeacons, T($"Privilieges.Description.{RespawnBeaconPrivileges.BreakBeacons}"), adminAutoGrant: true);
        foreach (var role in sapi.Server.Config.Roles)        
            role.Privileges.AddIfNotPresent(RespawnBeaconPrivileges.UseBeacons);
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
    public override void StartServerSide(ICoreServerAPI sapi)
    {
        EnabledBeacons = _beaconCacheStore?.ParseAsMany<EnabledBeacon>().ToList();
        _settings = Core.Services.GetRequiredService<RespawnBeaconSettings>();
        _settings.AddPropertyChangedAction(p => p.Craftable, ToggleGridRecipes);
    }

    private void ToggleGridRecipes(bool craftable) 
        => UApi.World.ToggleGridRecipes("respawntools:respawnbeacon", craftable);

    public override void StartClientSide(ICoreClientAPI capi)
    {
        var scs = Core.Services.Resolve<ISynchronisedClientSettings>();
        _craftableSub = scs.SynchroniseValue<RespawnBeaconSettings, bool>(
            scope: ModFileScope.World,
            propertyExpression: p => p.Craftable,
            onUpdated: ToggleGridRecipes);

        Core.Resolve<IFeatureSettingsDialoguesProvider>()
            .Add<RespawnBeaconAdminSettingsDialogue>(Core, "RespawnBeacon", adminOnly: true);
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
        _craftableSub?.Dispose();
    }

    /// <summary>
    ///     Helper for translating text keys for this feature.
    /// </summary>
    /// <param name="code">Translation code/key.</param>
    /// <param name="args">Optional format arguments.</param>
    /// <returns>Translated string from the language system.</returns>
    private string T(string code, params object[] args)
        => Core.Lang.Translate("RespawnBeacons", code, args);
}