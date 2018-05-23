using System;
using System.Collections.Generic;
using UnityEngine;
using VOCASY.Utility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that fakes network state between self and another client. It should be used for tests and debug
    /// </summary>
    [CreateAssetMenu(fileName = "SelfTransport", menuName = "VOCASY/DataTransports/Self")]
    public class SelfDataTransport : VoiceDataTransport
    {
        private const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);

        private const int pLength = 1024;
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public override int MaxDataLength { get { return packetDataSize; } }
        /// <summary>
        /// To which id fake packets should be sent
        /// </summary>
        public ulong ReceiverId;

        [SerializeField]
        private int packetDataSize = pLength - FirstPacketByteAvailable;

        /// <summary>
        /// Process packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <param name="dataReceived">Raw data received from network</param>
        /// <param name="startIndex">Raw data start index</param>
        /// <param name="length">Raw data length</param>
        /// <param name="netId">Sender net id</param>
        /// <returns>data info</returns>
        public override VoicePacketInfo ProcessReceivedData(GamePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
        {
            VoicePacketInfo info = new VoicePacketInfo();
            info.NetId = netId;
            info.Frequency = Utils.ReadUInt16(dataReceived, startIndex);
            startIndex += sizeof(ushort);
            info.Channels = Utils.ReadByte(dataReceived, startIndex);
            startIndex += sizeof(byte);
            info.Format = (AudioDataTypeFlag)Utils.ReadByte(dataReceived, startIndex);
            startIndex += sizeof(byte);
            info.ValidPacketInfo = true;

            buffer.WriteByteData(dataReceived, startIndex, length - sizeof(ushort) - sizeof(byte) - sizeof(byte));

            return info;
        }
        /// <summary>
        /// Sends a packet to all the other clients that need it
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        public override void SendToAllOthers(GamePacket data, VoicePacketInfo info)
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

            Workflow.ProcessReceivedPacket(toSend.Data, sizeof(ulong), toSend.CurrentLength - sizeof(ulong), ReceiverId);

            toSend.DisposePacket();
        }
        /// <summary>
        /// Performs a normal SendToAllOthers
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        public override void SendTo(GamePacket data, VoicePacketInfo info, ulong receiverID)
        {
            SendToAllOthers(data, info);
        }
    }
}