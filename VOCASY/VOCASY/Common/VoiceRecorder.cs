using UnityEngine;
namespace VOCASY.Common
{
    /// <summary>
    /// A class that manages audio input
    /// </summary>
    public abstract class VoiceRecorder : MonoBehaviour
    {
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public abstract AudioDataTypeFlag AvailableTypes { get; }
        /// <summary>
        /// Is this input recording?
        /// </summary>
        public abstract bool IsEnabled { get; }
        /// <summary>
        /// Amount of mic data recorded currently available
        /// </summary>
        public abstract int MicDataAvailable { get; }
        /// <summary>
        /// Gets recorded data and stores it in format Single
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="maxDataCount">max amount of data to store</param>
        /// <param name="effectiveDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public abstract VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int maxDataCount, out int effectiveDataCount);
        /// <summary>
        /// Gets recorded data and stores it in format Int16
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="maxDataCount">max amount of data to store</param>
        /// <param name="effectiveDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public abstract VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int maxDataCount, out int effectiveDataCount);
        /// <summary>
        /// Starts recording
        /// </summary>
        public abstract void StartRecording();
        /// <summary>
        /// Stops recording
        /// </summary>
        public abstract void StopRecording();
    }
}