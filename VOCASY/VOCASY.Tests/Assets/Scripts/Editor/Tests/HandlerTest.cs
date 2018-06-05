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
    MethodInfo receiverOnEnable;

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
        receiverOnEnable = typeof(Receiver).GetMethod("OnEnable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    [SetUp]
    public void SetupVoiceHandler()
    {
        go = new GameObject();
        workflow = ScriptableObject.CreateInstance<SupportWorkflow>();
        Transport t = ScriptableObject.CreateInstance<Transport>();
        VoiceDataManipulator m = ScriptableObject.CreateInstance<VoidManipulator>();
        workflow.Transport = t;
        workflow.Manipulator = m;
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
        workflow.Initialize();
    }
    [TearDown]
    public void TeardownVoiceHandler()
    {
        workflow.Settings = null;
        handler.Workflow = null;
        handler.Recorder = null;
        handler.Receiver = null;
        GameObject.DestroyImmediate(go);
        ScriptableObject.DestroyImmediate(workflow.Transport);
        ScriptableObject.DestroyImmediate(workflow.Manipulator);
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
    public void TestOnVoiceChatEnabledChanged3()
    {
        handler.enabled = false;
        settings.VoiceChatEnabled = true;
        handlerOnVoiceChatEnabledChanged.Invoke(handler, empty);
        Assert.That(handler.enabled, Is.True);
    }
    [Test]
    public void TestOnVoiceChatEnabledChanged4()
    {
        handler.enabled = true;
        settings.VoiceChatEnabled = false;
        handlerOnVoiceChatEnabledChanged.Invoke(handler, empty);
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
    [Test]
    public void TestInitUpdateEarlyOut()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = null;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(handlerInitialized.GetValue(handler), Is.False);
    }
    [Test]
    public void TestInitUpdateEarlyOut2()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = false;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(handlerInitialized.GetValue(handler), Is.False);
    }
    [Test]
    public void TestInitUpdateSuccessInitialized()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(handlerInitialized.GetValue(handler), Is.True);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestInitUpdateNullRefException()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Receiver = null;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as NullReferenceException, Is.Not.Null);
        }
        finally
        {
            handler.Receiver = receiver;
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateNullRefException2()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Recorder = null;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as NullReferenceException, Is.Not.Null);
        }
        finally
        {
            handler.Recorder = recorder;
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateArgExceptionCompatibility()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Int16;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Single;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as ArgumentException, Is.Not.Null);
        }
        finally
        {
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateArgExceptionCompatibility2()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Single;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Int16;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as ArgumentException, Is.Not.Null);
        }
        finally
        {
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateArgExceptionCompatibility3()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.None;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Single;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as ArgumentException, Is.Not.Null);
        }
        finally
        {
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateArgExceptionCompatibility4()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.None;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.None;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as ArgumentException, Is.Not.Null);
        }
        finally
        {
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateArgExceptionCompatibility5()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.None;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        try
        {
            handlerInitUpdate.Invoke(handler, empty);
        }
        catch (TargetInvocationException e)
        {
            Assert.That(e.InnerException as ArgumentException, Is.Not.Null);
        }
        finally
        {
            handlerOnDisable.Invoke(handler, empty);
        }
    }
    [Test]
    public void TestInitUpdateAvailableTypesValue()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Int16;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(handler.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Int16));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestInitUpdateAvailableTypesValue2()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(handler.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Both));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestInitUpdateAvailableTypesValue3()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Single;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(handler.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Single));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestInitUpdateOnEnableInvoked()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handlerInitUpdate.Invoke(handler, empty);
        Assert.That(workflow.Handlers.Count, Is.EqualTo(1));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestUpdateInitVersion()
    {
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, false);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        handlerUpdate.Invoke(handler, empty);
        Assert.That(workflow.Handlers.Count, Is.EqualTo(1));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestUpdateNormalVersion()
    {
        SupportReceiver rec = go.AddComponent<SupportReceiver>();
        rec.Volume = 1f;
        handler.Receiver = rec;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = false;
        handler.SelfOutputVolume = 0.5f;
        settings.VoiceChatVolume = 0.5f;
        handlerUpdate.Invoke(handler, empty);
        Assert.That(handler.Receiver.Volume, Is.EqualTo(0.25).Within(0.00001));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestReceiveAudioDataInt16()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = false;
        rece.enabled = false;
        handler.ReceiveAudioDataInt16(null, 0, 0, new VoicePacketInfo() { ValidPacketInfo = true });
        Assert.That(rece.ReceivedInt16, Is.False);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestReceiveAudioDataInt162()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = false;
        rece.enabled = true;
        handler.ReceiveAudioDataInt16(null, 0, 0, new VoicePacketInfo() { ValidPacketInfo = true });
        Assert.That(rece.ReceivedInt16, Is.True);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestReceiveAudioDataSingle()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = false;
        rece.enabled = false;
        handler.ReceiveAudioData(null, 0, 0, new VoicePacketInfo() { ValidPacketInfo = true });
        Assert.That(rece.ReceivedSingle, Is.False);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestReceiveAudioDataSingle2()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = false;
        rece.enabled = true;
        handler.ReceiveAudioData(null, 0, 0, new VoicePacketInfo() { ValidPacketInfo = true });
        Assert.That(rece.ReceivedSingle, Is.True);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt16()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = false;
        reco.MicDataReady = 10;
        byte[] data = new byte[10];
        int count;
        handler.GetMicDataInt16(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(0));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt162()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 0;
        byte[] data = new byte[10];
        int count;
        handler.GetMicDataInt16(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(0));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt163()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = false;
        reco.MicDataReady = 10;
        byte[] data = new byte[10];
        int count;
        Assert.That(handler.GetMicDataInt16(data, 0, 10, out count).ValidPacketInfo, Is.False);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt164()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 10;
        byte[] data = new byte[10];
        int count;
        Assert.That(handler.GetMicDataInt16(data, 0, 10, out count).ValidPacketInfo, Is.True);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt165()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 10;
        byte[] data = new byte[10];
        int count;
        handler.GetMicDataInt16(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(10));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt166()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 5;
        byte[] data = new byte[10];
        int count;
        handler.GetMicDataInt16(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(10));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataInt167()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 3;
        byte[] data = new byte[10];
        int count;
        handler.GetMicDataInt16(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(6));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataSingle()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = false;
        reco.MicDataReady = 10;
        float[] data = new float[10];
        int count;
        handler.GetMicData(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(0));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataSingle2()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 0;
        float[] data = new float[10];
        int count;
        handler.GetMicData(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(0));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataSingle3()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = false;
        reco.MicDataReady = 10;
        float[] data = new float[10];
        int count;
        Assert.That(handler.GetMicData(data, 0, 10, out count).ValidPacketInfo, Is.False);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataSingle4()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 10;
        float[] data = new float[10];
        int count;
        Assert.That(handler.GetMicData(data, 0, 10, out count).ValidPacketInfo, Is.True);
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataSingle5()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 10;
        float[] data = new float[10];
        int count;
        handler.GetMicData(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(10));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestGetMicDataSingle6()
    {
        SupportReceiver rece = go.AddComponent<SupportReceiver>();
        rece.Flag = AudioDataTypeFlag.Both;
        SupportRecorder reco = go.AddComponent<SupportRecorder>();
        reco.Flag = AudioDataTypeFlag.Both;
        handler.Receiver = rece;
        handler.Recorder = reco;
        handlerAwake.Invoke(handler, empty);
        handlerInitialized.SetValue(handler, true);
        handler.Identity = new NetworkIdentity();
        handler.Identity.IsInitialized = true;
        handler.Identity.IsLocalPlayer = true;
        reco.Enabled = true;
        reco.MicDataReady = 5;
        float[] data = new float[10];
        int count;
        handler.GetMicData(data, 0, 10, out count);
        Assert.That(count, Is.EqualTo(5));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestNormalUpdateNotRecorderOutmutMuted()
    {
        //handlerAwake.Invoke(handler, empty);
        //handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = false };
        //handlerUpdate.Invoke(handler, empty);
        //handler.SelfOutputVolume = 0.1f;
        //handler.IsSelfOutputMuted = true;
        //settings.VoiceChatVolume = 0.6f;
        //receiverOnEnable.Invoke(receiver, empty);
        //handlerNormalUpdate.Invoke(handler, empty);
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = false };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That((int)(workflow.HandlersMuteStatuses[1] & MuteStatus.LocalHasMutedRemote), Is.EqualTo(0));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestNormalUpdateNotRecorderOutmutMuted2()
    {
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = false };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = true;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That((int)(workflow.HandlersMuteStatuses[1] & MuteStatus.LocalHasMutedRemote), Is.Not.EqualTo(0));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestNormalUpdateNotRecorderVolume()
    {
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = false };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(receiver.Volume, Is.EqualTo(0.3).Within(0.00001));
        handlerOnDisable.Invoke(handler, empty);
    }
    [Test]
    public void TestNormalUpdateRecorderPTTON()
    {
        LogAssert.ignoreFailingMessages = true;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        settings.PushToTalk = true;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(recorder.IsEnabled, Is.False);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderPTTON2()
    {
        LogAssert.ignoreFailingMessages = true;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StartRecording();
        settings.PushToTalk = true;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.False);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderPTTOff()
    {
        LogAssert.ignoreFailingMessages = true;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StopRecording();
        settings.PushToTalk = false;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(recorder.IsEnabled, Is.True);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderPTTOff2()
    {
        LogAssert.ignoreFailingMessages = true;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);
        recorderAwake.Invoke(recorder, empty);
        recorder.StopRecording();
        settings.PushToTalk = false;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(Microphone.IsRecording(settings.MicrophoneDevice), Is.True);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderProcessData()
    {
        LogAssert.ignoreFailingMessages = true;
        SupportRecorder rec = go.AddComponent<SupportRecorder>();
        handler.Recorder = rec;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);

        rec.Enabled = true;
        rec.MicDataReady = 10;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(workflow.ProcessData, Is.True);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderProcessData2()
    {
        LogAssert.ignoreFailingMessages = true;
        SupportRecorder rec = go.AddComponent<SupportRecorder>();
        handler.Recorder = rec;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);

        rec.Enabled = false;
        rec.MicDataReady = 10;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(workflow.ProcessData, Is.False);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderProcessData3()
    {
        LogAssert.ignoreFailingMessages = true;
        SupportRecorder rec = go.AddComponent<SupportRecorder>();
        handler.Recorder = rec;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = true };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);

        rec.Enabled = true;
        rec.MicDataReady = 0;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(workflow.ProcessData, Is.False);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
    [Test]
    public void TestNormalUpdateRecorderProcessData4()
    {
        LogAssert.ignoreFailingMessages = true;
        SupportRecorder rec = go.AddComponent<SupportRecorder>();
        handler.Recorder = rec;
        handlerAwake.Invoke(handler, empty);
        handler.Identity = new NetworkIdentity() { IsInitialized = true, NetworkId = 1, IsLocalPlayer = false };
        handlerUpdate.Invoke(handler, empty);
        handler.SelfOutputVolume = 0.5f;
        handler.IsSelfOutputMuted = false;
        settings.VoiceChatVolume = 0.6f;
        receiverOnEnable.Invoke(receiver, empty);

        rec.Enabled = true;
        rec.MicDataReady = 10;
        handlerNormalUpdate.Invoke(handler, empty);
        Assert.That(workflow.ProcessData, Is.False);
        handlerOnDisable.Invoke(handler, empty);
        recorder.StopRecording();
    }
}