using Gantry.Services.IO.Configuration.Attributes;

namespace RespawnTools.Features.RespawnTablet.Settings;

/// <summary>
///     Settings for the Respawn Tablet feature, allowing configuration of its properties and behaviour.
/// </summary>
public class RespawnTabletSettings : FeatureSettings<RespawnTabletSettings>
{
    /// <summary>
    ///     Determines whether the Respawn Tablet is craftable by players. 
    ///     If set to true, players can craft the Respawn Tablet in-game; if false, 
    ///     it cannot be crafted, but still will be available within the Creative menu.
    /// </summary>
    public bool Craftable { get; set; } = true;

    /// <summary>
    ///     The number of charges a tablet has before it breaks. Defaults to 1.
    /// </summary>
    [Slider<int>(defaultValue: 1, minValue: 1, maxValue: 10, step: 1)]
    public int Charges { get; set; } = 1;
}