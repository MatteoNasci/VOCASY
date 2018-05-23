using UnityEngine;
namespace VOCASY.Common
{
    /// <summary>
    /// A class that manages audio output
    /// </summary>
    public abstract class VoiceReceiver : MonoBehaviour
    {
        /// <summary>
        /// Volume specific for this output source
        /// </summary>
        public float Volume;
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public abstract AudioDataTypeFlag AvailableTypes { get; }
        /// <summary>
        /// Processes audio data in format Single and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public abstract void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
        /// <summary>
        /// Processes audio data in format Int16 and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public abstract void ReceiveAudioData(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
    }
}