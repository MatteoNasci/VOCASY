using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using VOCASY;
using GENUtility;
using VOCASY.Common;
[CreateAssetMenu(menuName = "Steam/Transports/Lobby")]
public class SteamLobbyAudioTransport : VoiceDataTransport
{
    public const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);

    public override int MaxDataLength { get { return temp1024.MaxCapacity - FirstPacketByteAvailable; } }

    private BytePacket temp1024;
    private BytePacket temp768;
    private BytePacket temp512;
    private BytePacket temp256;
    public override VoicePacketInfo ProcessReceivedData(BytePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
    {
        throw new NotImplementedException();
    }

    public override void SendToAllOthers(BytePacket data, VoicePacketInfo info)
    {
        throw new NotImplementedException();
    }
    public override void SendTo(BytePacket data, VoicePacketInfo info, ulong receiverID)
    {
        throw new NotImplementedException();
    }
}