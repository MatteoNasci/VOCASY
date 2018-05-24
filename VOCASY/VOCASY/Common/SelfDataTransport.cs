﻿using UnityEngine;
using GENUtility;
using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that fakes network state between self and another client. It should be used for tests and debug
    /// </summary>
    [CreateAssetMenu(fileName = "SelfTransport", menuName = "VOCASY/DataTransports/Self")]
    public class SelfDataTransport : VoiceDataTransport
    {
        /// <summary>
        /// Header size
        /// </summary>
        public const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);
        /// <summary>
        /// Max final packet length
        /// </summary>
        public const int pLength = 1024;
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public override int MaxDataLength { get { return Internal_packetDataSize; } }
        /// <summary>
        /// To which id fake packets should be sent
        /// </summary>
        public ulong ReceiverId;

        /// <summary>
        /// Exposed for tests. Max payload size that can be processed
        /// </summary>
        [NonSerialized]
        public int Internal_packetDataSize = pLength - FirstPacketByteAvailable;

        /// <summary>
        /// Process packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <param name="dataReceived">Raw data received from network</param>
        /// <param name="startIndex">Raw data start index</param>
        /// <param name="length">Raw data length</param>
        /// <param name="netId">Sender net id</param>
        /// <returns>data info</returns>
        public override VoicePacketInfo ProcessReceivedData(BytePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
        {
            VoicePacketInfo info = new VoicePacketInfo();
            info.NetId = netId;
            info.Frequency = ByteManipulator.ReadUInt16(dataReceived, startIndex);
            startIndex += sizeof(ushort);
            info.Channels = ByteManipulator.ReadByte(dataReceived, startIndex);
            startIndex += sizeof(byte);
            info.Format = (AudioDataTypeFlag)ByteManipulator.ReadByte(dataReceived, startIndex);
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
        public override void SendToAllOthers(BytePacket data, VoicePacketInfo info)
        {
            //Debug.Log("packet sent to all others");
            BytePacket toSend = new BytePacket(Internal_packetDataSize);
            toSend.Write(ReceiverId, 0);
            toSend.Write(info.Frequency);
            toSend.Write(info.Channels);
            toSend.Write((byte)info.Format);

            int n;
            toSend.Copy(data, out n);

            toSend.CurrentSeek = sizeof(ulong);

            Workflow.ProcessReceivedPacket(toSend.Data, sizeof(ulong), toSend.CurrentLength - sizeof(ulong), ReceiverId);

        }
        /// <summary>
        /// Performs a normal SendToAllOthers
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        public override void SendTo(BytePacket data, VoicePacketInfo info, ulong receiverID)
        {
            SendToAllOthers(data, info);
        }
    }
}