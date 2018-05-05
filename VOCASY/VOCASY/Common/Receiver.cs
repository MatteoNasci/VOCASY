using UnityEngine;
using VOCASY.Utility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages voice audio output, compatible with all data formats and frequency/channels
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Receiver : MonoBehaviour, IVoiceReceiver
    {
        /// <summary>
        /// Output frequency
        /// </summary>
        public const ushort OutputBaseFrequency = 48000;
        /// <summary>
        /// Output inverse frequency
        /// </summary>
        public const float OutputBaseFrequencyInverse = 0.0000208333334f;
        /// <summary>
        /// Output channels
        /// </summary>
        public const byte OutputBaseChannels = 2;

        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Both; } }
        /// <summary>
        /// Volume specific for this output source
        /// </summary>
        public float Volume { get { return source.volume; } set { source.volume = value; } }
        /// <summary>
        /// Is this output source disabled?
        /// </summary>
        public bool IsDisabled { get { return !enabled; } }

        private AudioSource source;

        private float[] audioBuffer;
        private int readIndex;
        private int writeIndex;

        /// <summary>
        /// Updates the enable status of this output source
        /// </summary>
        /// <param name="isEnabled">enable value</param>
        public void Enable(bool isEnabled)
        {
            this.enabled = isEnabled;
        }
        /// <summary>
        /// Processes audio data in format Int16 and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public void ReceiveAudioData(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            if (audioBuffer == null)
                audioBuffer = new float[VoiceChatSettings.MaxFrequency / 4];

            int length = audioDataCount / sizeof(short);

            //operations to convert the given audio data stored at tot frequency and tot channels into audio data with Frequency and Channels compatible with output source, inserting results into internal cyclic buffer
            float frequencyPerc = OutputBaseFrequencyInverse * info.Frequency;
            float channelsPerc = OutputBaseChannels / info.Channels;

            int bufferLength = audioBuffer.Length;
            float index = writeIndex;
            float v = 0f;
            int prevDtReadIndex = int.MinValue;
            for (float i = 0; i < length; i += frequencyPerc)
            {
                //Converts given Int16 format data into Single format data. If the given data has already been read and converted previously use directly the cached value
                int idx = audioDataOffset + ((int)i * sizeof(short));
                if (idx != prevDtReadIndex)
                {
                    v = Mathf.InverseLerp(short.MinValue, short.MaxValue, Utils.ReadInt16(audioData, idx));
                    prevDtReadIndex = idx;
                }

                audioBuffer[(int)index] = v;

                index += channelsPerc;
                if (index >= bufferLength)
                    index -= bufferLength;
            }
            writeIndex = (int)index;
        }
        /// <summary>
        /// Processes audio data in format Single and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            if (audioBuffer == null)
                audioBuffer = new float[VoiceChatSettings.MaxFrequency / 4];

            //if given audio data is already configured the same as the output source copy elements directly into internal cyclic buffer
            if (info.Frequency == OutputBaseFrequency && info.Channels == OutputBaseChannels)
            {
                Utils.WriteToCycle(audioData, audioDataOffset, audioBuffer, writeIndex, audioDataCount, out writeIndex);
                return;
            }

            //operations to convert the given audio data stored at tot frequency and tot channels into audio data with Frequency and Channels compatible with output source, inserting results into internal cyclic buffer
            float frequencyPerc = OutputBaseFrequencyInverse * info.Frequency;
            float channelsPerc = OutputBaseChannels / info.Channels;

            int bufferLength = audioBuffer.Length;
            float index = writeIndex;
            for (float i = 0; i < audioDataCount; i += frequencyPerc)
            {
                audioBuffer[(int)index] = audioData[(int)i + audioDataOffset];

                index += channelsPerc;
                if (index >= bufferLength)
                    index -= bufferLength;
            }
            writeIndex = (int)index;
        }

        void OnAudioFilterRead(float[] data, int channels)//this method fills the unity audiosource audio data with the stored data
        {
            if (audioBuffer == null)
                return;

            //current total number of audio data stored
            int count = readIndex > writeIndex ? (audioBuffer.Length - readIndex) + writeIndex : writeIndex - readIndex;
            //total number of elements to supply to the audiosource
            count = Mathf.Min(count, data.Length);

            if (count == 0)
                return;

            //supply data to the audiosource
            Utils.WriteFromCycle(audioBuffer, readIndex, data, 0, count, out readIndex);
        }
        void Awake()
        {
            source = GetComponent<AudioSource>();
        }
        void OnEnable()
        {
            source.enabled = true;
            source.Play();
        }
        void OnDisable()
        {
            //resets stored data
            readIndex = 0;
            writeIndex = 0;

            source.Stop();
            source.enabled = false;
        }
    }
}