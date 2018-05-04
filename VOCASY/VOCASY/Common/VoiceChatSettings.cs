using UnityEngine;
using System.IO;
using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages and holds voice chat settings
    /// </summary>
    [CreateAssetMenu(menuName = "Communication/Voice Chat/Settings")]
    public class VoiceChatSettings : ScriptableObject, IVoiceChatSettings
    {
        /// <summary>
        /// Minimum frequency possible
        /// </summary>
        public const ushort MinFrequency = (ushort)FrequencyType.LowerThanAverageQuality;
        /// <summary>
        /// Maximum frequency possible
        /// </summary>
        public const ushort MaxFrequency = (ushort)FrequencyType.BestQuality;
        /// <summary>
        /// Name of the folder used to store files
        /// </summary>
        public string FolderName
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
        public string SettingsFileName
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
        public string SavedCustomValuesPath { get; private set; }
        /// <summary>
        /// Directory full path that contains files.
        /// </summary>
        public string SavedCustomValuesDirectoryPath { get; private set; }
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
        public int PushToTalkKey { get { return (int)pushToTalkKey; } set { pushToTalkKey = (KeyCode)value; } }
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
        public event Action PushToTalkChanged;
        /// <summary>
        /// Event called whenever MuteSelf state has been changed
        /// </summary>
        public event Action MuteSelfChanged;
        /// <summary>
        /// Event called whenever Audio Quality value has been changed , previous audio quality is passed as argument
        /// </summary>
        public event Action<FrequencyType> AudioQualityChanged;
        /// <summary>
        /// Event called whenever the current microphone device has been changed, previous mic device is passed as argument
        /// </summary>
        public event Action<string> MicrophoneDeviceChanged;
        /// <summary>
        /// Event called whenever Voice Chat enbaled state has been changed
        /// </summary>
        public event Action VoiceChatEnabledChanged;

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
        private KeyCode pushToTalkKey = KeyCode.C;

        [SerializeField]
        private bool muteSelf = false;

        [SerializeField]
        private FrequencyType audioQuality = (FrequencyType)MaxFrequency;

        [Range(0f, 1f)]
        [SerializeField]
        private float voiceChatVolume = 1f;

        /// <summary>
        /// Restore the settings to the saved file values. If file is not found it is created with current settings values
        /// </summary>
        public void RestoreToSavedSettings()
        {
            if (File.Exists(SavedCustomValuesPath))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(SavedCustomValuesPath), this);
            else
                SaveCurrentSettings();
        }
        /// <summary>
        /// Saves current settings to file. If it is not performed changes to the settings will not be recorded
        /// </summary>
        public void SaveCurrentSettings()
        {
            if (!Directory.Exists(SavedCustomValuesDirectoryPath))
                Directory.CreateDirectory(SavedCustomValuesDirectoryPath);

            File.WriteAllText(SavedCustomValuesPath, JsonUtility.ToJson(this));
        }

        void OnEnable()
        {
            SavedCustomValuesDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);

            SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

            RestoreToSavedSettings();
        }
    }
}