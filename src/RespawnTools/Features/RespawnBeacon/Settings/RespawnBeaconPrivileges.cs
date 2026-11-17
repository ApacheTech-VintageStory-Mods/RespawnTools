using Gantry.Services.ExtendedEnums;

namespace RespawnTools.Features.RespawnBeacon.Settings;

/// <summary>
///     Contains privileges used to control respawn beacons.
/// </summary>
internal class RespawnBeaconPrivileges : StringEnum<RespawnBeaconPrivileges>
{
    /// <summary>
    ///     The ability to place Respawn Beacon blocks.
    /// </summary>
    public static string PlaceBeacons { get; } = Create("respawntools.beacons.place")!;

    /// <summary>
    ///     The ability to configure Respawn Beacon blocks.
    /// </summary>
    public static string ConfigureBeacons { get; } = Create("respawntools.beacons.configure")!;

    /// <summary>
    ///     The ability to use Respawn Beacon blocks.
    /// </summary>
    public static string UseBeacons { get; } = Create("respawntools.beacons.use")!;

    /// <summary>
    ///     The ability to break Respawn Beacon blocks.
    /// </summary>
    public static string BreakBeacons { get; } = Create("respawntools.beacons.break")!;
}