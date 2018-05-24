using UnityEngine;
using GENUtility;
using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that records mic data using Unity API
    /// </summary>
    public class Recorder : VoiceRecorder
    {
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public override AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Both; } }
        /// <summary>
        /// Is this input disabled?
        /// </summary>
        public override bool IsEnabled { get { return Internal_isEnabled; } }
        /// <summary>
        /// Amount of mic data recorded currently available
        /// </summary>
        public override int MicDataAvailable { get { return Internal_readIndex <= Internal_writeIndex ? Internal_writeIndex - Internal_readIndex : (Internal_cyclicAudioBuffer.Length - Internal_readIndex) + Internal_writeIndex; } }
        /// <summary>
        /// Voice chat settings
        /// </summary>
        public VoiceChatSettings Settings;

        /// <summary>
        /// Exposed for tests. True if recorder is currently recording
        /// </summary>
        [NonSerialized]
        public bool Internal_isEnabled;

        /// <summary>
        /// Exposed for tests. Current device min frequency
        /// </summary>
        [NonSerialized]
        public int Internal_minDevFrequency;
        /// <summary>
        /// Exposed for tests. Current device max frequency
        /// </summary>
        [NonSerialized]
        public int Internal_maxDevFrequency;

        /// <summary>
        /// Exposed fot tests. Internal clip created by UnityEngine API holding recorded audio data
        /// </summary>
        [NonSerialized]
        public AudioClip Internal_clip;

        /// <summary>
        /// Exposed for tests. Offset used to keep tracking of last clip buffer index
        /// </summary>
        [NonSerialized]
        public int Internal_prevOffset;

        /// <summary>
        /// Exposed for tests. Internal cyclic audio buffer which holds audio data received from clip
        /// </summary>
        [NonSerialized]
        public float[] Internal_cyclicAudioBuffer;
        /// <summary>
        /// Exposed for tests. Internal cyclic buffer current read index
        /// </summary>
        [NonSerialized]
        public int Internal_readIndex;
        /// <summary>
        /// Exposed for tests. Internal cyclic buffer current write index
        /// </summary>
        [NonSerialized]
        public int Internal_writeIndex;

        /// <summary>
        /// Exposed for tests. Gets new audio data from clip
        /// </summary>
        public void Update()
        {
            if (!Internal_isEnabled)
                return;
            int offset = Microphone.GetPosition(Settings.MicrophoneDevice);
            if (Internal_prevOffset != offset)
            {
                int count = Internal_prevOffset < offset ? offset - Internal_prevOffset : ((Internal_clip.samples * Internal_clip.channels) - Internal_prevOffset) + offset;

                Internal_clip.GetData(Internal_cyclicAudioBuffer, 0);

                Internal_writeIndex += count;
                if (Internal_writeIndex >= Internal_cyclicAudioBuffer.Length)
                    Internal_writeIndex -= Internal_cyclicAudioBuffer.Length;

                Internal_prevOffset = offset;
            }
        }
        /// <summary>
        /// Gets recorded data and stores it in format Single
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="dataCount">amount of data to store</param>
        /// <param name="effectiveDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public override VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int dataCount, out int effectiveDataCount)
        {
            effectiveDataCount = Mathf.Min(Mathf.Min(dataCount, buffer.Length - bufferOffset), MicDataAvailable);
            if (effectiveDataCount <= 0)
                return VoicePacketInfo.InvalidPacket;

            ByteManipulator.WriteFromCycle(this.Internal_cyclicAudioBuffer, Internal_readIndex, buffer, bufferOffset, dataCount, out Internal_readIndex);

            return new VoicePacketInfo(0, (ushort)Internal_clip.frequency, (byte)Internal_clip.channels, AudioDataTypeFlag.Single);
        }
        /// <summary>
        /// Gets recorded data and stores it in format Int16
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="dataCount">amount of data to store</param>
        /// <param name="effectiveDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public override VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int dataCount, out int effectiveDataCount)
        {
            effectiveDataCount = Mathf.Min(Mathf.Min(dataCount, buffer.Length - bufferOffset), MicDataAvailable * sizeof(short));
            if ((effectiveDataCount & 1) == 1)
                effectiveDataCount--;

            if (effectiveDataCount <= 0)
                return VoicePacketInfo.InvalidPacket;

            int l = effectiveDataCount + bufferOffset;

            for (int i = bufferOffset; i < l; i += sizeof(short))
            {
                ByteManipulator.Write(buffer, i, (short)Mathf.Lerp(short.MinValue, short.MaxValue, Internal_cyclicAudioBuffer[Internal_readIndex]));

                Internal_readIndex++;
                if (Internal_readIndex >= Internal_cyclicAudioBuffer.Length)
                    Internal_readIndex = 0;
            }

            return new VoicePacketInfo(0, (ushort)Internal_clip.frequency, (byte)Internal_clip.channels, AudioDataTypeFlag.Int16);
        }
        /// <summary>
        /// Stops recording
        /// </summary>
        public override void StopRecording()
        {
            Microphone.End(Settings.MicrophoneDevice);

            if (Internal_clip != null)
                Destroy(Internal_clip);

            Internal_clip = null;

            Internal_isEnabled = false;
        }
        /// <summary>
        /// Starts recording
        /// </summary>
        public override void StartRecording()
        {
            int freq = (ushort)Mathf.Clamp((int)Settings.AudioQuality, VoiceChatSettings.MinFrequency, Mathf.Min(Internal_maxDevFrequency, VoiceChatSettings.MaxFrequency));

            Internal_clip = Microphone.Start(Settings.MicrophoneDevice, true, 1, freq);

            if (Internal_cyclicAudioBuffer == null)
                Internal_cyclicAudioBuffer = new float[VoiceChatSettings.MaxFrequency / 10];

            Internal_readIndex = 0;
            Internal_writeIndex = 0;

            Internal_isEnabled = true;
        }
        /// <summary>
        /// Exposed for tests. Internal method invoked in response to settings frequency changed
        /// </summary>
        /// <param name="prevFrequency">prev frequency</param>
        public void Internal_OnFrequencyChanged(FrequencyType prevFrequency)
        {
            //if it was recording restart with new frequency
            if (Internal_isEnabled)
            {
                StopRecording();
                StartRecording();
            }
        }
        /// <summary>
        /// Exposed for tests. Internal method invoked in response to settings device changed
        /// </summary>
        /// <param name="prevMicDevice">prev mic device</param>
        public void Internal_OnMicDeviceChanged(string prevMicDevice)
        {
            bool wasRecording = Internal_isEnabled;

            //stop recording if it was rec previously
            if (wasRecording)
                StopRecording();

            //update curren frequency limits
            Microphone.GetDeviceCaps(Settings.MicrophoneDevice, out Internal_minDevFrequency, out Internal_maxDevFrequency);

            //restart recording if it was rec previously
            if (wasRecording)
                StartRecording();
        }
        /// <summary>
        /// Exposed for tests. Initializes recorder state and settings event responses
        /// </summary>
        public void Awake()
        {
            Internal_isEnabled = false;

            Settings.MicrophoneDeviceChanged += Internal_OnMicDeviceChanged;
            Settings.AudioQualityChanged += Internal_OnFrequencyChanged;

            Internal_OnMicDeviceChanged(null);
            Internal_OnFrequencyChanged(Settings.AudioQuality);
        }
        /// <summary>
        /// Exposed for tests. Removes settings event responses
        /// </summary>
        public void OnDestroy()
        {
            Settings.MicrophoneDeviceChanged -= Internal_OnMicDeviceChanged;
            Settings.AudioQualityChanged -= Internal_OnFrequencyChanged;
        }
    }
}