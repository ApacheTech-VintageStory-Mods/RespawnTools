using RespawnTools.Features.RespawnBeacon.Settings;

namespace RespawnTools.Features.RespawnBeacon.Dialogue;

public class RespawnBeaconAdminSettingsDialogue : SynchronisedSettingsDialogue<RespawnBeaconSettings>
{
    public RespawnBeaconAdminSettingsDialogue(ICoreGantryAPI gapi) 
        : base(gapi)
    {
        Alignment = EnumDialogArea.CenterMiddle;
        ModalTransparency = 0.4f;
        Movable = false;
    }
}