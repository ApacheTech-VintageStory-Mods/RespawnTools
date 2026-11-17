namespace RespawnTools.Features.RespawnBeacon;

[ProtoContract]
public sealed class RespawnPacket
{
    [ProtoMember(1)]
    public bool RespawnAtBeacon { get; set; }
}