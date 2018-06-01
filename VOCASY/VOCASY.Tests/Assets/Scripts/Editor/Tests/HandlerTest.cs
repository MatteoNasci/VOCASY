using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VOCASY.Common;
using UnityEngine;
using System.Reflection;
using UnityEngine.TestTools;
using VOCASY;
using GENUtility;
[TestFixture]
[TestOf(typeof(Handler))]
[Category("VOCASY")]
public class HandlerTest
{
    GameObject go;
    Handler handler;
    Recorder recorder;
    Receiver receiver;
    SupportWorkflow workflow;
    SupportSettings settings;

    FieldInfo handlerInitialized;

    MethodInfo handlerPTTOffUpdate;
    MethodInfo handlerPTTOnUpdate;
    MethodInfo handlerOnVoiceChatEnabledChanged;
    MethodInfo handlerInitUpdate;
    MethodInfo handlerNormalUpdate;

    MethodInfo handlerUpdate;
    MethodInfo handlerOnEnable;
    MethodInfo handlerOnDisable;
    MethodInfo handlerAwake;

    MethodInfo recorderAwake;

    object[] empty;
    //MethodInfo handlerOnDestroy;
    [OneTimeSetUp]
    public void OneTimeSetupReflections()
    {
        empty = new object[0];
        Type t = typeof(Handler);
        handlerInitialized = t.GetField("initialized", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerPTTOffUpdate = t.GetMethod("PTTOffUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerPTTOnUpdate = t.GetMethod("PTTOnUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerOnVoiceChatEnabledChanged = t.GetMethod("OnVoiceChatEnabledChanged", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerInitUpdate = t.GetMethod("InitUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerNormalUpdate = t.GetMethod("NormalUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerUpdate = t.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerOnEnable = t.GetMethod("OnEnable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerOnDisable = t.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        handlerAwake = t.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //handlerOnDestroy = t.GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        recorderAwake = typeof(Recorder).GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    [SetUp]
    public void SetupVoiceHandler()
    {
        go = new GameObject();
        workflow = ScriptableObject.CreateInstance<SupportWorkflow>();
        settings = ScriptableObject.CreateInstance<SupportSettings>();
        workflow.Settings = settings;
        recorder = go.AddComponent<Recorder>();
        recorder.Settings = settings;
        receiver = go.AddComponent<Receiver>();
        receiver.Settings = settings;
        handler = go.AddComponent<Handler>();
        handler.Recorder = recorder;
        handler.Receiver = receiver;
        handler.Workflow = workflow;
    }
    [TearDown]
    public void TeardownVoiceHandler()
    {
        workflow.Settings = null;
        handler.Workflow = null;
        handler.Recorder = null;
        handler.Receiver = null;
        GameObject.DestroyImmediate(go);
        ScriptableObject.DestroyImmediate(workflow);
        ScriptableObject.DestroyImmediate(settings);
    }
    [Test]
    public void TestAvailableTypesInitValue()
    {
        handlerAwake.Invoke(handler, empty);
        Assert.That(handler.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.None));
    }
    [Test]
    public void TestIsInitializedValue()
    {
        handlerInitialized.SetValue(handler, true);
        Assert.That(handler.IsInitialized, Is.True);
    }
    [Test]
    public void TestIsInitializedValue2()
    {
        handlerInitialized.SetValue(handler, false);
        Assert.That(handler.IsInitialized, Is.False);
    }
    [Test]
    public void TestInitInitialized()
    {
        handlerInitialized.SetValue(handler, true);
        handlerAwake.Invoke(handler, empty);
        Assert.That(handler.IsInitialized, Is.False);
    }
    [Test]
    public void TestInitRecorderEnabled()
    {
        recorder.enabled = true;
        handlerAwake.Invoke(handler, empty);
        Assert.That(recorder.enabled, Is.False);
    }
    [Test]
    public void TestInitReceiverEnabled()
    {
        receiver.enabled = true;
        handlerAwake.Invoke(handler, empty);
        Assert.That(receiver.enabled, Is.False);
    }
    [Test]
    public void TestInitVoiceChatEnabled()
    {
        settings.VoiceChatEnabled = true;
        handler.enabled = false;
        handlerAwake.Invoke(handler, empty);
        Assert.That(handler.enabled, Is.True);
    }
    [Test]
    public void TestInitVoiceChatEnabled2()
    {
        settings.VoiceChatEnabled = false;
        handler.enabled = true;
        handlerAwake.Invoke(handler, empty);
        Assert.That(handler.enabled, Is.False);
    }
    [Test]
    public void TestNetIDValue()
    {
        handler.Identity = new NetworkIdentity();
        handler.Identity.NetworkId = 789;
        Assert.That(handler.NetID, Is.EqualTo(789));
    }
    [Test]
    public void TestNetIDValue2()
    {
        handler.Identity = new NetworkIdentity();
        handler.Identity.NetworkId = 7189;
        Assert.That(handler.NetID, Is.EqualTo(7189));
    }
    [Test]
    public void TestIsRecorderValue()
    {
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        Assert.That(handler.IsRecorder, Is.True);
    }
    [Test]
    public void TestIsRecorderValue2()
    {
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = false;
        Assert.That(handler.IsRecorder, Is.False);
    }
    [Test]
    public void TestOnVoiceChatEnabledChanged()
    {
        settings.VoiceChatEnabled = false;
        handlerAwake.Invoke(handler, empty);
        handler.enabled = false;
        settings.VoiceChatEnabled = true;
        Assert.That(handler.enabled, Is.True);
    }
    [Test]
    public void TestOnVoiceChatEnabledChanged2()
    {
        settings.VoiceChatEnabled = true;
        handlerAwake.Invoke(handler, empty);
        handler.enabled = true;
        settings.VoiceChatEnabled = false;
        Assert.That(handler.enabled, Is.False);
    }
    [Test]
    public void TestOnEnableNonInit()
    {
        settings.VoiceChatEnabled = true;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handlerOnEnable.Invoke(handler, empty);
        Assert.That(workflow.Handlers.Count, Is.EqualTo(0));
    }
    [Test]
    public void TestOnEnableNonInit2()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        recorder.enabled = false;
        handlerInitialized.SetValue(handler, false);
        handlerOnEnable.Invoke(handler, empty);
        Assert.That(recorder.enabled, Is.False);
    }
    [Test]
    public void TestOnEnableNonInit3()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        receiver.enabled = true;
        handlerInitialized.SetValue(handler, false);
        handlerOnEnable.Invoke(handler, empty);
        Assert.That(receiver.enabled, Is.True);
    }
    [Test]
    public void TestOnEnableInit()
    {
        settings.VoiceChatEnabled = true;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handlerOnEnable.Invoke(handler, empty);
        Assert.That(workflow.Handlers.Count, Is.EqualTo(1));
    }
    [Test]
    public void TestOnEnableInit2()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        recorder.enabled = false;
        handlerInitialized.SetValue(handler, true);
        handlerOnEnable.Invoke(handler, empty);
        Assert.That(recorder.enabled, Is.True);
    }
    [Test]
    public void TestOnEnableInit3()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        receiver.enabled = true;
        handlerInitialized.SetValue(handler, true);
        handlerOnEnable.Invoke(handler, empty);
        Assert.That(receiver.enabled, Is.False);
    }
    [Test]
    public void TestOnDisableNonInit()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        workflow.AddVoiceHandler(handler);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(workflow.Handlers.Count, Is.EqualTo(1));
    }
    [Test]
    public void TestOnDisableNonInit2()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        recorder.enabled = true;
        handlerInitialized.SetValue(handler, false);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(recorder.enabled, Is.True);
    }
    [Test]
    public void TestOnDisableNonInit3()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        receiver.enabled = true;
        handlerInitialized.SetValue(handler, false);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(receiver.enabled, Is.True);
    }
    [Test]
    public void TestOnDisableNonInit4()
    {
        LogAssert.ignoreFailingMessages = true;
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerInitialized.SetValue(handler, false);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    [Test]
    public void TestOnDisableNonInit5()
    {
        LogAssert.ignoreFailingMessages = true;
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = false;
        handlerAwake.Invoke(handler, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerInitialized.SetValue(handler, false);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    [Test]
    public void TestOnDisableInit()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        workflow.AddVoiceHandler(handler);
        handlerInitialized.SetValue(handler, true);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(workflow.Handlers.Count, Is.EqualTo(0));
    }
    [Test]
    public void TestOnDisableInit2()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        recorder.enabled = true;
        handlerInitialized.SetValue(handler, true);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(recorder.enabled, Is.False);
    }
    [Test]
    public void TestOnDisableInit3()
    {
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        receiver.enabled = true;
        handlerInitialized.SetValue(handler, true);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(receiver.enabled, Is.False);
    }
    [Test]
    public void TestOnDisableInit4()
    {
        LogAssert.ignoreFailingMessages = true;
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = true;
        handlerAwake.Invoke(handler, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerInitialized.SetValue(handler, true);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
        recorder.StopRecording();
    }
    [Test]
    public void TestOnDisableInit5()
    {
        LogAssert.ignoreFailingMessages = true;
        settings.VoiceChatEnabled = true;
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsLocalPlayer = false;
        handlerAwake.Invoke(handler, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerInitialized.SetValue(handler, true);
        handlerOnDisable.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    [Test]
    public void TestPTTOffUpdate()
    {
        LogAssert.ignoreFailingMessages = true;
        recorderAwake.Invoke(recorder, empty);
        recorder.StopRecording();
        handlerPTTOffUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    [Test]
    public void TestPTTOffUpdate2()
    {
        LogAssert.ignoreFailingMessages = true;
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerPTTOffUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    [Test]
    public void TestPTTOnUpdate()
    {
        LogAssert.ignoreFailingMessages = true;
        recorderAwake.Invoke(recorder, empty);
        recorder.StopRecording();
        handlerPTTOnUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
        recorder.StopRecording();
    }
    [Test]
    public void TestPTTOnUpdate2()
    {
        LogAssert.ignoreFailingMessages = true;
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerPTTOnUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
        recorder.StopRecording();
    }
    [Test]
    public void TestPTTOnUpdate3()
    {
        LogAssert.ignoreFailingMessages = true;
        settings.IsPTTOn = () => true;
        recorderAwake.Invoke(recorder, empty);
        recorder.StopRecording();
        handlerPTTOnUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    [Test]
    public void TestPTTOnUpdate4()
    {
        LogAssert.ignoreFailingMessages = true;
        settings.IsPTTOn = () => true;
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        handlerPTTOnUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        recorder.StopRecording();
    }
    //TODO: Update , NormalUpdate , InitUpdate , ReceiveAudioData x2 , GetMicData x2
}