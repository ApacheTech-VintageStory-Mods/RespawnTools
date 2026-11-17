namespace RespawnTools.Features.RespawnBeacon;

public sealed class RespawnManager : UniversalModSystem<RespawnManager>
{
    private const string CHANNEL_NAME = "rt-respawn-manager";
    private IServerNetworkChannel? _serverChannel;
    private IClientNetworkChannel? _clientChannel;

    protected override void StartPreServerSide(ICoreServerAPI sapi)
    {
        _serverChannel = sapi.Network.GetOrRegisterChannel(Core, CHANNEL_NAME)
            .RegisterPacket<RespawnPacket>(Core, OnRespawnPacketRecieved)
            ;
    }

    private void OnRespawnPacketRecieved(IServerPlayer player, RespawnPacket packet)
    {
        var saveGameData = Sapi.ServerMain.GetField<SaveGame>("SaveGameData");
        var lives = saveGameData?.WorldConfiguration.GetString("playerlives", "-1").ToInt(-1) ?? -1;
        if (lives >= 0 && lives <= player.WorldData.Deaths)
        {
            player.SendMessage(GlobalConstants.CurrentChatGroup, "Cannot revive! All lives used up.", EnumChatType.CommandError, null);
            return;
        }

        player.Entity.Attributes.SetBool("rt-RespawnAtBeacon", packet.RespawnAtBeacon);        
        ServerMain.Logger.VerboseDebug("Received respawn request from {0}", player.PlayerName);
        Sapi.ServerMain.EventManager.TriggerPlayerRespawn(player);
    }

    protected override void StartPreClientSide(ICoreClientAPI capi)
    {
        _clientChannel = capi.Network.GetOrRegisterChannel(Core, CHANNEL_NAME)
            .RegisterPacket<RespawnPacket>(Core)
            ;
    }

    public void Respawn(bool respawnAtBeacon)
    {
        _clientChannel!.SendPacket<RespawnPacket>(new() { RespawnAtBeacon = respawnAtBeacon });
    }
}