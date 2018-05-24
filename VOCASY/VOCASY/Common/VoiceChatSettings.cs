using UnityEngine;
using System.IO;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages and holds voice chat settings
    /// </summary>
    [CreateAssetMenu(menuName = "VOCASY/Settings")]
    public class VoiceChatSettings : ScriptableObject
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
        public const ushort MinFrequency = (ushort)FrequencyType.LowerThanAverageQuality;
        /// <summary>
        /// Maximum frequency possible
        /// </summary>
        public const ushort MaxFrequency = (ushort)FrequencyType.BestQuality;
        /// <summary>
        /// Minimum frequency possible
        /// </summary>
        public const byte MinChannels = 1;
        /// <summary>
        /// Maximum frequency possible
        /// </summary>
        public const byte MaxChannels = 2;
        /// <summary>
        /// Name of the folder used to store files
        /// </summary>
        public string FolderName
        {
            get { return Internal_folderName; }
            set
            {
                if (!value.Equals(Internal_folderName))
                {
                    if (File.Exists(SavedCustomValuesPath))
                        File.Delete(SavedCustomValuesPath);

                    Internal_folderName = value;

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
            get { return Internal_settingsFileName; }
            set
            {
                if (!value.Equals(Internal_settingsFileName))
                {
                    if (File.Exists(SavedCustomValuesPath))
                        File.Delete(SavedCustomValuesPath);

                    Internal_settingsFileName = value;

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
            get { return Internal_muteSelf; }
            set
            {
                if (Internal_muteSelf != value)
                {
                    Internal_muteSelf = value;
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
            get { return Internal_pushToTalk; }
            set
            {
                if (Internal_pushToTalk != value)
                {
                    Internal_pushToTalk = value;
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
            get { return Internal_audioQuality; }
            set
            {
                FrequencyType newF = (FrequencyType)Mathf.Clamp((int)value, MinFrequency, MaxFrequency);
                if (Internal_audioQuality != newF)
                {
                    FrequencyType prev = Internal_audioQuality;
                    Internal_audioQuality = newF;
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
            get { return Internal_microphoneDevice; }
            set
            {
                if (!value.Equals(Internal_microphoneDevice))
                {
                    string prev = Internal_microphoneDevice;
                    Internal_microphoneDevice = value;
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
            get { return Internal_voiceChatEnabled; }
            set
            {
                if (Internal_voiceChatEnabled != value)
                {
                    Internal_voiceChatEnabled = value;
                    if (VoiceChatEnabledChanged != null)
                        VoiceChatEnabledChanged.Invoke();
                }
            }
        }
        /// <summary>
        /// Determines volume of voice chat data received from network
        /// </summary>
        public float VoiceChatVolume { get { return Internal_voiceChatVolume; } set { Internal_voiceChatVolume = value; } }

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

        /// <summary>
        /// Exposed for tests. Folder used
        /// </summary>
        public string Internal_folderName = "Communication";
        /// <summary>
        /// Exposed for tests. Saved settings file name
        /// </summary>
        public string Internal_settingsFileName = "VoiceChatSettings.txt";
        /// <summary>
        /// Exposed for tests. Device name
        /// </summary>
        public string Internal_microphoneDevice = string.Empty;
        /// <summary>
        /// Exposed for tests. Voice chat status
        /// </summary>
        public bool Internal_voiceChatEnabled = true;
        /// <summary>
        /// Exposed for tests. PTT status
        /// </summary>
        public bool Internal_pushToTalk = true;
        /// <summary>
        /// Exposed for tests. True if mic is muted
        /// </summary>
        public bool Internal_muteSelf = false;
        /// <summary>
        /// Exposed for tests. Frequency used
        /// </summary>
        public FrequencyType Internal_audioQuality = (FrequencyType)MaxFrequency;
        /// <summary>
        /// Exposed for tests. General voice chat volume
        /// </summary>
        [Range(0f, 1f)]
        public float Internal_voiceChatVolume = 1f;

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
        /// <summary>
        /// Exposed for tests. Initializes paths and read from file
        /// </summary>
        public void OnEnable()
        {
            SavedCustomValuesDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);

            SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

            RestoreToSavedSettings();
        }
    }
}