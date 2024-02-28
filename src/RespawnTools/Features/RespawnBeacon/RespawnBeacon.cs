using System.Collections.Generic;
using System.Linq;
using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon.GameContent.BlockEntities;
using ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon.GameContent.Blocks;
using ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon.Model;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.Extensions;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon;

/// <summary>
///     Mod Entry-point for the RespawnBeacon feature.
/// </summary>
/// <seealso cref="UniversalModSystem" />
public sealed class RespawnBeacon : UniversalModSystem, IClientServiceRegistrar
{
    private static IFileSystemService _fileSystem;
    private static IJsonModFile _beaconCacheStore;

    public static List<EnabledBeacon> EnabledBeacons { get; private set; }

    public void ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi)
    {
        services.AddTransient<Dialogue.RespawnBeaconDialogue>();
    }

    protected override void StartPreUniversal(ICoreAPI api)
    {
        _fileSystem = IOC.Services.Resolve<IFileSystemService>();
    }

    protected override void StartPreServerSide(ICoreServerAPI sapi)
    {
        _fileSystem.RegisterFile("beacon-cache-server.json", FileScope.World);
        _beaconCacheStore = _fileSystem.GetJsonFile("beacon-cache-server.json");
    }

    /// <summary>
    ///     Updates a beacon, within the beacon cache.
    /// </summary>
    /// <param name="beacon">The beacon.</param>
    /// <param name="addEnabled">if set to <c>true</c> adds the beacon to the cache, if the beacon is enabled.</param>
    public static void UpdateBeaconCache(BlockEntityRespawnBeacon beacon, bool addEnabled)
    {
        if (beacon.Pos is null) return;
        if (EnabledBeacons is null) return;
        EnabledBeacons.RemoveAll(p => p.Position == null);
        EnabledBeacons.RemoveAll(p => p.Position == beacon.Pos);
        if (beacon.Enabled && addEnabled) EnabledBeacons.Add(EnabledBeacon.FromBlockEntity(beacon));
        _beaconCacheStore.SaveFrom(EnabledBeacons);
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
        EnabledBeacons = _beaconCacheStore.ParseAsMany<EnabledBeacon>().ToList();
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