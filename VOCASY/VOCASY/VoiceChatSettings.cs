using UnityEngine;
using System.IO;
namespace VOCASY
{
    /// <summary>
    /// Class that manages and holds voice chat settings
    /// </summary>
    public abstract class VoiceChatSettings : ScriptableObject
    {
        /// <summary>
        /// Delegate used on some settings changed
        /// </summary>
        public delegate void OnSettingChanged();
        /// <summary>
        /// Delegate used on event AudioQualityChanged
        /// </summary>
        /// <param name="previousFrequency">previous frequency value</param>
        public delegate void OnFrequencyChanged(FrequencyType previousFrequency);
        /// <summary>
        /// Delegate used on event MicrophoneDeviceChanged
        /// </summary>
        /// <param name="previousDevice">previous device name</param>
        public delegate void OnMicDeviceChanged(string previousDevice);
        /// <summary>
        /// Minimum frequency possible
        /// </summary>
        public abstract ushort MinFrequency { get; }
        /// <summary>
        /// Maximum frequency possible
        /// </summary>
        public abstract ushort MaxFrequency { get; }
        /// <summary>
        /// Minimum channels possible
        /// </summary>
        public abstract byte MinChannels { get; }
        /// <summary>
        /// Maximum channels possible
        /// </summary>
        public abstract byte MaxChannels { get; }
        /// <summary>
        /// Name of the folder used to store files
        /// </summary>
        public virtual string FolderName
        {
            get { return folderName; }
            set
            {
                if (!value.Equals(folderName))
                {
                    if (File.Exists(SavedCustomValuesPath))
                        File.Delete(SavedCustomValuesPath);

                    folderName = value;

                    SavedCustomValuesDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);

                    SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

                    RestoreToSavedSettings();
                }
            }
        }
        /// <summary>
        /// Name of the file used to store settings
        /// </summary>
        public virtual string SettingsFileName
        {
            get { return settingsFileName; }
            set
            {
                if (!value.Equals(settingsFileName))
                {
                    if (File.Exists(SavedCustomValuesPath))
                        File.Delete(SavedCustomValuesPath);

                    settingsFileName = value;

                    SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

                    RestoreToSavedSettings();
                }
            }
        }
        /// <summary>
        /// File complete path name that contains saved settings.
        /// </summary>
        public string SavedCustomValuesPath { get; protected set; }
        /// <summary>
        /// Directory full path that contains files.
        /// </summary>
        public string SavedCustomValuesDirectoryPath { get; protected set; }
        /// <summary>
        /// Determines whenever self should be muted
        /// </summary>
        public bool MuteSelf
        {
            get { return muteSelf; }
            set
            {
                if (muteSelf != value)
                {
                    muteSelf = value;
                    if (MuteSelfChanged != null)
                        MuteSelfChanged.Invoke();
                }
            }
        }
        /// <summary>
        /// Determines if voice chat works in mode push to talk
        /// </summary>
        public bool PushToTalk
        {
            get { return pushToTalk; }
            set
            {
                if (pushToTalk != value)
                {
                    pushToTalk = value;
                    if (PushToTalkChanged != null)
                        PushToTalkChanged.Invoke();
                }
            }
        }
        /// <summary>
        /// Key used in push to talk mode
        /// </summary>
        public KeyCode PushToTalkKey = KeyCode.C;
        /// <summary>
        /// Audio quality used. Does not effect audio received from network
        /// </summary>
        public FrequencyType AudioQuality
        {
            get { return audioQuality; }
            set
            {
                FrequencyType newF = (FrequencyType)Mathf.Clamp((int)value, MinFrequency, MaxFrequency);
                if (audioQuality != newF)
                {
                    FrequencyType prev = audioQuality;
                    audioQuality = newF;
                    if (AudioQualityChanged != null)
                        AudioQualityChanged.Invoke(prev);
                }
            }
        }
        /// <summary>
        /// Current microphone device to be used for recording
        /// </summary>
        public string MicrophoneDevice
        {
            get { return microphoneDevice; }
            set
            {
                if (!value.Equals(microphoneDevice))
                {
                    string prev = microphoneDevice;
                    microphoneDevice = value;
                    if (MicrophoneDeviceChanged != null)
                        MicrophoneDeviceChanged.Invoke(prev);
                }
            }
        }
        /// <summary>
        /// Determines whenever voice chat shoul dbe enabled
        /// </summary>
        public bool VoiceChatEnabled
        {
            get { return voiceChatEnabled; }
            set
            {
                if (voiceChatEnabled != value)
                {
                    voiceChatEnabled = value;
                    if (VoiceChatEnabledChanged != null)
                        VoiceChatEnabledChanged.Invoke();
                }
            }
        }
        /// <summary>
        /// Determines volume of voice chat data received from network
        /// </summary>
        public float VoiceChatVolume { get { return voiceChatVolume; } set { voiceChatVolume = value; } }

        /// <summary>
        /// Event called whenever push to talk mode has been changed
        /// </summary>
        public event OnSettingChanged PushToTalkChanged;
        /// <summary>
        /// Event called whenever MuteSelf state has been changed
        /// </summary>
        public event OnSettingChanged MuteSelfChanged;
        /// <summary>
        /// Event called whenever Audio Quality value has been changed , previous audio quality is passed as argument
        /// </summary>
        public event OnFrequencyChanged AudioQualityChanged;
        /// <summary>
        /// Event called whenever the current microphone device has been changed, previous mic device is passed as argument
        /// </summary>
        public event OnMicDeviceChanged MicrophoneDeviceChanged;
        /// <summary>
        /// Event called whenever Voice Chat enbaled state has been changed
        /// </summary>
        public event OnSettingChanged VoiceChatEnabledChanged;

        [SerializeField]
        private string folderName = "Communication";

        [SerializeField]
        private string settingsFileName = "VoiceChatSettings.txt";

        [SerializeField]
        private string microphoneDevice = string.Empty;

        [SerializeField]
        private bool voiceChatEnabled = true;

        [SerializeField]
        private bool pushToTalk = true;

        [SerializeField]
        private bool muteSelf = false;

        [SerializeField]
        private FrequencyType audioQuality = FrequencyType.BestQuality;

        [SerializeField]
        [Range(0f, 1f)]
        private float voiceChatVolume = 1f;

        /// <summary>
        /// Restore the settings to the saved file values. If file is not found it is created with current settings values
        /// </summary>
        public abstract void RestoreToSavedSettings();
        /// <summary>
        /// Saves current settings to file. If it is not performed changes to the settings will not be recorded
        /// </summary>
        public abstract void SaveCurrentSettings();

        /// <summary>
        /// Initializes paths and gets values from file
        /// </summary>
        protected virtual void OnEnable()
        {
            SavedCustomValuesDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);

            SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

            RestoreToSavedSettings();
        }
    }
}