using UnityEngine;
namespace VOCASY
{
    /// <summary>
    /// Class that handles voice input/output
    /// </summary>
    public abstract class VoiceHandler : MonoBehaviour
    {
        /// <summary>
        /// Workflow used by this handler
        /// </summary>
        public VoiceDataWorkflow Workflow;
        /// <summary>
        /// Mute condition specific for this output source
        /// </summary>
        public bool IsSelfOutputMuted;
        /// <summary>
        /// True if this handler is recording input
        /// </summary>
        public abstract bool IsRecorder { get; }
        /// <summary>
        /// Network ID associated with this hanlder
        /// </summary>
        public abstract ulong NetID { get; }
        /// <summary>
        /// Is output source muted?
        /// </summary>
        public bool IsOutputMuted { get { return IsSelfOutputMuted || Mathf.Approximately(0f, OutputVolume); } }
        /// <summary>
        /// Volume specific for this output source
        /// </summary>
        public float SelfOutputVolume { get { return selfOutputVolume; } set { selfOutputVolume = Mathf.Clamp01(value); } }
        /// <summary>
        /// Effective volume of this output source
        /// </summary>
        public float OutputVolume { get { return SelfOutputVolume * Workflow.Settings.VoiceChatVolume; } }
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public AudioDataTypeFlag AvailableTypes { get; protected set; }

        private float selfOutputVolume = 1f;

        /// <summary>
        /// Gets recorded data and stores it in format Single
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="micDataCount">amount of data to store</param>
        /// <param name="effectiveMicDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public abstract VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount);
        /// <summary>
        /// Gets recorded data and stores it in format Int16
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="micDataCount">amount of data to store</param>
        /// <param name="effectiveMicDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public abstract VoicePacketInfo GetMicDataInt16(byte[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount);
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
        public abstract void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
    }
}