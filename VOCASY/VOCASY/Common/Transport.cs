using UnityEngine;
using GENUtility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that fakes network state between self and another client. It should be used for tests and debug
    /// </summary>
    [CreateAssetMenu(fileName = "Transport", menuName = "VOCASY/DataTransports/Transport")]
    public class Transport : VoiceDataTransport
    {
        /// <summary>
        /// Delegate used when data is requested to be sent to a specific target
        /// </summary>
        public delegate void SendToTarget(byte[] data, int startIndex, int length, ulong target);
        /// <summary>
        /// delegate used when data is requested to be sent to all
        /// </summary>
        public delegate void SendToAllTargets(byte[] data, int startIndex, int length);
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

        private BytePacket toSend;

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
        /// Performs a normal SendToAllOthers to the given id
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
        private void OnEnable()
        {
            toSend = new BytePacket(MaxDataLength);
        }
    }
}