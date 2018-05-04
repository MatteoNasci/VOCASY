using VOCASY.Utility;
namespace VOCASY.Common.Unity
{
    /// <summary>
    /// Class that converts audio data formats to game packet. Performs no other actions
    /// </summary>
    public class VoidManipulator : IAudioDataManipulator
    {
        /// <summary>
        /// Audio data formats that this class can process
        /// </summary>
        public AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Both; } }
        /// <summary>
        /// Processes audio data in format Single into a GamePacket
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data startIndex</param>
        /// <param name="audioDataCount">number of bytes to process</param>
        /// <param name="info">data info</param>
        /// <param name="output">gamepacket on which data will be written</param>
        public void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output)
        {
            output.Write((int)(audioDataCount * 0.25000001f));
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
        public void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output)
        {
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
        public void FromPacketToAudioData(GamePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount)
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
        public void FromPacketToAudioDataInt16(GamePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount)
        {
            dataCount = packet.ReadInt();
            packet.ReadByteData(out_audioData, out_audioDataOffset, dataCount);
        }
    }
}