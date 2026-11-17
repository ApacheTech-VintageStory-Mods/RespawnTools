using RespawnTools.Features.RespawnTablet.Settings;

namespace RespawnTools.Features.RespawnTablet.Dialogue;

public class RespawnTabletAdminSettingsDialogue : SynchronisedSettingsDialogue<RespawnTabletSettings>
{
    public RespawnTabletAdminSettingsDialogue(ICoreGantryAPI gapi) 
        : base(gapi)
    {
        Alignment = EnumDialogArea.CenterMiddle;
        ModalTransparency = 0.4f;
        Movable = false;
    }
}