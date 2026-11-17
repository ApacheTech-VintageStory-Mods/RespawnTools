using Gantry.Features.Nexus;
using Gantry.Features.NexusModMenu.Dialogue;

namespace RespawnTools;

internal sealed class Program : G.Host<Program>, IClientServiceRegistrar
{
    public void ConfigureClientModServices(IServiceCollection services, ICoreGantryAPI core)
    {
        services.AddModSettingsDialogue<ModSettingsDialogue>(isAdminMenu: true);
    }
}