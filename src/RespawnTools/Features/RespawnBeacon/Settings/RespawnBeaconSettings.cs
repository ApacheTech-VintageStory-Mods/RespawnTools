namespace RespawnTools.Features.RespawnBeacon.Settings;

/// <summary>
///     Settings for the Respawn Beacon feature, allowing configuration of its properties and behaviour.
/// </summary>
public class RespawnBeaconSettings : FeatureSettings<RespawnBeaconSettings>
{
    /// <summary>
    ///     Determines whether the Respawn Beacon is craftable by players. 
    ///     If set to true, players can craft the Respawn Beacon in-game; if false, 
    ///     it cannot be crafted, but still will be available within the Creative menu.
    /// </summary>
    public bool Craftable { get; set; } = true;
}