using RespawnTools.Features.RespawnBeacon.Model;

namespace RespawnTools.Features.RespawnBeacon;

[ProtoContract]
public sealed class BeaconCachePacket
{
    [ProtoMember(1)]
    public List<EnabledBeacon> EnabledBeacons { get; set; } = [];
}