﻿using UnityEngine;
using VOCASY.Utility;
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
        public override bool IsEnabled { get { return isEnabled; } }
        /// <summary>
        /// Amount of mic data recorded currently available
        /// </summary>
        public override int MicDataAvailable { get { return readIndex <= writeIndex ? writeIndex - readIndex : (cyclicBuffer.Length - readIndex) + writeIndex; } }
        /// <summary>
        /// Voice chat settings
        /// </summary>
        public VoiceChatSettings Settings;

        private bool isEnabled;

        private int minDevFrequency;
        private int maxDevFrequency;

        private AudioClip clip;

        private int prevOffset;

        private float[] cyclicBuffer;
        private int readIndex;
        private int writeIndex;

        void Update()
        {
            if (!isEnabled)
                return;
            int offset = Microphone.GetPosition(Settings.MicrophoneDevice);
            if (prevOffset != offset)
            {
                int count = prevOffset < offset ? offset - prevOffset : ((clip.samples * clip.channels) - prevOffset) + offset;

                clip.GetData(cyclicBuffer, 0);

                writeIndex += count;
                if (writeIndex >= cyclicBuffer.Length)
                    writeIndex -= cyclicBuffer.Length;

                prevOffset = offset;
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

            Utils.WriteFromCycle(this.cyclicBuffer, readIndex, buffer, bufferOffset, dataCount, out readIndex);

            return new VoicePacketInfo(0, (ushort)clip.frequency, (byte)clip.channels, AudioDataTypeFlag.Single);
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
                Utils.Write(buffer, i, (short)Mathf.Lerp(short.MinValue, short.MaxValue, cyclicBuffer[readIndex]));

                readIndex++;
                if (readIndex >= cyclicBuffer.Length)
                    readIndex = 0;
            }

            return new VoicePacketInfo(0, (ushort)clip.frequency, (byte)clip.channels, AudioDataTypeFlag.Int16);
        }
        /// <summary>
        /// Stops recording
        /// </summary>
        public override void StopRecording()
        {
            Microphone.End(Settings.MicrophoneDevice);

            if (clip != null)
                Destroy(clip);

            clip = null;

            isEnabled = false;
        }
        /// <summary>
        /// Starts recording
        /// </summary>
        public override void StartRecording()
        {
            int freq = (ushort)Mathf.Clamp((int)Settings.AudioQuality, VoiceChatSettings.MinFrequency, Mathf.Min(maxDevFrequency, VoiceChatSettings.MaxFrequency));

            clip = Microphone.Start(Settings.MicrophoneDevice, true, 1, freq);

            if (cyclicBuffer == null)
                cyclicBuffer = new float[VoiceChatSettings.MaxFrequency / 10];

            readIndex = 0;
            writeIndex = 0;

            isEnabled = true;
        }

        void OnFrequencyChanged(FrequencyType prevFrequency)
        {
            //if it was recording restart with new frequency
            if (isEnabled)
            {
                StopRecording();
                StartRecording();
            }
        }
        void OnMicDeviceChanged(string prevMicDevice)
        {
            bool wasRecording = isEnabled;

            //stop recording if it was rec previously
            if (wasRecording)
                StopRecording();

            //update curren frequency limits
            Microphone.GetDeviceCaps(Settings.MicrophoneDevice, out minDevFrequency, out maxDevFrequency);

            //restart recording if it was rec previously
            if (wasRecording)
                StartRecording();
        }
        void Awake()
        {
            isEnabled = false;

            Settings.MicrophoneDeviceChanged += OnMicDeviceChanged;
            Settings.AudioQualityChanged += OnFrequencyChanged;

            OnMicDeviceChanged(null);
            OnFrequencyChanged(Settings.AudioQuality);
        }
        void OnDestroy()
        {
            Settings.MicrophoneDeviceChanged -= OnMicDeviceChanged;
            Settings.AudioQualityChanged -= OnFrequencyChanged;
        }
    }
}