using Gantry.Services.ExtendedEnums;

namespace RespawnTools.Features.RespawnTablet.Settings;

/// <summary>
///     Contains privileges used to control respawn beacons.
/// </summary>
internal class RespawnTabletPrivileges : StringEnum<RespawnTabletPrivileges>
{
    /// <summary>
    ///     The ability to use Respawn Tablets.
    /// </summary>
    public static string UseTablets { get; } = Create("respawntools.tablets.use")!;
}