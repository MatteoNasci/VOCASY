using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using VOCASY.Common;
using VOCASY;
using UnityEngine.TestTools;
using System.Reflection;
using GENUtility;
using UnityEngine;
[TestFixture]
[TestOf(typeof(Recorder))]
[Category("VOCASY")]
public class RecorderTest
{
    GameObject go;
    Recorder recorder;
    SupportSettings settings;

    FieldInfo recIsEnabled;

    FieldInfo recMinDevFrequency;
    FieldInfo recMaxDevFrequency;

    FieldInfo recClip;
    FieldInfo recPrevOffset;

    FieldInfo recCyclicAudioBuffer;
    FieldInfo recReadIndex;
    FieldInfo recWriteIndex;

    MethodInfo recUpdate;
    MethodInfo recAwake;
    MethodInfo recOnDestroy;

    MethodInfo recOnFrequencyChanged;
    MethodInfo recOnMicDeviceChanged;

    [OneTimeSetUp]
    public void OneTimeSetupReflections()
    {
        Type t = typeof(Recorder);
        recIsEnabled = t.GetField("isEnabled", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recMinDevFrequency = t.GetField("minDevFrequency", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recMaxDevFrequency = t.GetField("maxDevFrequency", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recClip = t.GetField("clip", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recPrevOffset = t.GetField("prevOffset", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recCyclicAudioBuffer = t.GetField("cyclicAudioBuffer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recReadIndex = t.GetField("readIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recWriteIndex = t.GetField("writeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        recUpdate = t.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recAwake = t.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recOnDestroy = t.GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recOnFrequencyChanged = t.GetMethod("OnFrequencyChanged", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        recOnMicDeviceChanged = t.GetMethod("OnMicDeviceChanged", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    [SetUp]
    public void SetupRecorder()
    {
        go = new GameObject();
        settings = ScriptableObject.CreateInstance<SupportSettings>();
        recorder = go.AddComponent<Recorder>();
        recorder.Settings = settings;
    }
    [TearDown]
    public void TeardownRecorder()
    {
        LogAssert.ignoreFailingMessages = true;
        recorder.StopRecording();
        recorder.Settings = null;
        ScriptableObject.DestroyImmediate(settings);
        GameObject.DestroyImmediate(go);
    }
    [Test]
    public void TestAvailableTypesValue()
    {
        Assert.That(recorder.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Both));
    }
    [Test]
    public void TestInitIsEnabledValue()
    {
        recIsEnabled.SetValue(recorder, true);
        recAwake.Invoke(recorder, new object[0]);
        Assert.That(recorder.IsEnabled, Is.False);
    }
    [Test]
    public void TestInitClipValue()
    {
        recAwake.Invoke(recorder, new object[0]);
        Assert.That(recClip.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestBufferInitValue()
    {
        recAwake.Invoke(recorder, new object[0]);
        Assert.That(recCyclicAudioBuffer.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestInitIsEnabledValue3()
    {
        recIsEnabled.SetValue(recorder, true);
        Assert.That(recorder.IsEnabled, Is.True);
    }
    [Test]
    public void TestMicDataAvailableCount()
    {
        recReadIndex.SetValue(recorder, 100);
        recWriteIndex.SetValue(recorder, 100);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(0));
    }
    [Test]
    public void TestMicDataAvailableCount2()
    {
        recCyclicAudioBuffer.SetValue(recorder, new float[10]);
        recReadIndex.SetValue(recorder, 0);
        recWriteIndex.SetValue(recorder, 0);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(0));
    }
    [Test]
    public void TestMicDataAvailableCount3()
    {
        recCyclicAudioBuffer.SetValue(recorder, new float[10]);
        recReadIndex.SetValue(recorder, 0);
        recWriteIndex.SetValue(recorder, 5);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(5));
    }
    [Test]
    public void TestMicDataAvailableCount4()
    {
        recCyclicAudioBuffer.SetValue(recorder, new float[10]);
        recReadIndex.SetValue(recorder, 9);
        recWriteIndex.SetValue(recorder, 1);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(2));
    }
    [Test]
    public void TestMicDataAvailableCount5()
    {
        recCyclicAudioBuffer.SetValue(recorder, new float[10]);
        recReadIndex.SetValue(recorder, 9);
        recWriteIndex.SetValue(recorder, 8);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(9));
    }
    [Test]
    public void TestMicDataAvailableCount6()
    {
        recCyclicAudioBuffer.SetValue(recorder, new float[10]);
        recReadIndex.SetValue(recorder, 8);
        recWriteIndex.SetValue(recorder, 9);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(1));
    }
    [Test]
    public void TestMicDataAvailableCount7()
    {
        recCyclicAudioBuffer.SetValue(recorder, new float[10]);
        recReadIndex.SetValue(recorder, 5);
        recWriteIndex.SetValue(recorder, 5);
        Assert.That(recorder.MicDataAvailable, Is.EqualTo(0));
    }
    [Test]
    public void TestInitIsEnabledValue2()
    {
        recIsEnabled.SetValue(recorder, true);
        recAwake.Invoke(recorder, new object[0]);
        Assert.That(recIsEnabled.GetValue(recorder), Is.False);
    }
    [Test]
    public void TestInitIsRecording()
    {
        recAwake.Invoke(recorder, new object[0]);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recClip.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription6()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recClip.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription7()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recorder.IsEnabled, Is.False);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription2()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recCyclicAudioBuffer.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription3()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recorder.IsEnabled, Is.True);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription8()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription9()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, false);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
    }
    [Test]
    public void TestInitOnFreqChangedSubscription4()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        recReadIndex.SetValue(recorder, 5);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recReadIndex.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestInitOnFreqChangedSubscription5()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        recWriteIndex.SetValue(recorder, 5);
        settings.AudioQuality = FrequencyType.HighQuality;
        Assert.That(recWriteIndex.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestOnFreqChanged()
    {
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        recIsEnabled.SetValue(recorder, true);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recClip.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestOnFreqChanged6()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recClip.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestOnFreqChanged7()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recorder.IsEnabled, Is.False);
    }
    [Test]
    public void TestOnFreqChanged2()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recIsEnabled.SetValue(recorder, true);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recCyclicAudioBuffer.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestOnFreqChanged3()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recIsEnabled.SetValue(recorder, true);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recorder.IsEnabled, Is.True);
    }
    [Test]
    public void TestOnFreqChanged8()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recIsEnabled.SetValue(recorder, true);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
    }
    [Test]
    public void TestOnFreqChanged9()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recIsEnabled.SetValue(recorder, false);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
    }
    [Test]
    public void TestOnFreqChanged4()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recIsEnabled.SetValue(recorder, true);
        recReadIndex.SetValue(recorder, 5);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recReadIndex.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestOnFreqChanged5()
    {
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        settings.AudioQuality = FrequencyType.LowerThanAverageQuality;
        recIsEnabled.SetValue(recorder, true);
        recWriteIndex.SetValue(recorder, 5);
        recOnFrequencyChanged.Invoke(recorder, new object[] { FrequencyType.BestQuality });
        Assert.That(recWriteIndex.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestInitOnMicDevChangedSubscription()
    {
        settings.MicrophoneDevice = null;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.MicrophoneDevice = "default";
        Assert.That(recClip.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestInitOnMicDevChangedSubscription2()
    {
        settings.MicrophoneDevice = null;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, false);
        settings.MicrophoneDevice = "default";
        Assert.That(recClip.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestInitOnMicDevChangedSubscription3()
    {
        settings.MicrophoneDevice = null;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.MicrophoneDevice = "default";
        Assert.That(recorder.IsEnabled, Is.True);
    }
    [Test]
    public void TestInitOnMicDevChangedSubscription4()
    {
        settings.MicrophoneDevice = null;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, false);
        settings.MicrophoneDevice = "default";
        Assert.That(recorder.IsEnabled, Is.False);
    }
    [Test]
    public void TestInitOnMicDevChangedSubscription5()
    {
        settings.MicrophoneDevice = null;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, false);
        settings.MicrophoneDevice = "default";
        Assert.That(Microphone.IsRecording("default"), Is.False);
    }
    [Test]
    public void TestInitOnMicDevChangedSubscription6()
    {
        settings.MicrophoneDevice = null;
        recAwake.Invoke(recorder, new object[0]);
        recIsEnabled.SetValue(recorder, true);
        settings.MicrophoneDevice = "default";
        Assert.That(Microphone.IsRecording("default"), Is.True);
    }
    [Test]
    public void TestOnMicDevChanged()
    {
        settings.MicrophoneDevice = "default";
        recIsEnabled.SetValue(recorder, true);
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        Assert.That(recClip.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestOnMicDevChanged2()
    {
        settings.MicrophoneDevice = "default";
        recIsEnabled.SetValue(recorder, false);
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        Assert.That(recClip.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestOnMicDevChanged3()
    {
        settings.MicrophoneDevice = "default";
        recIsEnabled.SetValue(recorder, true);
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        Assert.That(recorder.IsEnabled, Is.True);
    }
    [Test]
    public void TestOnMicDevChanged5()
    {
        settings.MicrophoneDevice = "default";
        recIsEnabled.SetValue(recorder, true);
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
    }
    [Test]
    public void TestOnMicDevChanged4()
    {
        settings.MicrophoneDevice = "default";
        recIsEnabled.SetValue(recorder, false);
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        Assert.That(recorder.IsEnabled, Is.False);
    }
    [Test]
    public void TestOnMicDevChanged6()
    {
        settings.MicrophoneDevice = "default";
        recIsEnabled.SetValue(recorder, false);
        recOnMicDeviceChanged.Invoke(recorder, new object[] { null });
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
    }
    [Test]
    public void TestStartRecordingClipInit()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That(recClip.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestStartRecordingBufferInit()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That(recCyclicAudioBuffer.GetValue(recorder), Is.Not.Null);
    }
    [Test]
    public void TestStartRecordingIsEnabled()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That(recorder.IsEnabled, Is.True);
    }
    [Test]
    public void TestStartRecordingIsRecording()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
    }
    [Test]
    public void TestStartRecordingClipChannels()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That((recClip.GetValue(recorder) as AudioClip).channels, Is.EqualTo(1));
    }
    [Test]
    public void TestStartRecordingIndexesSet()
    {
        recAwake.Invoke(recorder, new object[0]);
        recWriteIndex.SetValue(recorder, 5);
        recReadIndex.SetValue(recorder, 11);
        recorder.StartRecording();
        Assert.That(recWriteIndex.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestStartRecordingIndexesSet2()
    {
        recAwake.Invoke(recorder, new object[0]);
        recWriteIndex.SetValue(recorder, 5);
        recReadIndex.SetValue(recorder, 11);
        recorder.StartRecording();
        Assert.That(recReadIndex.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestStartRecordingClipFreqRedLight()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That((recClip.GetValue(recorder) as AudioClip).frequency, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestStartRecordingClipDuration()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        Assert.That((recClip.GetValue(recorder) as AudioClip).length, Is.EqualTo(1).Within(0.00001));
    }
    [Test]
    public void TestStopRecordingClipNull()
    {
        LogAssert.ignoreFailingMessages = true;
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        recorder.StopRecording();
        Assert.That(recClip.GetValue(recorder), Is.Null);
    }
    [Test]
    public void TestStopRecordingIsEnabled()
    {
        LogAssert.ignoreFailingMessages = true;
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        recorder.StopRecording();
        Assert.That(recorder.IsEnabled, Is.False);
    }
    [Test]
    public void TestStopRecordingIsNotRecording()
    {
        LogAssert.ignoreFailingMessages = true;
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        recorder.StopRecording();
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
    }
    [Test]
    public void TestUpdateEarlyOutDisabled()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        recIsEnabled.SetValue(recorder, false);
        recUpdate.Invoke(recorder, new object[0]);
        Assert.That(recPrevOffset.GetValue(recorder), Is.EqualTo(0));
    }
    [Test]
    public void TestUpdateEarlyOutNotRecording()
    {
        LogAssert.ignoreFailingMessages = true;
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();
        recorder.StopRecording();
        recUpdate.Invoke(recorder, new object[0]);
        Assert.That(recPrevOffset.GetValue(recorder), Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessCount()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(effectiveCount, Is.Not.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessValidPacket()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessReadIndex()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(recReadIndex.GetValue(recorder), Is.Not.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessFormat()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.Format, Is.EqualTo(AudioDataTypeFlag.Single));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessChannels()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.Channels, Is.EqualTo((recClip.GetValue(recorder) as AudioClip).channels));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessFrequency()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.Frequency, Is.EqualTo((recClip.GetValue(recorder) as AudioClip).frequency));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleSuccessNetidDefault()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        float[] data = new float[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.NetId, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleNoDataRecorded()
    {
        recAwake.Invoke(recorder, new object[0]);
        float[] data = new float[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(effectiveCount, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleNoDataRecorded2()
    {
        recAwake.Invoke(recorder, new object[0]);
        float[] data = new float[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleNoSpace()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        float[] data = new float[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);


        VoicePacketInfo info = recorder.GetMicData(data, 1000, 1000, out effectiveCount);
        Assert.That(effectiveCount, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleNoSpace2()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        float[] data = new float[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);


        VoicePacketInfo info = recorder.GetMicData(data, 0, 0, out effectiveCount);
        Assert.That(effectiveCount, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleNoSpace4()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        float[] data = new float[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);


        VoicePacketInfo info = recorder.GetMicData(data, 1000, 1000, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataSingleNoSpace3()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        float[] data = new float[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);


        VoicePacketInfo info = recorder.GetMicData(data, 0, 0, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessCount()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(effectiveCount, Is.Not.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessValidPacket()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessReadIndex()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(recReadIndex.GetValue(recorder), Is.Not.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessFormat()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.Format, Is.EqualTo(AudioDataTypeFlag.Int16));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessChannels()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.Channels, Is.EqualTo((recClip.GetValue(recorder) as AudioClip).channels));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessFrequency()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.Frequency, Is.EqualTo((recClip.GetValue(recorder) as AudioClip).frequency));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16SuccessNetidDefault()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        byte[] data = new byte[1000];
        int effectiveCount;
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.NetId, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16NoDataRecorded()
    {
        recAwake.Invoke(recorder, new object[0]);
        byte[] data = new byte[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;

        recUpdate.Invoke(recorder, new object[0]);
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(effectiveCount, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16NoDataRecorded2()
    {
        recAwake.Invoke(recorder, new object[0]);
        byte[] data = new byte[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;

        recUpdate.Invoke(recorder, new object[0]);
        VoicePacketInfo info = recorder.GetMicData(data, 0, 1000, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16NoSpace()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        byte[] data = new byte[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        VoicePacketInfo info = recorder.GetMicData(data, 1000, 1000, out effectiveCount);
        Assert.That(effectiveCount, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16NoSpace2()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        byte[] data = new byte[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        VoicePacketInfo info = recorder.GetMicData(data, 0, 0, out effectiveCount);
        Assert.That(effectiveCount, Is.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16NoSpace4()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        byte[] data = new byte[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        VoicePacketInfo info = recorder.GetMicData(data, 1000, 1000, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [UnityTest]
    public IEnumerator TestGetMicDataInt16NoSpace3()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        byte[] data = new byte[1000];
        int effectiveCount;

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        VoicePacketInfo info = recorder.GetMicData(data, 0, 0, out effectiveCount);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [UnityTest]
    public IEnumerator TestUpdateSuccessPrevOffset()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        Assert.That(recPrevOffset.GetValue(recorder), Is.Not.EqualTo(0));
    }
    [UnityTest]
    public IEnumerator TestUpdateSuccessWriteIndex()
    {
        recAwake.Invoke(recorder, new object[0]);
        recorder.StartRecording();

        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);
        yield return null;
        recUpdate.Invoke(recorder, new object[0]);

        Assert.That(recWriteIndex.GetValue(recorder), Is.Not.EqualTo(0));
    }
}