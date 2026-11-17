using RespawnTools.Features.RespawnBeacon.Settings;

namespace RespawnTools.Features.RespawnBeacon.Patches;

public sealed class GuiDialogDeadPatches : GantryPatch
{
    [HarmonyPrefix]
    [HarmonyClientPatch(typeof(GuiDialogDead), "ComposeDialog")]
    public static bool ClientPatch_GuiDialogDead_ComposeDialog_Prefix(
        GuiDialogDead __instance,
        ClientMain ___game,
        int ___livesLeft,
        bool ___respawning,
        ref double ___ingameMinutesRevivableLeft,
        ref int ___prevIngMinLeftInt
        )
    {
        __instance.ClearComposers();
        __instance.Composers["backgroundd"] = ___game.GuiComposers.Create("deadbg", ElementBounds.Fill).AddGrayBG(ElementBounds.Fill).Compose(true);
        var deadMsg = Lang.Get("Congratulations, you died!", []);
        if (___livesLeft > 0)
        {
            deadMsg = Lang.Get("Congratulations, you died! {0} lives left.", ___livesLeft);
        }
        if (___livesLeft == 0)
        {
            deadMsg = Lang.Get("Congratulations, you died! Forever!");
        }
        var respText = (___respawning ? Lang.Get("Respawning...") : Lang.Get("Respawn"));
        var reviveFont = CairoFont.WhiteSmallText().WithOrientation(EnumTextOrientation.Center);
        var buttonWidth = 300.0;
        var canUseBeacons = Gantry.Capi.World.Player.HasPrivilege(RespawnBeaconPrivileges.UseBeacons) && BeaconInRange();
        var quitButtonRow = canUseBeacons ? 2.6f : 2f;

        __instance.Composers["menu"] = ___game.GuiComposers.Create("deadmenu", ElementStdBounds.AutosizedMainDialog.WithFixedAlignmentOffset(0.0, 40.0)).AddShadedDialogBG(ElementStdBounds.DialogBackground().WithFixedPadding(0.0, GuiStyle.ElementToDialogPadding), false, 5.0, 0.75f)
            .BeginChildElements()
                .AddStaticText(deadMsg, CairoFont.WhiteSmallishText(), EnumTextOrientation.Center, ElementStdBounds.MenuButton(0f, EnumDialogArea.CenterFixed).WithFixedWidth(350.0), null)
                .AddDynamicText("", reviveFont, ElementStdBounds.MenuButton(0.35f, EnumDialogArea.CenterFixed).WithFixedSize(350.0, 25.0), "reviveCountdown")
                .AddIf(___livesLeft != 0)
                    .AddButton(respText, () => OnRespawn(__instance, false), ElementStdBounds.MenuButton(1f, EnumDialogArea.CenterFixed).WithFixedWidth(buttonWidth), EnumButtonStyle.Normal, "respawnbtn")
                    .AddIf(canUseBeacons)
                        .AddButton(Gantry.Lang.Translate("RespawnBeacon", "RespawnAtBeacon"), () => OnRespawn(__instance, true), ElementStdBounds.MenuButton(1.6f, EnumDialogArea.CenterFixed).WithFixedWidth(buttonWidth), EnumButtonStyle.Normal, "btnRespawnAtBeacon")
                    .EndIf()
                .EndIf()
                .AddIf(___livesLeft == 0 && ___game.IsSingleplayer)
                    .AddButton(Lang.Get("Delete World"), new ActionConsumable(() => __instance.CallMethod<bool>("OnDeleteWorld")), ElementStdBounds.MenuButton(1f, EnumDialogArea.CenterFixed).WithFixedWidth(buttonWidth), EnumButtonStyle.Normal, "deletebtn")
                .EndIf()
                .AddButton(Lang.Get("Rage Quit"), new ActionConsumable(() => __instance.CallMethod<bool>("OnLeaveWorld")), ElementStdBounds.MenuButton(quitButtonRow, EnumDialogArea.CenterFixed).WithFixedWidth(buttonWidth), EnumButtonStyle.Normal, null)
            .EndChildElements()
            .Compose(true);
        __instance.Composers["menu"].GetButton("respawnbtn")?.Enabled = !___respawning;
        if (!___respawning && (!___game.IsSingleplayer || ___game.OpenedToLan))
        {
            ___ingameMinutesRevivableLeft = ___game.player.Entity.RevivableIngameHoursLeft() * 60.0;
            ___prevIngMinLeftInt = (int)___ingameMinutesRevivableLeft;
            GuiElementDynamicText dynamicText = __instance.Composers["menu"].GetDynamicText("reviveCountdown");
            if (dynamicText == null)
            {
                return false;
            }
            dynamicText.SetNewText(Lang.Get("ingame-minutes-left-for-player-revival", (int)___ingameMinutesRevivableLeft), false, false, false);
        }
        return false;
    }

    private static bool BeaconInRange()
        => Gantry.ModSystem<RespawnBeacon>().BeaconInRange();

    private static bool OnRespawn(GuiDialogDead __instance, bool respawnAtBeacon)
    {
        __instance.SetField("respawning", true);
        __instance.CallMethod("ComposeDialog");
        Gantry.ModSystem<RespawnManager>().Respawn(respawnAtBeacon);
        return true;
    }
}