using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VOCASY;
using VOCASY.Common;
using GENUtility;
using System.IO;

[TestFixture]
[TestOf(typeof(VoiceChatSettings))]
[Category("VOCASY")]
public class VoiceChatSettingsTest
{
    Settings settings;

    [SetUp]
    public void SetupSettings()
    {
        settings = ScriptableObject.CreateInstance<Settings>();
    }
    [TearDown]
    public void TearDownSettings()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        ScriptableObject.DestroyImmediate(settings);
    }
    [Test]
    public void TestInitSavedFolderPathValue()
    {
        Assert.That(settings.SavedCustomValuesDirectoryPath.Equals(Path.Combine(Application.persistentDataPath, settings.FolderName)));
    }
    [Test]
    public void TestInitSavedFilePathValue()
    {
        Assert.That(settings.SavedCustomValuesPath.Equals(Path.Combine(Path.Combine(Application.persistentDataPath, settings.FolderName), settings.SettingsFileName)));
    }
    [Test]
    public void TestChangeFolderName()
    {
        settings.FolderName = "Pippo";
        Assert.That(settings.SavedCustomValuesDirectoryPath.Equals(Path.Combine(Application.persistentDataPath, "Pippo")));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFolderNameEmptyString()
    {
        settings.FolderName = "";
        Assert.That(settings.SavedCustomValuesDirectoryPath.Equals(Path.Combine(Application.persistentDataPath, "")));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFolderNameNullStringNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => settings.FolderName = null);
    }
    [Test]
    public void TestChangeFolderNameFileCreated()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.FolderName = "Pippo";
        Assert.That(File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, "Pippo"), settings.SettingsFileName)));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFolderNameDeletePrevFile()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.FolderName = "Pippo";
        settings.FolderName = "Pippo2";
        Assert.That(!File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, "Pippo"), settings.SettingsFileName)));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFolderName2()
    {
        settings.FolderName = "Pippo";
        Assert.That(settings.FolderName.Equals("Pippo"));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFileName()
    {
        settings.SettingsFileName = "Pluto.txt";
        Assert.That(settings.SavedCustomValuesPath.Equals(Path.Combine(Path.Combine(Application.persistentDataPath, settings.FolderName), "Pluto.txt")));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFileNameEmptyStringWithExtension()
    {
        settings.SettingsFileName = ".txt";
        Assert.That(settings.SavedCustomValuesPath.Equals(Path.Combine(Path.Combine(Application.persistentDataPath, settings.FolderName), ".txt")));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFileNameEmptyStringWithoutExtensionNoAuthorizationException()
    {
        Assert.Throws<UnauthorizedAccessException>(() => settings.SettingsFileName = "");
    }
    [Test]
    public void TestChangeFileNameNullStringNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => settings.SettingsFileName = null);
    }
    [Test]
    public void TestChangeFileNameFileCreated()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.SettingsFileName = "Pinco.txt";
        Assert.That(File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, settings.FolderName), "Pinco.txt")));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFileName2()
    {
        settings.SettingsFileName = "Pluto.txt";
        Assert.That(settings.SettingsFileName.Equals("Pluto.txt"));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeFileNameDeletePrevFile()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.SettingsFileName = "Pluto.txt";
        settings.SettingsFileName = "Pluto2.txt";
        Assert.That(!File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, settings.FolderName), "Pluto.txt")));
        File.Delete(settings.SavedCustomValuesPath);
    }
    [Test]
    public void TestChangeValueMuteSelf()
    {
        settings.MuteSelf = false;
        settings.MuteSelf = true;
        Assert.That(settings.MuteSelf, Is.True);
    }
    [Test]
    public void TestChangeValueMuteSelfEventInvoked()
    {
        settings.MuteSelf = false;
        bool res = false;
        settings.MuteSelfChanged += () => res = true;
        settings.MuteSelf = true;
        Assert.That(res, Is.True);
        settings.MuteSelfChanged -= () => res = true;
    }
    [Test]
    public void TestChangeValueMuteSelfEventNotInvoked()
    {
        settings.MuteSelf = true;
        bool res = false;
        settings.MuteSelfChanged += () => res = true;
        settings.MuteSelf = true;
        Assert.That(res, Is.False);
        settings.MuteSelfChanged -= () => res = true;
    }
    [Test]
    public void TestChangeValueMuteSelf2()
    {
        settings.MuteSelf = true;
        settings.MuteSelf = false;
        Assert.That(settings.MuteSelf, Is.False);
    }
    [Test]
    public void TestChangeValuePTTEventInvoked()
    {
        settings.PushToTalk = false;
        bool res = false;
        settings.PushToTalkChanged += () => res = true;
        settings.PushToTalk = true;
        Assert.That(res, Is.True);
        settings.PushToTalkChanged -= () => res = true;
    }
    [Test]
    public void TestChangeValuePTTEventNotInvoked()
    {
        settings.PushToTalk = true;
        bool res = false;
        settings.PushToTalkChanged += () => res = true;
        settings.PushToTalk = true;
        Assert.That(res, Is.False);
        settings.PushToTalkChanged -= () => res = true;
    }
    [Test]
    public void TestChangeValuePTT()
    {
        settings.PushToTalk = false;
        settings.PushToTalk = true;
        Assert.That(settings.PushToTalk, Is.True);
    }
    [Test]
    public void TestChangeValuePTT2()
    {
        settings.PushToTalk = true;
        settings.PushToTalk = false;
        Assert.That(settings.PushToTalk, Is.False);
    }
    [Test]
    public void TestChangeValueChatEnabled()
    {
        settings.VoiceChatEnabled = false;
        settings.VoiceChatEnabled = true;
        Assert.That(settings.VoiceChatEnabled, Is.True);
    }
    [Test]
    public void TestChangeValueChatEnabledEventInvoked()
    {
        settings.VoiceChatEnabled = false;
        bool res = false;
        settings.VoiceChatEnabledChanged += () => res = true;
        settings.VoiceChatEnabled = true;
        Assert.That(res, Is.True);
        settings.VoiceChatEnabledChanged -= () => res = true;
    }
    [Test]
    public void TestChangeValueChatEnabledEventNotInvoked()
    {
        settings.VoiceChatEnabled = true;
        bool res = false;
        settings.VoiceChatEnabledChanged += () => res = true;
        settings.VoiceChatEnabled = true;
        Assert.That(res, Is.False);
        settings.VoiceChatEnabledChanged -= () => res = true;
    }
    [Test]
    public void TestChangeValueChatEnabled2()
    {
        settings.VoiceChatEnabled = true;
        settings.VoiceChatEnabled = false;
        Assert.That(settings.VoiceChatEnabled, Is.False);
    }
    [Test]
    public void TestChangeValuePTTKey()
    {
        settings.PushToTalkKey = KeyCode.A;
        settings.PushToTalkKey = KeyCode.Alpha0;
        Assert.That(settings.PushToTalkKey, Is.EqualTo(KeyCode.Alpha0));
    }
    [Test]
    public void TestChangeValuePTTKey2()
    {
        settings.PushToTalkKey = KeyCode.Alpha0;
        settings.PushToTalkKey = KeyCode.A;
        Assert.That(settings.PushToTalkKey, Is.EqualTo(KeyCode.A));
    }
    [Test]
    public void TestChangeValueDeviceNameEmptyString()
    {
        settings.MicrophoneDevice = "first";
        settings.MicrophoneDevice = "";
        Assert.That(settings.MicrophoneDevice.Equals(""));
    }
    [Test]
    public void TestChangeValueDeviceNameNullStringToEmptyString()
    {
        settings.MicrophoneDevice = "first";
        settings.MicrophoneDevice = null;
        Assert.That(settings.MicrophoneDevice.Equals(""));
    }
    [Test]
    public void TestChangeValueDeviceName()
    {
        settings.MicrophoneDevice = "first";
        settings.MicrophoneDevice = "second";
        Assert.That(settings.MicrophoneDevice.Equals("second"));
    }
    [Test]
    public void TestChangeValueDeviceNameEventInvoked()
    {
        settings.MicrophoneDevice = null;
        bool res = false;
        settings.MicrophoneDeviceChanged += (string s) => res = true;
        settings.MicrophoneDevice = "Ciao";
        Assert.That(res, Is.True);
        settings.MicrophoneDeviceChanged -= (string s) => res = true;
    }
    [Test]
    public void TestChangeValueDeviceNameEventNotInvoked()
    {
        settings.MicrophoneDevice = "Ciao";
        bool res = false;
        settings.MicrophoneDeviceChanged += (string s) => res = true;
        settings.MicrophoneDevice = "Ciao";
        Assert.That(res, Is.False);
        settings.MicrophoneDeviceChanged -= (string s) => res = true;
    }
    [Test]
    public void TestChangeValueDeviceName2()
    {
        settings.MicrophoneDevice = "first";
        settings.MicrophoneDevice = "second";
        Assert.That(settings.MicrophoneDevice.Equals("second"));
    }
    [Test]
    public void TestChangeValueAudioQuality()
    {
        settings.AudioQuality = FrequencyType.VoipQuality;
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(settings.AudioQuality, Is.EqualTo(FrequencyType.HighQuality));
    }
    [Test]
    public void TestChangeValueAudioQualityEventInvoked()
    {
        settings.AudioQuality = FrequencyType.HighQuality;
        bool res = false;
        settings.AudioQualityChanged += (FrequencyType f) => res = true;
        settings.AudioQuality = FrequencyType.VoipQuality;
        Assert.That(res, Is.True);
        settings.AudioQualityChanged -= (FrequencyType f) => res = true;
    }
    [Test]
    public void TestChangeValueAudioQualityEventNotInvoked()
    {
        settings.AudioQuality = FrequencyType.HighQuality;
        bool res = false;
        settings.AudioQualityChanged += (FrequencyType f) => res = true;
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(res, Is.False);
        settings.AudioQualityChanged -= (FrequencyType f) => res = true;
    }
    [Test]
    public void TestChangeValueAudioQuality2()
    {
        settings.AudioQuality = FrequencyType.HighQuality;
        settings.AudioQuality = FrequencyType.VoipQuality;
        Assert.That(settings.AudioQuality, Is.EqualTo(FrequencyType.VoipQuality));
    }
    [Test]
    public void TestChangeValueAudioQuality3()
    {
        settings.AudioQuality = FrequencyType.VoipQuality;
        settings.AudioQuality = (FrequencyType)ushort.MaxValue;
        Assert.That(settings.AudioQuality, Is.EqualTo((FrequencyType)48000));
    }
    [Test]
    public void TestChangeValueAudioQuality4()
    {
        settings.AudioQuality = FrequencyType.VoipQuality;
        settings.AudioQuality = (FrequencyType)ushort.MinValue;
        Assert.That(settings.AudioQuality, Is.EqualTo((FrequencyType)12000));
    }
    [Test]
    public void TestChangeValueVoiceChatVolume()
    {
        settings.VoiceChatVolume = 0f;
        settings.VoiceChatVolume = 0.5f;
        Assert.That(settings.VoiceChatVolume, Is.EqualTo(0.5).Within(0.0001));
    }
    [Test]
    public void TestChangeValueVoiceChatVolume2()
    {
        settings.VoiceChatVolume = 0.5f;
        settings.VoiceChatVolume = 0f;
        Assert.That(settings.VoiceChatVolume, Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestChangeValueVoiceChatVolume3()
    {
        settings.VoiceChatVolume = 0f;
        settings.VoiceChatVolume = 2f;
        Assert.That(settings.VoiceChatVolume, Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestChangeValueVoiceChatVolume4()
    {
        settings.VoiceChatVolume = 0.5f;
        settings.VoiceChatVolume = -20f;
        Assert.That(settings.VoiceChatVolume, Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestSaveSettingsFileCreated()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.SaveCurrentSettings();
        Assert.That(File.Exists(settings.SavedCustomValuesPath));
    }
    [Test]
    public void TestSaveSettingsFileContentValid()
    {
        settings.AudioQuality = (FrequencyType)17500;
        string json = JsonUtility.ToJson(settings);
        settings.SaveCurrentSettings();
        Assert.That(json.Equals(File.ReadAllText(settings.SavedCustomValuesPath)));
    }
    [Test]
    public void TestSaveSettingsFileContentValid2()
    {
        settings.AudioQuality = (FrequencyType)17500;
        settings.SaveCurrentSettings();
        settings.AudioQuality = (FrequencyType)27500;
        JsonUtility.FromJsonOverwrite(File.ReadAllText(settings.SavedCustomValuesPath), settings);
        Assert.That(settings.AudioQuality, Is.EqualTo((FrequencyType)17500));
    }
    [Test]
    public void TestRestoreSettingsFileCreated()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.RestoreToSavedSettings();
        Assert.That(File.Exists(settings.SavedCustomValuesPath));
    }
    [Test]
    public void TestRestoreSettingsFileContentValid()
    {
        settings.AudioQuality = (FrequencyType)17500;
        string json = JsonUtility.ToJson(settings);
        settings.RestoreToSavedSettings();
        Assert.That(!json.Equals(File.ReadAllText(settings.SavedCustomValuesPath)));
    }
    [Test]
    public void TestRestoreSettingsFileContentValid2()
    {
        settings.AudioQuality = (FrequencyType)17500;
        settings.SaveCurrentSettings();
        settings.AudioQuality = (FrequencyType)27500;
        settings.RestoreToSavedSettings();
        Assert.That(settings.AudioQuality, Is.EqualTo((FrequencyType)17500));
    }
    [Test]
    public void TestValuesInitSettings()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.AudioQuality = (FrequencyType)17500;
        settings.SaveCurrentSettings();
        Settings other = ScriptableObject.CreateInstance<Settings>();
        Assert.That(other.AudioQuality, Is.EqualTo((FrequencyType)17500));
        ScriptableObject.DestroyImmediate(other);
    }
    [Test]
    public void TestValuesInitSettings2()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.MuteSelf = true;
        settings.SaveCurrentSettings();
        Settings other = ScriptableObject.CreateInstance<Settings>();
        Assert.That(other.MuteSelf, Is.True);
        ScriptableObject.DestroyImmediate(other);
    }
    [Test]
    public void TestValuesInitSettings3()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.AudioQuality = (FrequencyType)47500;
        settings.SaveCurrentSettings();
        Settings other = ScriptableObject.CreateInstance<Settings>();
        Assert.That(other.AudioQuality, Is.EqualTo((FrequencyType)47500));
        ScriptableObject.DestroyImmediate(other);
    }
    [Test]
    public void TestValuesInitSettings4()
    {
        if (File.Exists(settings.SavedCustomValuesPath))
            File.Delete(settings.SavedCustomValuesPath);
        settings.MuteSelf = false;
        settings.SaveCurrentSettings();
        Settings other = ScriptableObject.CreateInstance<Settings>();
        Assert.That(other.MuteSelf, Is.False);
        ScriptableObject.DestroyImmediate(other);
    }
}