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
        private const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(bool);

        private const int pLength = 1024;
        /// <summary>
        /// True as long as there are packets available to receive
        /// </summary>
        public bool IsPacketAvailable { get { return packets.Count > 0; } }
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public int MaxPacketLength { get { return packetDataSize; } }
        /// <summary>
        /// To which id fake packets should be sent
        /// </summary>
        public ulong ReceiverId;

        private Queue<GamePacket> packets = new Queue<GamePacket>();

        [SerializeField]
        private int packetDataSize = pLength - FirstPacketByteAvailable;

        /// <summary>
        /// Receive packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <returns>data info</returns>
        public VoicePacketInfo Receive(GamePacket buffer)
        {
            //Debug.Log("Data packet received");
            GamePacket received = packets.Dequeue();

            VoicePacketInfo info = new VoicePacketInfo();
            info.NetId = received.ReadULong(0);
            info.Frequency = received.ReadUShort();
            info.Channels = received.ReadByte();
            info.Format = (AudioDataTypeFlag)received.ReadByte();
            info.ValidPacketInfo = true;

            int n;
            buffer.Copy(received, out n);

            received.DisposePacket();

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

            packets.Enqueue(toSend);
        }
        void Update()
        {
            //if (packets.Count != 0)
            //    Debug.Log(packets.Count);
            if (IsPacketAvailable && onPacketAvailable != null)
            {
                //Debug.Log("OnpacketAvailable");
                onPacketAvailable.Invoke();
            }
        }
        private Action onPacketAvailable;
        /// <summary>
        /// Sets an action to be called when a packet is available
        /// </summary>
        /// <param name="onPacketAvailable">Action called on packet available</param>
        public void SetOnPacketAvailable(Action onPacketAvailable)
        {
            this.onPacketAvailable = onPacketAvailable;
        }
    }
}