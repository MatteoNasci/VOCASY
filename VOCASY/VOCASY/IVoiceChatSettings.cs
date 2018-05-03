using System;
namespace VOCASY
{
    public interface IVoiceChatSettings
    {
        /// <summary>
        /// Name of the folder used to store files
        /// </summary>
        string FolderName { get; set; }
        /// <summary>
        /// Name of the file used to store settings
        /// </summary>
        string SettingsFileName { get; set; }
        /// <summary>
        /// File complete path name that contains saved settings.
        /// </summary>
        string SavedCustomValuesPath { get; }
        /// <summary>
        /// Directory full path that contains files.
        /// </summary>
        string SavedCustomValuesDirectoryPath { get; }
        /// <summary>
        /// Determines whenever self should be muted
        /// </summary>
        bool MuteSelf { get; set; }
        /// <summary>
        /// Determines if voice chat works in mode push to talk
        /// </summary>
        bool PushToTalk { get; set; }
        /// <summary>
        /// Key used in push to talk mode
        /// </summary>
        int PushToTalkKey { get; set; }
        /// <summary>
        /// Audio quality used. Does not effect audio received from network
        /// </summary>
        FrequencyType AudioQuality { get; set; }
        /// <summary>
        /// Current microphone device to be used for recording
        /// </summary>
        string MicrophoneDevice { get; set; }
        /// <summary>
        /// Determines whenever voice chat shoul dbe enabled
        /// </summary>
        bool VoiceChatEnabled { get; set; }
        /// <summary>
        /// Determines volume of voice chat data received from network
        /// </summary>
        float VoiceChatVolume { get; set; }

        /// <summary>
        /// Event called whenever push to talk mode has been changed
        /// </summary>
        event Action PushToTalkChanged;
        /// <summary>
        /// Event called whenever MuteSelf state has been changed
        /// </summary>
        event Action MuteSelfChanged;
        /// <summary>
        /// Event called whenever Audio Quality value has been changed , previous audio quality is passed as argument
        /// </summary>
        event Action<FrequencyType> AudioQualityChanged;
        /// <summary>
        /// Event called whenever the current microphone device has been changed, previous mic device is passed as argument
        /// </summary>
        event Action<string> MicrophoneDeviceChanged;
        /// <summary>
        /// Event called whenever Voice Chat enbaled state has been changed
        /// </summary>
        event Action VoiceChatEnabledChanged;

        /// <summary>
        /// Restore the settings to the saved file values. If file is not found it is created with current settings values
        /// </summary>
        void RestoreToSavedSettings();
        /// <summary>
        /// Saves current settings to file. If it is not performed changes to the settings will not be recorded
        /// </summary>
        void SaveCurrentSettings();
    }
}