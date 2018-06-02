using UnityEngine;
using GENUtility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages the transport side of the Voice Chat Workflow.
    /// </summary>
    [CreateAssetMenu(fileName = "Transport", menuName = "VOCASY/DataTransports/Transport")]
    public class Transport : VoiceDataTransport
    {
        /// <summary>
        /// Delegate used when data is requested to be sent to a specific target
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="startIndex">data start index</param>
        /// <param name="length">data length</param>
        /// <param name="target">target to send packet</param>
        public delegate void SendToTarget(byte[] data, int startIndex, int length, ulong target);
        /// <summary>
        /// Delegate used when data is requested to be sent to all
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="startIndex">data start index</param>
        /// <param name="length">data length</param>
        public delegate void SendToAllTargets(byte[] data, int startIndex, int length);
        /// <summary>
        /// Delegate used to send a packet message to the target informing him whenever he has been muted/unmuted by the local client.
        /// </summary>
        /// <param name="targetID">Target to which the packet should be sent</param>
        /// <param name="isTargetMutedByLocal">True if target is muted and can avoid sending voice chat packets to the local client</param>
        public delegate void SendMessageIsMutedToTarget(ulong targetID, bool isTargetMutedByLocal);
        /// <summary>
        /// Header size
        /// </summary>
        public const int FirstPacketByteAvailable = sizeof(ulong) + sizeof(ushort) + sizeof(byte) + sizeof(byte);
        /// <summary>
        /// Max final packet length
        /// </summary>
        public const int PLength = 1000;
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public override int MaxDataLength { get { return PLength - FirstPacketByteAvailable; } }
        /// <summary>
        /// Action invoked when data is requested to be sent to a specific target
        /// </summary>
        public SendToTarget SendToAction;
        /// <summary>
        /// Action invoked when data is requested to be sent to all
        /// </summary>
        public SendToAllTargets SendToAllAction;
        /// <summary>
        /// Action invoked when an ismuted message is requested to be sent to a target
        /// </summary>
        public SendMessageIsMutedToTarget SendMsgTo;
        /// <summary>
        /// Voice chat workflow
        /// </summary>
        public VoiceDataWorkflow Workflow;

        private BytePacket toSend;

        /// <summary>
        /// Processes the ismuted message received
        /// </summary>
        /// <param name="isSelfMuted">true if local slient has been muted by the sender</param>
        /// <param name="senderID">message sender id</param>
        public void ProcessNetworkIsMutedMessage(bool isSelfMuted, ulong senderID)
        {
            Workflow.ProcessIsMutedMessage(isSelfMuted, senderID);
        }
        /// <summary>
        /// Process the received packet data.
        /// </summary>
        /// <param name="receivedData">received raw data</param>
        /// <param name="startIndex">received raw data start index</param>
        /// <param name="length">received raw data length</param>
        /// <param name="netId">sender net id</param>
        public void ProcessNetworkReceivedPacket(byte[] receivedData, int startIndex, int length, ulong netId)
        {
            Workflow.ProcessReceivedPacket(receivedData, startIndex, length, netId);
        }
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
            toSend.CurrentSeek = 0;
            toSend.CurrentLength = 0;
            toSend.Write(info.NetId);
            toSend.Write(info.Frequency);
            toSend.Write(info.Channels);
            toSend.Write((byte)info.Format);

            int n = toSend.Copy(data);

            SendToAllAction?.Invoke(toSend.Data, 0, toSend.CurrentLength);
        }
        /// <summary>
        /// Sends a packet to the given target
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        public override void SendTo(BytePacket data, VoicePacketInfo info, ulong receiverID)
        {
            toSend.CurrentSeek = 0;
            toSend.CurrentLength = 0;
            toSend.Write(info.NetId);
            toSend.Write(info.Frequency);
            toSend.Write(info.Channels);
            toSend.Write((byte)info.Format);

            int n = toSend.Copy(data);

            SendToAction?.Invoke(toSend.Data, 0, toSend.CurrentLength, receiverID);
        }
        /// <summary>
        /// Sends a packet message to the target informing him whenever he has been muted/unmuted by the local client.
        /// </summary>
        /// <param name="receiverID">Receiver to which the packet should be sent</param>
        /// <param name="isReceiverMutedByLocal">True if receiver is muted and can avoid sending voice chat packets to the local client</param>
        public override void SendMessageIsMutedTo(ulong receiverID, bool isReceiverMutedByLocal)
        {
            SendMsgTo?.Invoke(receiverID, isReceiverMutedByLocal);
        }

        private void OnEnable()
        {
            toSend = new BytePacket(MaxDataLength);
        }
    }
}