using System;
using UnityEngine;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that handles voice input/output
    /// </summary>
    public class VoiceHandler : MonoBehaviour
    {
        /// <summary>
        /// Default flag
        /// </summary>
        public const AudioDataTypeFlag SelfFlag = AudioDataTypeFlag.Both;
        /// <summary>
        /// Voice chat workflow
        /// </summary>
        public VoiceDataWorkflow Workflow;
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
        /// Mute condition specific for this output source
        /// </summary>
        public bool IsSelfOutputMuted;
        /// <summary>
        /// True if this handler is recording input
        /// </summary>
        public bool IsRecorder { get { return Identity.IsLocalPlayer; } }
        /// <summary>
        /// Is output source muted?
        /// </summary>
        public bool IsOutputMuted { get { return IsSelfOutputMuted || Mathf.Approximately(0f, OutputVolume); } }
        /// <summary>
        /// Volume specific for this output source
        /// </summary>
        public float SelfOutputVolume { get { return Internal_selfOutputVolume; } set { Internal_selfOutputVolume = Mathf.Clamp01(value); } }
        /// <summary>
        /// Effective volume of this output source
        /// </summary>
        public float OutputVolume { get { return SelfOutputVolume * Workflow.Settings.VoiceChatVolume; } }
        /// <summary>
        /// Flag that determines which types of data format this class can process
        /// </summary>
        public AudioDataTypeFlag AvailableTypes { get; protected set; }
        /// <summary>
        /// True if VoiceHandler has initialized correctly
        /// </summary>
        public bool IsInitialized { get { return Internal_initialized; } }

        /// <summary>
        /// Exposed for tests. Internal local output volume
        /// </summary>
        [NonSerialized]
        public float Internal_selfOutputVolume;
        /// <summary>
        /// Exposed for tests. Initialization status
        /// </summary>
        [NonSerialized]
        public bool Internal_initialized;

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
            if (!Recorder.IsEnabled)
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
            if (!Recorder.IsEnabled)
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
        public void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
        {
            //Gives receiver the audio data for the output is not disabled
            if (Receiver.enabled)
                Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
        }
        /// <summary>
        /// Exposed for tests. Logic used when PTT is off
        /// </summary>
        public void Internal_PTTOffUpdate()
        {
            if (!Recorder.IsEnabled)
                Recorder.StartRecording();
        }
        /// <summary>
        /// Exposed for tests. Logic used when PTT is on
        /// </summary>
        public void Internal_PTTOnUpdate()
        {
            if (Input.GetKey(Workflow.Settings.PushToTalkKey))
            {
                //if ptt key is pressed and recorder is not recording start recording
                if (!Recorder.IsEnabled)
                    Recorder.StartRecording();
            }
            else if (Recorder.IsEnabled) //if ptt key is not pressed and recorder is recording stop recording
                Recorder.StopRecording();
        }
        /// <summary>
        /// Exposed for tests. Responds to settings voice chat changed event. Disables-Enables I/O system
        /// </summary>
        public void Internal_OnVoiceChatEnabledChanged()
        {
            this.enabled = Workflow.Settings.VoiceChatEnabled;
        }
        /// <summary>
        /// Exposed for tests. Update used to initialize status
        /// </summary>
        public void Internal_InitUpdate()
        {
            if (Identity != null && Identity.IsInitialized)
            {
                Internal_initialized = true;

                if (Receiver == null || Recorder == null)
                    throw new NullReferenceException("VoiceHandler requeires 2 valid components VoiceReceiver and VoiceRecorder");

                //Compatibility check between self, receiver and recorder
                AudioDataTypeFlag res = Receiver.AvailableTypes & Recorder.AvailableTypes & SelfFlag;

                if (res == AudioDataTypeFlag.None)
                    throw new ArgumentException("The voice handler underlying receiver and recorder components are incompatible");

                //Set the compatibility value of this handler
                AvailableTypes = res;

                OnEnable();
            }
        }
        /// <summary>
        /// Exposed for tests. Update used after initialization occurred
        /// </summary>
        public void Internal_NormalUpdate()
        {
            //If it is not a recorder update output volume
            if (!IsRecorder)
            {
                Receiver.Volume = OutputVolume;
                return;
            }

            //custom ptt update
            if (Workflow.Settings.PushToTalk)
                Internal_PTTOnUpdate();
            else
                Internal_PTTOffUpdate();

            //if the are mic data recorded available inform the workflow
            if (Recorder.IsEnabled && Recorder.MicDataAvailable > 0)
                Workflow.ProcessMicData(this);
        }
        /// <summary>
        /// Exposed for tests. Updates I/O system
        /// </summary>
        public void Update()
        {
            if (Internal_initialized)
            {
                Internal_NormalUpdate();
            }
            else
            {
                Internal_InitUpdate();
            }
        }
        /// <summary>
        /// Exposed for tests. Enables I/O system
        /// </summary>
        public void OnEnable()
        {
            if (Internal_initialized)
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
        /// <summary>
        /// Exposed for tests. Disables I/O system
        /// </summary>
        public void OnDisable()
        {
            if (Internal_initialized)
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
        /// <summary>
        /// Exposed for tests. Initialize status
        /// </summary>
        public void Awake()
        {
            Internal_initialized = false;
            Internal_selfOutputVolume = 1f;

            Recorder.enabled = false;
            Receiver.enabled = false;

            Workflow.Settings.VoiceChatEnabledChanged += Internal_OnVoiceChatEnabledChanged;

            Internal_OnVoiceChatEnabledChanged();
        }
        /// <summary>
        /// Exposed for tests. Finalize status
        /// </summary>
        public void OnDestroy()
        {
            Workflow.Settings.VoiceChatEnabledChanged -= Internal_OnVoiceChatEnabledChanged;
        }
    }
}