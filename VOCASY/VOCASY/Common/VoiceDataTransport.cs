﻿using UnityEngine;
using VOCASY.Utility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages the voice packets received/sent
    /// </summary>
    public abstract class VoiceDataTransport : ScriptableObject
    {
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public abstract int MaxDataLength { get; }

        /// <summary>
        /// Voice chat workflow
        /// </summary>
        public VoiceDataWorkflow Workflow;

        /// <summary>
        /// Process packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <param name="dataReceived">Raw data received from network</param>
        /// <param name="startIndex">Raw data start index</param>
        /// <param name="length">Raw data length</param>
        /// <param name="netId">Sender net id</param>
        /// <returns>data info</returns>
        public abstract VoicePacketInfo ProcessReceivedData(GamePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId);
        /// <summary>
        /// Sends a packet to all the other clients
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        public abstract void SendToAllOthers(GamePacket data, VoicePacketInfo info);
        /// <summary>
        /// Sends a packet to another client
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        public abstract void SendTo(GamePacket data, VoicePacketInfo info, ulong receiverID);
    }
}