﻿using System;
using ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon.GameContent.BlockEntities;
using Gantry.Core;
using Gantry.Core.GameContent.GUI.Abstractions;
using Gantry.Core.Hosting.Annotation;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnBeacon.Dialogue;

/// <summary>
///     GUI to allow user to change the settings for a specific Respawn Beacon. This class cannot be inherited.
/// </summary>
/// <seealso cref="GenericDialogue" />
public sealed class RespawnBeaconDialogue : GenericDialogue
{
    private readonly BlockEntityRespawnBeacon _beacon;
    private bool _enabled;
    private int _radius;
    private int _ambientVolume;
    private int _respawnVolume;
    
    /// <summary>
    /// 	Initialises a new instance of the <see cref="RespawnBeaconDialogue"/> class.
    /// </summary>
    /// <param name="capi">The capi.</param>
    /// <param name="beacon">The beacon.</param>
    [SidedConstructor(EnumAppSide.Universal)]
    public RespawnBeaconDialogue(ICoreClientAPI capi, BlockEntityRespawnBeacon beacon)
        : base(capi)
    {
        _beacon = beacon;
        _enabled = _beacon.Enabled;
        _radius = _beacon.Radius;
        _ambientVolume = _beacon.AmbientVolume;
        _respawnVolume = _beacon.RespawnVolume;
        ModalTransparency = 0.4f;
        Alignment = EnumDialogArea.CenterMiddle;
        Title = LangEntry("Title");
    }

    /// <summary>
    ///     Gets an entry from the language files, for the feature this instance is representing.
    /// </summary>
    /// <param name="code">The entry to return.</param>
    /// <returns>A localised <see cref="string"/>, for the specified language file code.</returns>
    private static string LangEntry(string code) 
        => LangEx.FeatureString("RespawnBeacon.Dialogue", code);

    /// <summary>
    ///     Gets or sets the action to perform when the used presses the OK button.
    /// </summary>
    /// <value>The <see cref="Action{RespawnBeaconPacket}"/> action to perform.</value>
    public Action<RespawnBeaconPacket> OnOkAction { private get; set; }

    /// <summary>
    ///     Refreshes the displayed values on the form.
    /// </summary>
    protected override void RefreshValues()
    {
        if (!IsOpened()) return;
        SingleComposer.GetSlider("sldRadius").SetValues(_radius, 0, 128, 1);
        SingleComposer.GetSlider("sldAmbientVolume").SetValues(_ambientVolume, 0, 100, 1);
        SingleComposer.GetSlider("sldRespawnVolume").SetValues(_respawnVolume, 0, 100, 1);
        SingleComposer.GetSwitch("btnEnabled").SetValue(_enabled);
    }

    /// <summary>
    ///     Composes the header for the GUI.
    /// </summary>
    /// <param name="composer">The composer.</param>
    protected override void ComposeBody(GuiComposer composer)
    {
        var labelFont = CairoFont.WhiteSmallText();
        var topBounds = ElementBounds.FixedSize(600, 30);

        var left = ElementBounds.FixedSize(200, 30).FixedUnder(topBounds, 10);
        var right = ElementBounds.FixedSize(270, 30).FixedUnder(topBounds, 10).FixedRightOf(left, 10);
        var controlRowBoundsRightFixed = ElementBounds.FixedSize(200, 30).WithAlignment(EnumDialogArea.RightFixed);

        composer
            .AddStaticText(LangEntry("lblRadius"), labelFont, EnumTextOrientation.Right, left)
            .AddHoverText(LangEntry("lblRadius.HoverText"), labelFont, 260, left)
            .AddSlider(OnRadiusChanged, right, "sldRadius");


        left = ElementBounds.FixedSize(200, 30).FixedUnder(left, 10);
        right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

        composer
            .AddStaticText(LangEntry("lblAmbientVolume"), labelFont, EnumTextOrientation.Right, left)
            .AddHoverText(LangEntry("lblAmbientVolume.HoverText"), labelFont, 260, left)
            .AddSlider(OnAmbientVolumeChanged, right, "sldAmbientVolume");


        left = ElementBounds.FixedSize(200, 30).FixedUnder(left, 10);
        right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

        composer
            .AddStaticText(LangEntry("lblRespawnVolume"), labelFont, EnumTextOrientation.Right, left)
            .AddHoverText(LangEntry("lblRespawnVolume.HoverText"), labelFont, 260, left)
            .AddSlider(OnRespawnVolumeChanged, right, "sldRespawnVolume");


        left = ElementBounds.FixedSize(200, 30).FixedUnder(left, 10);
        right = ElementBounds.FixedSize(270, 30).FixedUnder(right, 10).FixedRightOf(left, 10);

        composer
            .AddStaticText(LangEntry("lblEnabled"), labelFont, EnumTextOrientation.Right, left)
            .AddHoverText(LangEntry("lblEnabled.HoverText"), labelFont, 260, left)
            .AddSwitch(OnEnableToggle, right, "btnEnabled");

        composer.AddSmallButton(LangEx.ConfirmationString("ok"), OnOkButtonPressed, controlRowBoundsRightFixed.FixedUnder(right, 10));
    }

    private bool OnOkButtonPressed()
    {
        OnOkAction(new RespawnBeaconPacket
        {
            Enabled = _enabled,
            Radius = _radius,
            AmbientVolume = _ambientVolume,
            RespawnVolume = _respawnVolume,
            Position = _beacon.Pos
        });
        return TryClose();
    }

    private void OnEnableToggle(bool state)
    {
        _enabled = state;
    }

    private bool OnRadiusChanged(int radius)
    {
        _radius = radius;
        return true;
    }

    private bool OnAmbientVolumeChanged(int ambientVolume)
    {
        _ambientVolume = ambientVolume;
        return true;
    }

    private bool OnRespawnVolumeChanged(int respawnVolume)
    {
        _respawnVolume = respawnVolume;
        return true;
    }
}