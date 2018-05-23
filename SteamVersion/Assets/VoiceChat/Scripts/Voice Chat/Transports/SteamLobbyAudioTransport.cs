using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using VOCASY;
using VOCASY.Utility;
using VOCASY.Common;
[CreateAssetMenu(menuName = "Steam/Transports/Lobby")]
public class SteamLobbyAudioTransport : VoiceDataTransport
{
    public const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);

    public override int MaxDataLength { get { return temp1024.MaxCapacity - FirstPacketByteAvailable; } }

    private GamePacket temp1024;
    private GamePacket temp768;
    private GamePacket temp512;
    private GamePacket temp256;
    public override VoicePacketInfo ProcessReceivedData(GamePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
    {
        throw new NotImplementedException();
    }

    public override void SendToAllOthers(GamePacket data, VoicePacketInfo info)
    {
        throw new NotImplementedException();
    }
    public override void SendTo(GamePacket data, VoicePacketInfo info, ulong receiverID)
    {
        throw new NotImplementedException();
    }
}