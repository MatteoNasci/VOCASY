using UnityEngine;
using System.IO;
using System;
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

    public string SavedCustomValuesPath { get; private set; }

    public string SavedCustomValuesDirectoryPath { get; private set; }

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

    public KeyCode PushToTalkKey { get { return pushToTalkKey; } set { pushToTalkKey = value; } }

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

    public float VoiceChatVolume { get { return voiceChatVolume; } set { voiceChatVolume = value; } }


    public event Action PushToTalkChanged;

    public event Action MuteSelfChanged;

    public event Action<FrequencyType> AudioQualityChanged;

    public event Action<string> MicrophoneDeviceChanged;

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

    public void RestoreToSavedSettings()
    {
        if (File.Exists(SavedCustomValuesPath))
            JsonUtility.FromJsonOverwrite(File.ReadAllText(SavedCustomValuesPath), this);
        else
            SaveCurrentSettings();
    }

    public void SaveCurrentSettings()
    {
        if (!Directory.Exists(SavedCustomValuesDirectoryPath))
            Directory.CreateDirectory(SavedCustomValuesDirectoryPath);

        File.WriteAllText(SavedCustomValuesPath, JsonUtility.ToJson(this));
    }

    public bool IsPushToTalkKeyOpen()
    {
        return Input.GetKey(PushToTalkKey);
    }

    public bool IsPushToTalkKeyReleased()
    {
        return Input.GetKeyUp(PushToTalkKey);
    }

    public bool IsPushToTalkKeyDown()
    {
        return Input.GetKeyDown(PushToTalkKey);
    }

    void OnEnable()
    {
        SavedCustomValuesDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);

        SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

        RestoreToSavedSettings();
    }
}