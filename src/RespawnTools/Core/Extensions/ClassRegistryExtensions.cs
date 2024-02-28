using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.RespawnTools.Core.Extensions;

public static class ClassRegistryExtensions
{
    public static void RegisterCollectibleBehaviour<T>(this ICoreAPI api, string name = null)
    {
        name ??= nameof(T);
        api.RegisterCollectibleBehaviorClass(name, typeof(T));
    }

    public static void RegisterItem<T>(this ICoreAPI api, string name = null)
    {
        name ??= nameof(T);
        api.RegisterItemClass(name, typeof(T));
    }
}