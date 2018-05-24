using UnityEngine;
using GENUtility;
using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages voice audio output, compatible with all data formats and frequency/channels
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Receiver : VoiceReceiver
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
        /// Volume specific for this output source
        /// </summary>
        public override float Volume { get { return Internal_source.volume; } set { Internal_source.volume = value; } }
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public override AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Both; } }

        /// <summary>
        /// Exposed for tests. Internal audiosource used
        /// </summary>
        [NonSerialized]
        public AudioSource Internal_source;

        /// <summary>
        /// Exposed for tests. Internal cyclic audioBuffer used to store data and fetch them to the audiosource
        /// </summary>
        [NonSerialized]
        public float[] Internal_cyclicAudioBuffer;
        /// <summary>
        /// Exposed for tests. Internal index that indicates the current cyclic buffer read index
        /// </summary>
        [NonSerialized]
        public int Internal_readIndex;
        /// <summary>
        /// Exposed for tests. Internal index that indicates the current cyclic buffer write index
        /// </summary>
        [NonSerialized]
        public int Internal_writeIndex;

        /// <summary>
        /// Processes audio data in format Int16 and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public override void ReceiveAudioData(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            if (Internal_cyclicAudioBuffer == null)
                Internal_cyclicAudioBuffer = new float[VoiceChatSettings.MaxFrequency / 4];

            int length = audioDataCount / sizeof(short);

            //operations to convert the given audio data stored at tot frequency and tot channels into audio data with Frequency and Channels compatible with output source, inserting results into internal cyclic buffer
            float frequencyPerc = OutputBaseFrequencyInverse * info.Frequency;
            float channelsPerc = OutputBaseChannels / info.Channels;

            int bufferLength = Internal_cyclicAudioBuffer.Length;
            float index = Internal_writeIndex;
            float v = 0f;
            int prevDtReadIndex = int.MinValue;
            for (float i = 0; i < length; i += frequencyPerc)
            {
                //Converts given Int16 format data into Single format data. If the given data has already been read and converted previously use directly the cached value
                int idx = audioDataOffset + ((int)i * sizeof(short));
                if (idx != prevDtReadIndex)
                {
                    v = Mathf.InverseLerp(short.MinValue, short.MaxValue, ByteManipulator.ReadInt16(audioData, idx));
                    prevDtReadIndex = idx;
                }

                Internal_cyclicAudioBuffer[(int)index] = v;

                index += channelsPerc;
                if (index >= bufferLength)
                    index -= bufferLength;
            }
            Internal_writeIndex = (int)index;
        }
        /// <summary>
        /// Processes audio data in format Single and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public override void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            if (Internal_cyclicAudioBuffer == null)
                Internal_cyclicAudioBuffer = new float[VoiceChatSettings.MaxFrequency / 4];

            //if given audio data is already configured the same as the output source copy elements directly into internal cyclic buffer
            if (info.Frequency == OutputBaseFrequency && info.Channels == OutputBaseChannels)
            {
                ByteManipulator.WriteToCycle(audioData, audioDataOffset, Internal_cyclicAudioBuffer, Internal_writeIndex, audioDataCount, out Internal_writeIndex);
                return;
            }

            //operations to convert the given audio data stored at tot frequency and tot channels into audio data with Frequency and Channels compatible with output source, inserting results into internal cyclic buffer
            float frequencyPerc = OutputBaseFrequencyInverse * info.Frequency;
            float channelsPerc = OutputBaseChannels / info.Channels;

            int bufferLength = Internal_cyclicAudioBuffer.Length;
            float index = Internal_writeIndex;
            for (float i = 0; i < audioDataCount; i += frequencyPerc)
            {
                Internal_cyclicAudioBuffer[(int)index] = audioData[(int)i + audioDataOffset];

                index += channelsPerc;
                if (index >= bufferLength)
                    index -= bufferLength;
            }
            Internal_writeIndex = (int)index;
        }
        /// <summary>
        /// Exposed for tests. Audio data from internal buffer is written into the given array
        /// </summary>
        /// <param name="data">data to fill</param>
        /// <param name="channels">channels of data required</param>
        public void OnAudioFilterRead(float[] data, int channels)//this method fills the unity audiosource audio data with the stored data
        {
            if (Internal_cyclicAudioBuffer == null)
                return;

            //current total number of audio data stored
            int count = Internal_readIndex > Internal_writeIndex ? (Internal_cyclicAudioBuffer.Length - Internal_readIndex) + Internal_writeIndex : Internal_writeIndex - Internal_readIndex;
            //total number of elements to supply to the audiosource
            count = Mathf.Min(count, data.Length);

            if (count <= 0)
                return;

            //supply data to the audiosource
            ByteManipulator.WriteFromCycle(Internal_cyclicAudioBuffer, Internal_readIndex, data, 0, count, out Internal_readIndex);
        }
        /// <summary>
        /// Exposed for tests. Gets audiosource from components
        /// </summary>
        public void Awake()
        {
            Internal_source = GetComponent<AudioSource>();
        }
        /// <summary>
        /// Exposed for tests. Enables audiosource
        /// </summary>
        public void OnEnable()
        {
            Internal_source.enabled = true;
            Internal_source.Play();
        }
        /// <summary>
        /// Exposed for tests. Disables audiosource and resets internal indexes
        /// </summary>
        public void OnDisable()
        {
            //resets stored data
            Internal_readIndex = 0;
            Internal_writeIndex = 0;

            Internal_source.Stop();
            Internal_source.enabled = false;
        }
    }
}