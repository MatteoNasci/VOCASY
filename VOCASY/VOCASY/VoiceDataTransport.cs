using UnityEngine;
using GENUtility;
namespace VOCASY
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
        /// Process packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <param name="dataReceived">Raw data received from network</param>
        /// <param name="startIndex">Raw data start index</param>
        /// <param name="length">Raw data length</param>
        /// <param name="netId">Sender net id</param>
        /// <returns>data info</returns>
        public abstract VoicePacketInfo ProcessReceivedData(BytePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId);
        /// <summary>
        /// Sends a packet to all the other clients
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        public abstract void SendToAllOthers(BytePacket data, VoicePacketInfo info);
        /// <summary>
        /// Sends a packet to another client
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        public abstract void SendTo(BytePacket data, VoicePacketInfo info, ulong receiverID);
        /// <summary>
        /// Sends a packet message to the target informing him whenever he has been muted/unmuted by the local client
        /// </summary>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        /// <param name="isReceiverMutedByLocal">True if receiver is muted and can avoid sending voice chat packets to the local client</param>
        public abstract void SendMessageIsMutedTo(ulong receiverID, bool isReceiverMutedByLocal);
    }
}