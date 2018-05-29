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
        /// Value used to convert byte data length to float data length
        /// </summary>
        public const float ConverterByteSingle = 0.25000001f;
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
                audioDataCount = (int)((outputAvailableSpace - sizeof(int)) * ConverterByteSingle);

            if (audioDataCount <= 0)
            {
                info.ValidPacketInfo = false;
                return;
            }

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

            if (audioDataCount <= 0)
            {
                info.ValidPacketInfo = false;
                return;
            }

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
        /// <returns>total number of floats written</returns>
        public override int FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset)
        {
            int maxP = packet.CurrentLength - packet.CurrentSeek - sizeof(int);
            int dataCount = Mathf.Min(Mathf.Min(packet.ReadInt(), out_audioData.Length - out_audioDataOffset), maxP);

            if (dataCount <= 0)
            {
                info.ValidPacketInfo = false;
                return dataCount;
            }

            int length = dataCount + out_audioDataOffset;
            for (int i = out_audioDataOffset; i < length; i++)
            {
                out_audioData[i] = packet.ReadFloat();
            }

            return dataCount;
        }
        /// <summary>
        /// Processes a Gamepacket into audio data in format Int16
        /// </summary>
        /// <param name="packet">GamePacket to process</param>
        /// <param name="info">data info</param>
        /// <param name="out_audioData">output array on which data will be written</param>
        /// <param name="out_audioDataOffset">output array start index</param>
        /// <returns>total number of bytes written</returns>
        public override int FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset)
        {
            int maxP = packet.CurrentLength - packet.CurrentSeek - sizeof(int);
            int dataCount = Mathf.Min(Mathf.Min(packet.ReadInt(), out_audioData.Length - out_audioDataOffset), maxP);

            if (dataCount <= 0)
            {
                info.ValidPacketInfo = false;
                return dataCount;
            }

            packet.ReadByteData(out_audioData, out_audioDataOffset, dataCount);

            return dataCount;
        }
    }
}