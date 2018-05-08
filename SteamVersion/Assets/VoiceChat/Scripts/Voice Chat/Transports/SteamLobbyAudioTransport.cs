using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using VOCASY;
using VOCASY.Utility;
public class SteamLobbyAudioTransport : MonoBehaviour, IAudioTransportLayer
{
    public const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);

    public int MaxPacketLength { get { return temp1024.MaxCapacity - FirstPacketByteAvailable; } }

    private GamePacket temp1024;
    private GamePacket temp768;
    private GamePacket temp512;
    private GamePacket temp256;

    public VoicePacketInfo Receive(GamePacket buffer, GamePacket dataReceived, ulong netId)
    {
        throw new NotImplementedException();
    }

    public void SendToAllOthers(GamePacket data, VoicePacketInfo info)
    {
        throw new NotImplementedException();
    }
}