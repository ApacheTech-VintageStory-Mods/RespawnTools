[assembly: ModDependency("game", GameVersion.ShortGameVersion)]

[assembly: ModInfo(
    "Respawn Tools",
    "respawntools",
    Description = "Adds tools for servers, to alter the consequences of death, and respawning.",
    Side = "Universal",
#if DEBUG
    Version = $"{ModVersion.Value}-dev.0",
#else
    Version = ModVersion.Value,
#endif
    NetworkVersion = "1.0.0",
    IconPath = "modicon.png",
    Website = "https://apachetech.co.uk",
    Contributors = ["ApacheTech Solutions"],
    Authors = ["ApacheTech Solutions"])]

file static class ModVersion
{
    public const string Value = "3.0.0";
}