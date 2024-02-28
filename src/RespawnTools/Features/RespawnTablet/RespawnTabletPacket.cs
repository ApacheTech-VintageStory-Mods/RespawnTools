using ProtoBuf;
using Vintagestory.API.MathTools;

namespace ApacheTech.VintageMods.RespawnTools.Features.RespawnTablet;

/// <summary>
///     A DTO, containing meta data information, required to synchronise Respawn Tablet item data between the client, and the server.
/// </summary>
[ProtoContract]
public sealed class RespawnTabletPacket
{
    /// <summary>
    ///     Gets or sets the position within the gameworld, of this specific Respawn Beacon.
    /// </summary>
    /// <value>An instance of <see cref="BlockPos"/>, giving an absolute position within the gameworld.</value>
    [ProtoMember(1)]
    public BlockPos Position { get; init; }
}