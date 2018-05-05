using System;
using UnityEngine;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that handles voice input/output
    /// </summary>
    [RequireComponent(typeof(INetworkIdentity), typeof(IVoiceReceiver), typeof(IVoiceRecorder))]
    public class VoiceHandler : MonoBehaviour, IVoiceHandler
    {
        private const AudioDataTypeFlag SelfFlag = AudioDataTypeFlag.Both;
        /// <summary>
        /// True if this handler is recording input
        /// </summary>
        public bool IsRecorder { get { return Identity.IsLocalPlayer; } }
        /// <summary>
        /// The INetworkIdentity associated with this object
        /// </summary>
        public INetworkIdentity Identity { get; private set; }
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public AudioDataTypeFlag AvailableTypes { get; private set; }
        /// <summary>
        /// Mute condition specific for this output source
        /// </summary>
        public bool IsSelfOutputMuted { get; set; }
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
        public float OutputVolume { get { return SelfOutputVolume * VoiceDataWorkflow.Settings.VoiceChatVolume; } }
        /// <summary>
        /// Output source
        /// </summary>
        protected IVoiceReceiver Receiver { get; private set; }
        /// <summary>
        /// Input source
        /// </summary>
        protected IVoiceRecorder Recorder { get; private set; }

        private float selfOutputVolume = 1f;

        private Action<IVoiceHandler> onMicDataProcessed;

        private Action updatePtt;

        private bool initialized = false;

        /// <summary>
        /// Sets an action to be called whenever there is mic data available
        /// </summary>
        /// <param name="onMicDataProcessed">action called on mic data ready</param>
        public void SetOnMicDataProcessed(Action<IVoiceHandler> onMicDataProcessed)
        {
            //Sets action to be called on mic data available
            this.onMicDataProcessed = onMicDataProcessed;
        }
        /// <summary>
        /// Gets recorded data and stores it in format Single
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="micDataCount">amount of data to store</param>
        /// <param name="effectiveMicDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount)
        {
            effectiveMicDataCount = 0;
            //Gets mic data from recorder if not disabled
            if (Recorder.IsDisabled)
                return VoicePacketInfo.InvalidPacket;

            VoicePacketInfo info = Recorder.GetMicData(buffer, bufferOffset, micDataCount, out effectiveMicDataCount);
            info.NetId = Identity.NetworkId;

            return info;
        }
        /// <summary>
        /// Gets recorded data and stores it in format Int16
        /// </summary>
        /// <param name="buffer">buffer to fill with audio data recorded</param>
        /// <param name="bufferOffset">buffer start index</param>
        /// <param name="micDataCount">amount of data to store</param>
        /// <param name="effectiveMicDataCount">effective amount of data stored</param>
        /// <returns>data info</returns>
        public VoicePacketInfo GetMicDataInt16(byte[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount)
        {
            effectiveMicDataCount = 0;
            //Gets mic data from recorder if not disabled
            if (Recorder.IsDisabled)
                return VoicePacketInfo.InvalidPacket;

            VoicePacketInfo info = Recorder.GetMicData(buffer, bufferOffset, micDataCount, out effectiveMicDataCount);
            info.NetId = Identity.NetworkId;

            return info;
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
            //Gives receiver the audio data for the output is not disabled
            if (!Receiver.IsDisabled)
                Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
        }
        /// <summary>
        /// Processes audio data in format Int16 and plays it
        /// </summary>
        /// <param name="audioData">audio data to process</param>
        /// <param name="audioDataOffset">audio data start index</param>
        /// <param name="audioDataCount">audio data amount to process</param>
        /// <param name="info">data info</param>
        public void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            //Gives receiver the audio data for the output is not disabled
            if (!Receiver.IsDisabled)
                Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
        }

        void Start()
        {
            if (!initialized)
            {
                initialized = true;
                OnEnable();
            }
        }
        void Update()
        {
            //If it is not a recorder update output volume
            if (!IsRecorder)
            {
                Receiver.Volume = OutputVolume;
                return;
            }

            //custom ptt update
            updatePtt.Invoke();

            //if the are mic data recorded available and there is an action setted call it
            if (!Recorder.IsDisabled && Recorder.MicDataAvailable > 0 && onMicDataProcessed != null)
                onMicDataProcessed.Invoke(this);
        }
        void OnEnable()
        {
            if (initialized)
            {
                //If this is a recorder && ptt is off start recording
                if (IsRecorder)
                {
                    VoiceDataWorkflow.Settings.PushToTalkChanged += OnPushToTalkChanged;
                    OnPushToTalkChanged();
                }
                //Set receiver enable status
                Receiver.Enable(!IsRecorder);
                //Add self to the workflow
                VoiceDataWorkflow.AddVoiceHandler(this);
            }
        }
        void OnDisable()
        {
            if (initialized)
            {
                //If this is a recorder stop recording
                if (IsRecorder)
                {
                    VoiceDataWorkflow.Settings.PushToTalkChanged -= OnPushToTalkChanged;
                    Recorder.StopRecording();
                }
                //Make sure to disables receiver
                Receiver.Enable(false);
                //Removes self from the workflow
                VoiceDataWorkflow.RemoveVoiceHandler(this);
            }
        }
        void Awake()
        {
            initialized = false;
            //Get all required components
            Identity = GetComponent<INetworkIdentity>();
            Receiver = GetComponent<IVoiceReceiver>();
            Recorder = GetComponent<IVoiceRecorder>();

            if (Identity == null || Receiver == null || Recorder == null)
                throw new NullReferenceException("VoiceHandler requeires 3 valid component interfaces INetworkIdentity, IVoiceReceiver and IVoiceRecorder");

            //Compatibility check between self, receiver and recorder
            AudioDataTypeFlag res = Receiver.AvailableTypes & Recorder.AvailableTypes & SelfFlag;

            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("The given handler type is incompatible with its underlying receiver and recorder components");

            //Set the compatibility value of this handler
            AvailableTypes = res;

            VoiceDataWorkflow.Settings.VoiceChatEnabledChanged += OnVoiceChatEnabledChanged;

            OnVoiceChatEnabledChanged();
        }
        void OnDestroy()
        {
            VoiceDataWorkflow.Settings.VoiceChatEnabledChanged -= OnVoiceChatEnabledChanged;
        }

        void PTTOffUpdate()
        {
            if (Recorder.IsDisabled)
                Recorder.StartRecording();
        }
        void PTTOnUpdate()
        {
            if (Input.GetKey((KeyCode)VoiceDataWorkflow.Settings.PushToTalkKey))
            {
                //if ptt key is pressed and recorder is not recording start recording
                if (Recorder.IsDisabled)
                    Recorder.StartRecording();
            }
            else if (!Recorder.IsDisabled) //if ptt key is not pressed and recorder is recording stop recording
                Recorder.StopRecording();
        }
        void OnPushToTalkChanged()
        {
            if (VoiceDataWorkflow.Settings.PushToTalk)
                updatePtt = PTTOnUpdate;
            else
                updatePtt = PTTOffUpdate;
        }
        void OnVoiceChatEnabledChanged()
        {
            this.enabled = VoiceDataWorkflow.Settings.VoiceChatEnabled;
        }
    }
}