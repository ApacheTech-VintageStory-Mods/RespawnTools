using Gantry.Features.NexusModMenu.Abstractions;
using Gantry.GameContent.Behaviours;
using Gantry.GameContent.ChatCommands;
using Gantry.Services.IO.Configuration.Synchonisation.Abstractions;
using Gantry.Services.IO.DataStructures;
using RespawnTools.Features.RespawnTablet.Dialogue;
using RespawnTools.Features.RespawnTablet.GameContent.Items;
using RespawnTools.Features.RespawnTablet.Settings;
using System.Diagnostics;
using System.Runtime;

namespace RespawnTools.Features.RespawnTablet;

/// <summary>
///     Mod Entry-point for the RespawnTablet feature.
/// </summary>
/// <seealso cref="UniversalModSystem" />
public sealed class RespawnTablet : UniversalModSystem<RespawnTablet>, IClientServiceRegistrar, IServerServiceRegistrar
{
    private IDisposable? _settingsSync;

    public RespawnTabletSettings Settings { get; private set; } = default!;

    public void ConfigureClientModServices(IServiceCollection services, ICoreGantryAPI capi)
    {
        services.AddTransient<RespawnTabletAdminSettingsDialogue>();
    }

    public void ConfigureServerModServices(IServiceCollection services, ICoreGantryAPI capi)
    {
        services.AddFeatureWorldSettings<RespawnTabletSettings>();
    }

    protected override void StartPreServerSide(ICoreServerAPI sapi)
    {
        Settings = Core.Services.GetRequiredService<RespawnTabletSettings>();
        Settings.AddPropertyChangedAction(p => p.Craftable, ToggleGridRecipes);
        sapi.Permissions.RegisterPrivilege(RespawnTabletPrivileges.UseTablets, G.T("RespawnTablet", $"Privilieges.Description.{RespawnTabletPrivileges.UseTablets}"), adminAutoGrant: true);
        foreach (var role in sapi.Server.Config.Roles)
            role.Privileges.AddIfNotPresent(RespawnTabletPrivileges.UseTablets);
        AddDebugCommands(sapi);
    }

    [Conditional("DEBUG")]
    private void AddDebugCommands(ICoreServerAPI sapi)
    {
        var command = sapi.ChatCommands
            .Create("rt-tablet")
            .RequiresPrivilege(Privilege.controlserver);

        command
            .BeginSubCommand("charges")
            .WithArgs(sapi.ChatCommands.Parsers.Int("charges"))
            .HandleWith(args =>
            {
                var player = args.Caller.Player;
                var slot = player.InventoryManager.ActiveHotbarSlot;
                var stack = slot.Itemstack;
                if (stack?.Item is not ItemRespawnTablet tablet) return;

                var charges = args.Parsers[0].To<IntArgParser>().Value<int>();
                stack.Attributes.SetInt("charges", charges);
                slot.MarkDirty();
            })
            .EndSubCommand();

        command
            .BeginSubCommand("recharge")
            .HandleWith(args =>
            {
                var player = args.Caller.Player;
                var slot = player.InventoryManager.ActiveHotbarSlot;
                var stack = slot.Itemstack;
                if (stack?.Item is not ItemRespawnTablet tablet) return;

                stack.Attributes.SetInt("charges", Settings.Charges);
                slot.MarkDirty();
            })
            .EndSubCommand();

        command
            .BeginSubCommand("consume")
            .HandleWith(args =>
            {
                var player = args.Caller.Player;
                var slot = player.InventoryManager.ActiveHotbarSlot;
                if (slot.Itemstack?.Item is not ItemRespawnTablet tablet) return;

                slot.TakeOutWhole();
                slot.MarkDirty();
            })
            .EndSubCommand();
    }

    private void ToggleGridRecipes(bool craftable)
        => UApi.World.ToggleGridRecipes("respawntools:respawntablet", craftable);

    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviour<ItemBehaviourConsumeOnDeath>("ConsumeOnDeath");
        api.RegisterCollectibleBehaviour<ItemBehaviourRespawnPlayerOnDeath>("RespawnPlayerOnDeath");
        api.RegisterCollectibleBehaviour<ItemBehaviourPreventPlayerDeath>("PreventPlayerDeath");
        api.RegisterItem<ItemRespawnTablet>("RespawnTablet");
    }

    protected override void StartPreClientSide(ICoreClientAPI api)
    {
        var scs = Core.Services.Resolve<ISynchronisedClientSettings>();
        _settingsSync = scs.Synchronise<RespawnTabletSettings>(
            scope: ModFileScope.World,
            onUpdated: s => { Settings = s; ToggleGridRecipes(s.Craftable); });

        Core.Resolve<IFeatureSettingsDialoguesProvider>()
            .Add<RespawnTabletAdminSettingsDialogue>(Core, "RespawnTablet", adminOnly: true);
    }

    public override void Dispose()
    {
        _settingsSync?.Dispose();
    }
}