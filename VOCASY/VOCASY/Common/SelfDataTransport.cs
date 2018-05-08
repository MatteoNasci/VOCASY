using System;
using System.Collections.Generic;
using UnityEngine;
using VOCASY.Utility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that fakes network state between self and another client. It should be used for tests and debug
    /// </summary>
    public class SelfDataTransport : MonoBehaviour, IAudioTransportLayer
    {
        private const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);

        private const int pLength = 1024;
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public int MaxPacketLength { get { return packetDataSize; } }
        /// <summary>
        /// To which id fake packets should be sent
        /// </summary>
        public ulong ReceiverId;

        [SerializeField]
        private int packetDataSize = pLength - FirstPacketByteAvailable;

        /// <summary>
        /// Receive packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <param name="dataReceived">Raw data received from network</param>
        /// <param name="netId">Sender net id</param>
        /// <returns>data info</returns>
        public VoicePacketInfo Receive(GamePacket buffer, GamePacket dataReceived, ulong netId)
        {
            VoicePacketInfo info = new VoicePacketInfo();
            info.NetId = netId;
            info.Frequency = dataReceived.ReadUShort();
            info.Channels = dataReceived.ReadByte();
            info.Format = (AudioDataTypeFlag)dataReceived.ReadByte();
            info.ValidPacketInfo = true;

            int n;
            buffer.Copy(dataReceived, out n);

            return info;
        }
        /// <summary>
        /// Sends a packet to all the other clients that need it
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        public void SendToAllOthers(GamePacket data, VoicePacketInfo info)
        {
            //Debug.Log("packet sent to all others");
            GamePacket toSend = GamePacket.CreatePacket(pLength);
            toSend.Write(ReceiverId, 0);
            toSend.Write(info.Frequency);
            toSend.Write(info.Channels);
            toSend.Write((byte)info.Format);

            int n;
            toSend.Copy(data, out n);

            toSend.CurrentSeek = sizeof(ulong);

            VoiceDataWorkflow.OnPacketAvailable(toSend, ReceiverId);
        }
    }
}