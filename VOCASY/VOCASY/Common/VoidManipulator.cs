using UnityEngine;
using GENUtility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that converts audio data formats to game packet. Performs no other actions and no compression, it should be used only for tests and debug
    /// </summary>
    [CreateAssetMenu(fileName = "VoidManipulator", menuName = "VOCASY/DataManipulators/Void")]
    public class VoidManipulator : VoiceDataManipulator
    {
        /// <summary>
        /// Audio data formats that this class can process
        /// </summary>
        public override AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Both; } }
        /// <summary>
        /// Processes audio data in format Single into a GamePacket
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data startIndex</param>
        /// <param name="audioDataCount">number of bytes to process</param>
        /// <param name="info">data info</param>
        /// <param name="output">gamepacket on which data will be written</param>
        public override void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
        {
            int outputAvailableSpace = output.Data.Length - output.CurrentSeek;
            if (outputAvailableSpace < (audioDataCount * sizeof(float)) + sizeof(int))
                audioDataCount = (outputAvailableSpace / sizeof(float)) - sizeof(int);

            output.Write(audioDataCount);
            int length = audioDataCount + audioDataOffset;
            for (int i = audioDataOffset; i < length; i++)
            {
                output.Write(audioData[i]);
            }
        }
        /// <summary>
        /// Processes audio data in format Int16 into a GamePacket
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data startIndex</param>
        /// <param name="audioDataCount">number of bytes to process</param>
        /// <param name="info">data info</param>
        /// <param name="output">gamepacket on which data will be written</param>
        public override void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
        {
            int outputAvailableSpace = output.Data.Length - output.CurrentSeek;
            if (outputAvailableSpace < audioDataCount + sizeof(int))
                audioDataCount = outputAvailableSpace - sizeof(int);

            output.Write(audioDataCount);
            output.WriteByteData(audioData, audioDataOffset, audioDataCount);
        }
        /// <summary>
        /// Processes a Gamepacket into audio data in format Single
        /// </summary>
        /// <param name="packet">GamePacket to process</param>
        /// <param name="info">data info</param>
        /// <param name="out_audioData">output array on which data will be written</param>
        /// <param name="out_audioDataOffset">output array start index</param>
        /// <param name="dataCount">total number of bytes written</param>
        public override void FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount)
        {
            dataCount = packet.ReadInt();
            int length = dataCount + out_audioDataOffset;
            for (int i = out_audioDataOffset; i < length; i++)
            {
                out_audioData[i] = packet.ReadFloat();
            }
        }
        /// <summary>
        /// Processes a Gamepacket into audio data in format Int16
        /// </summary>
        /// <param name="packet">GamePacket to process</param>
        /// <param name="info">data info</param>
        /// <param name="out_audioData">output array on which data will be written</param>
        /// <param name="out_audioDataOffset">output array start index</param>
        /// <param name="dataCount">total number of bytes written</param>
        public override void FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount)
        {
            dataCount = packet.ReadInt();
            packet.ReadByteData(out_audioData, out_audioDataOffset, dataCount);
        }
    }
}