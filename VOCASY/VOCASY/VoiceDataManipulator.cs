using UnityEngine;
using GENUtility;
namespace VOCASY
{
    /// <summary>
    /// Class that converts audio data formats to game packet.
    /// </summary>
    public abstract class VoiceDataManipulator : ScriptableObject
    {
        /// <summary>
        /// Audio data formats that this class can process
        /// </summary>
        public abstract AudioDataTypeFlag AvailableTypes { get; }
        /// <summary>
        /// Processes audio data in format Single into a GamePacket
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data startIndex</param>
        /// <param name="audioDataCount">number of bytes to process</param>
        /// <param name="info">data info</param>
        /// <param name="output">gamepacket on which data will be written</param>
        public abstract void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output);
        /// <summary>
        /// Processes audio data in format Int16 into a GamePacket
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data startIndex</param>
        /// <param name="audioDataCount">number of bytes to process</param>
        /// <param name="info">data info</param>
        /// <param name="output">gamepacket on which data will be written</param>
        public abstract void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output);
        /// <summary>
        /// Processes a Gamepacket into audio data in format Single
        /// </summary>
        /// <param name="packet">GamePacket to process</param>
        /// <param name="info">data info</param>
        /// <param name="out_audioData">output array on which data will be written</param>
        /// <param name="out_audioDataOffset">output array start index</param>
        /// <returns>total number of floats written</returns>
        public abstract int FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset);
        /// <summary>
        /// Processes a Gamepacket into audio data in format Int16
        /// </summary>
        /// <param name="packet">GamePacket to process</param>
        /// <param name="info">data info</param>
        /// <param name="out_audioData">output array on which data will be written</param>
        /// <param name="out_audioDataOffset">output array start index</param>
        /// <returns>total number of bytes written</returns>
        public abstract int FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset);
    }
}