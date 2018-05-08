using System;
using VOCASY.Utility;
namespace VOCASY
{
    /// <summary>
    /// Interface that represents a class that transports packets
    /// </summary>
    public interface IAudioTransportLayer
    {
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        int MaxPacketLength { get; }
        /// <summary>
        /// Receive packet data
        /// </summary>
        /// <param name="buffer">GamePacket of which data will be stored</param>
        /// <param name="dataReceived">Raw data received from network</param>
        /// <param name="netId">Sender net id</param>
        /// <returns>data info</returns>
        VoicePacketInfo Receive(GamePacket buffer, GamePacket dataReceived, ulong netId);
        /// <summary>
        /// Sends a packet to all the other clients that need it
        /// </summary>
        /// <param name="data">GamePacket that stores the data to send</param>
        /// <param name="info">data info</param>
        void SendToAllOthers(GamePacket data, VoicePacketInfo info);
    }
}