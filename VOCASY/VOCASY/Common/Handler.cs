using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that handles voice input/output
    /// </summary>
    public class Handler : VoiceHandler
    {
        /// <summary>
        /// Default flag
        /// </summary>
        public const AudioDataTypeFlag SelfFlag = AudioDataTypeFlag.Both;
        /// <summary>
        /// Output source
        /// </summary>
        public VoiceReceiver Receiver;
        /// <summary>
        /// Input source
        /// </summary>
        public VoiceRecorder Recorder;
        /// <summary>
        /// The INetworkIdentity associated with this object
        /// </summary>
        public NetworkIdentity Identity = new NetworkIdentity();
        /// <summary>
        /// True if this handler is recording input
        /// </summary>
        public override bool IsRecorder { get { return Identity.IsLocalPlayer; } }
        /// <summary>
        /// Network ID associated with this hanlder
        /// </summary>
        public override ulong NetID { get { return Identity.NetworkId; } }
        /// <summary>
        /// True if VoiceHandler has initialized correctly
        /// </summary>
        public bool IsInitialized { get { return initialized; } }

        private bool initialized;

        /// <summary>
        /// Gets recorded data and stores it in format Single
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="micDataCount">amount of data to store</param>
        /// <param name="effectiveMicDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public override VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount)
        {
            effectiveMicDataCount = 0;
            //Gets mic data from recorder if not disabled
            if (!Recorder.IsEnabled)
                return VoicePacketInfo.InvalidPacket;

            return Recorder.GetMicData(buffer, bufferOffset, micDataCount, out effectiveMicDataCount);
        }
        /// <summary>
        /// Gets recorded data and stores it in format Int16
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="micDataCount">amount of data to store</param>
        /// <param name="effectiveMicDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public override VoicePacketInfo GetMicDataInt16(byte[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount)
        {
            effectiveMicDataCount = 0;
            //Gets mic data from recorder if not disabled
            if (!Recorder.IsEnabled)
                return VoicePacketInfo.InvalidPacket;

            return Recorder.GetMicData(buffer, bufferOffset, micDataCount, out effectiveMicDataCount);
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
            //Gives receiver the audio data for the output is not disabled
            if (Receiver.enabled)
                Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
        }
        /// <summary>
        /// Processes audio data in format Int16 and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public override void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            //Gives receiver the audio data for the output is not disabled
            if (Receiver.enabled)
                Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
        }
        /// <summary>
        /// Resets initialized status. Do not Reset an handler if it has not been disabled yet, otherwise it will remain in an incorrect state and lead to possible errors.
        /// </summary>
        public void Reset()
        {
            initialized = false;
        }

        private void PTTOffUpdate()
        {
            if (!Recorder.IsEnabled)
                Recorder.StartRecording();
        }
        private void PTTOnUpdate()
        {
            if (Workflow.Settings.IsPushToTalkOn())
            {
                //if ptt key is pressed and recorder is not recording start recording
                if (!Recorder.IsEnabled)
                    Recorder.StartRecording();
            }
            else if (Recorder.IsEnabled) //if ptt key is not pressed and recorder is recording stop recording
                Recorder.StopRecording();
        }
        private void OnVoiceChatEnabledChanged()
        {
            if (initialized)
                this.enabled = Workflow.Settings.VoiceChatEnabled;
        }
        private void InitUpdate()
        {
            if (Identity != null && Identity.IsInitialized)
            {
                initialized = true;

                if (Receiver == null || Recorder == null)
                    throw new NullReferenceException("VoiceHandler requeires 2 valid components VoiceReceiver and VoiceRecorder");

                //Compatibility check between self, receiver and recorder
                AudioDataTypeFlag res = Receiver.AvailableTypes & Recorder.AvailableTypes & SelfFlag;

                if (res == AudioDataTypeFlag.None)
                    throw new ArgumentException("The voice handler underlying receiver and recorder components are incompatible");

                //Set the compatibility value of this handler
                AvailableTypes = res;

                OnEnable();

                OnVoiceChatEnabledChanged();
            }
        }
        private void NormalUpdate()
        {
            //If it is not a recorder update output volume
            if (!IsRecorder)
            {
                Receiver.Volume = OutputVolume;
                Workflow.IsHandlerMuted(this);
                return;
            }

            //custom ptt update
            if (Workflow.Settings.PushToTalk)
                PTTOnUpdate();
            else
                PTTOffUpdate();

            //if the are mic data recorded available inform the workflow
            if (Recorder.IsEnabled && Recorder.MicDataAvailable > 0)
                Workflow.ProcessMicData(this);
        }
        private void Update()
        {
            if (initialized)
            {
                NormalUpdate();
            }
            else
            {
                InitUpdate();
            }
        }
        private void OnEnable()
        {
            if (initialized)
            {
                //If this is a recorder && ptt is off start recording
                if (IsRecorder)
                    Recorder.enabled = IsRecorder;

                //Set receiver enable status
                Receiver.enabled = !IsRecorder;
                //Add self to the workflow
                Workflow.AddVoiceHandler(this);
            }
        }
        private void OnDisable()
        {
            if (initialized)
            {
                //If this is a recorder stop recording
                if (IsRecorder)
                    Recorder.StopRecording();

                Recorder.enabled = false;
                //Make sure to disables receiver
                Receiver.enabled = false;
                //Removes self from the workflow
                Workflow.RemoveVoiceHandler(this);
            }
        }
        private void Awake()
        {
            initialized = false;

            Recorder.enabled = false;
            Receiver.enabled = false;

            Workflow.Settings.VoiceChatEnabledChanged += OnVoiceChatEnabledChanged;
        }
        private void OnDestroy()
        {
            Workflow.Settings.VoiceChatEnabledChanged -= OnVoiceChatEnabledChanged;
        }
    }
}