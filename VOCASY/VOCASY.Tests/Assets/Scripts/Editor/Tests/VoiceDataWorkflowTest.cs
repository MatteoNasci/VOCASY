using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using VOCASY.Common;
using System.Reflection;
using VOCASY;
using GENUtility;
[TestFixture]
[TestOf(typeof(Workflow))]
[Category("VOCASY")]
public class VoiceDataWorkflowTest
{
    Workflow workflow;
    Settings settings;
    SupportManipulator manipulator;
    SupportTransport transport;
    GameObject go1;
    GameObject go2;
    SupportHandler handler1;
    SupportHandler handler2;

    byte[] dataReceived;
    byte[] dataRecordedInt16;
    float[] dataRecordedSingle;

    MethodInfo workflowOnDisable;
    FieldInfo workflowHandlers;
    FieldInfo workflowDataBufferInt16;
    FieldInfo workflowDataBuffer;
    FieldInfo workflowPacketBuffer;

    #region Setup

    [OneTimeSetUp]
    public void SetupReflections()
    {
        Type type = typeof(Workflow);
        workflowOnDisable = type.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowDataBuffer = type.GetField("dataBuffer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowDataBufferInt16 = type.GetField("dataBufferInt16", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowPacketBuffer = type.GetField("packetBuffer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowHandlers = type.GetField("handlers", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    [SetUp]
    public void Setup()
    {
        dataReceived = new byte[100];
        for (int i = 0; i < dataReceived.Length; i++)
        {
            dataReceived[i] = (byte)i;
        }

        dataRecordedInt16 = new byte[100];
        for (int i = 0; i < dataRecordedInt16.Length; i++)
        {
            dataRecordedInt16[i] = (byte)i;
        }
        dataRecordedSingle = new float[100];
        for (int i = 0; i < dataRecordedSingle.Length; i++)
        {
            dataRecordedSingle[i] = i;
        }

        workflow = ScriptableObject.CreateInstance<Workflow>();
        settings = ScriptableObject.CreateInstance<Settings>();
        manipulator = ScriptableObject.CreateInstance<SupportManipulator>();
        transport = ScriptableObject.CreateInstance<SupportTransport>();

        workflow.Settings = settings;
        workflow.Manipulator = manipulator;
        workflow.Transport = transport;

        transport.Workflow = workflow;

        go1 = new GameObject();

        handler1 = go1.AddComponent<SupportHandler>();
        handler1.Workflow = workflow;
        handler1.IsRec = true;
        handler1.ID = 1;

        go2 = new GameObject();

        handler2 = go2.AddComponent<SupportHandler>();
        handler2.Workflow = workflow;
        handler2.IsRec = false;
        handler2.ID = 2;
    }
    [TearDown]
    public void Teardown()
    {
        ScriptableObject.DestroyImmediate(workflow);
        ScriptableObject.DestroyImmediate(settings);
        ScriptableObject.DestroyImmediate(manipulator);
        ScriptableObject.DestroyImmediate(transport);

        GameObject.DestroyImmediate(go1);
        GameObject.DestroyImmediate(go2);

        dataRecordedInt16 = null;
        dataRecordedSingle = null;
        dataReceived = null;
    }
    [OneTimeTearDown]
    public void TeardownReflections()
    {
        workflowHandlers = null;
        workflowOnDisable = null;
        workflowPacketBuffer = null;
        workflowDataBufferInt16 = null;
        workflowDataBuffer = null;
    }

    #endregion

    #region Workflow Init

    [Test]
    public void TestNotInitializedHandlers()
    {
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.Initialize();
        Assert.That(workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>, Is.Empty);
    }
    [Test]
    public void TestNotInitializedPacketBuffer()
    {
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.Initialize();
        Assert.That(workflowPacketBuffer.GetValue(workflow) as BytePacket, Is.Null);
    }
    [Test]
    public void TestNotInitializedDataBufferInt16()
    {
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.Initialize();
        Assert.That(workflowDataBufferInt16.GetValue(workflow) as byte[], Is.Null);
    }
    [Test]
    public void TestNotInitializedDataBuffer()
    {
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.Initialize();
        Assert.That(workflowDataBuffer.GetValue(workflow) as float[], Is.Null);
    }
    [Test]
    public void TestInitializedHandlers()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        Assert.That(workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>, Is.Not.Null);
    }
    [Test]
    public void TestInitializedPacketBuffer()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        Assert.That(workflowPacketBuffer.GetValue(workflow) as BytePacket, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        Assert.That(workflowDataBufferInt16.GetValue(workflow) as byte[], Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBuffer()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        Assert.That(workflowDataBuffer.GetValue(workflow) as float[], Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16FormatInt16()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        Assert.That(workflowDataBufferInt16.GetValue(workflow) as byte[], Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferFormatInt16()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        Assert.That(workflowDataBuffer.GetValue(workflow) as float[], Is.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16FormatSingle()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        Assert.That(workflowDataBufferInt16.GetValue(workflow) as byte[], Is.Null);
    }
    [Test]
    public void TestInitializedDataBufferFormatSingle()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        Assert.That(workflowDataBuffer.GetValue(workflow) as float[], Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16FormatBoth()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        Assert.That(workflowDataBufferInt16.GetValue(workflow) as byte[], Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferFormatBoth()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        Assert.That(workflowDataBuffer.GetValue(workflow) as float[], Is.Not.Null);
    }
    [Test]
    public void TestFinalizeHandlers()
    {
        workflowOnDisable.Invoke(workflow, new object[] { });
        Assert.That(workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>, Is.Null);
    }
    [Test]
    public void TestFinalizeBufferInt16()
    {
        workflowOnDisable.Invoke(workflow, new object[] { });
        Assert.That(workflowDataBuffer.GetValue(workflow) as float[], Is.Null);
    }
    [Test]
    public void TestFinalizeBufferSingle()
    {
        workflowOnDisable.Invoke(workflow, new object[] { });
        Assert.That(workflowDataBufferInt16.GetValue(workflow) as byte[], Is.Null);
    }
    public void TestFinalizePacketBuffer()
    {
        workflowOnDisable.Invoke(workflow, new object[] { });
        Assert.That(workflowPacketBuffer.GetValue(workflow) as BytePacket, Is.Null);
    }
    [Test]
    public void TestAddHandlerCount()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestAddHandlerCountRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>).Count, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestAddHandlerNetId()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>).ContainsKey(1), Is.True);
    }
    [Test]
    public void TestAddHandlerNetIdRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>).ContainsKey(1), Is.Not.False);
    }
    [Test]
    public void TestAddHandlerRef()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>)[1], Is.SameAs(handler1));
    }
    [Test]
    public void TestAddHandlerRefRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>)[1], Is.Not.SameAs(handler2));
    }
    [Test]
    public void TestAddHandlerFlagArgException()
    {
        manipulator.Flag = AudioDataTypeFlag.None;
        workflow.Initialize();
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerFlagArgExceptionRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.None;
        workflow.Initialize();
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestAddHandlerFlagArgException2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.None;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerFlagArgException2RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler2.Flag = AudioDataTypeFlag.None;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestAddHandlerFlagArgException3()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerFlagArgException3RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler2.Flag = AudioDataTypeFlag.Single;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestAddHandlerFlagArgException4()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Int16;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerFlagArgException4RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler2.Flag = AudioDataTypeFlag.Int16;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestAddHandlerFlagArgException5()
    {
        manipulator.Flag = AudioDataTypeFlag.None;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.None;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerFlagArgException5RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.None;
        workflow.Initialize();
        handler2.Flag = AudioDataTypeFlag.None;
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestAddHandlerKeyArgException()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerKeyArgExceptionRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestRemoveHandlerDoubleNoException()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.RemoveVoiceHandler(handler1);
        Assert.DoesNotThrow(() => workflow.RemoveVoiceHandler(handler1));
    }
    [Test]
    public void TestRemoveHandlerDoubleNoExceptionRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler2);
        Assert.DoesNotThrow(() => workflow.RemoveVoiceHandler(handler2));
    }
    [Test]
    public void TestRemoveHandlerCount()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.RemoveVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>).Count, Is.EqualTo(0));
    }
    [Test]
    public void TestRemoveHandlerCountRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.RemoveVoiceHandler(handler1);
        Assert.That((workflowHandlers.GetValue(workflow) as Dictionary<ulong, VoiceHandler>).Count, Is.Not.EqualTo(1));
    }

    #endregion

    #region ProcessReceivedPacket

    [Test]
    public void TestProcessReceivedPacketNullArrayException()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentNullException>(() => workflow.ProcessReceivedPacket(null, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutChatDisabled()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        settings.VoiceChatEnabled = false;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(transport.DataReceived, Is.EqualTo(0));
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutInvalidId()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 3);
        Assert.That(manipulator.FromPacketToAudioInt16, Is.False);
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutInvalidTransportPacket()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = false;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(manipulator.FromPacketToAudioInt16, Is.False);
    }
    [Test]
    public void TestProcessReceivedPacketArgExceptionIncompatibleFormat()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.None;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketArgExceptionIncompatibleFormat2()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketArgExceptionIncompatibleFormat3()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler2.Flag = AudioDataTypeFlag.Single;
        Assert.Throws<ArgumentException>(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketArgExceptionIncompatibleFormat4()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Single;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketArgExceptionIncompatibleFormat5()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Int16;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketValidFormat()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Int16;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.DoesNotThrow(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketValidFormat2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Single;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.DoesNotThrow(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketValidFormat3()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.DoesNotThrow(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketValidFormat4()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Both;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.DoesNotThrow(() => workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2));
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutOutputMuted()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler2.IsSelfOutputMuted = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(manipulator.FromPacketToAudioInt16, Is.False);
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutOutputMuted2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler2.SelfOutputVolume = 0f;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(manipulator.FromPacketToAudioInt16, Is.False);
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutOutputNotReceiver()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 1);
        Assert.That(manipulator.FromPacketToAudioInt16, Is.False);
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutOutputInvalidManipulatorPacket()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        manipulator.Info.ValidPacketInfo = false;
        manipulator.UseInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataInt16, Is.False);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16Path()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataInt16, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16Path2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 2;
        transport.Info.Format = AudioDataTypeFlag.Both;
        transport.Info.Frequency = 24000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataInt16, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16Path3()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Int16;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 2;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 24000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataInt16, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16Path4()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 2;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataInt16, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSinglePath()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataSingle, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSinglePath2()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Single;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataSingle, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSinglePath3()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceiveDataSingle, Is.True);
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedDataInt16[23], Is.EqualTo(23));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedDataInt16[98], Is.EqualTo(98));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData3()
    {
        for (int i = 0; i < dataReceived.Length; i += sizeof(float))
        {
            ByteManipulator.Write(dataReceived, i, (float)(i / 4));
        }
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedData[23], Is.EqualTo(23).Within(0.0001));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData4()
    {
        for (int i = 0; i < dataReceived.Length; i += sizeof(float))
        {
            ByteManipulator.Write(dataReceived, i, (float)(i / 4));
        }
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedData[2], Is.EqualTo(2).Within(0.0001));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData5()
    {
        for (int i = 0; i < dataReceived.Length; i += sizeof(float))
        {
            ByteManipulator.Write(dataReceived, i, (float)(i / 4));
        }
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Both;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedData[23], Is.EqualTo(23).Within(0.0001));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData6()
    {
        for (int i = 0; i < dataReceived.Length; i += sizeof(float))
        {
            ByteManipulator.Write(dataReceived, i, (float)(i / 4));
        }
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Both;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedData[2], Is.EqualTo(2).Within(0.0001));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData7()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Both;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedDataInt16[23], Is.EqualTo(23));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData8()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Both;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedDataInt16[98], Is.EqualTo(98));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16PathValidData9()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(handler2.ReceivedDataInt16[98], Is.EqualTo(98));
    }

    #endregion

    #region ProcessMicData

    [Test]
    public void TestProcessMicDataEarlyOutDisabledChat()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        settings.VoiceChatEnabled = false;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataEarlyOutDisabledChat2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        settings.VoiceChatEnabled = false;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataSingle, Is.False);
    }
    [Test]
    public void TestProcessMicDataEarlyOutInputMuted()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        settings.MuteSelf = true;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataEarlyOutInputMuted2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        settings.MuteSelf = true;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataSingle, Is.False);
    }
    [Test]
    public void TestProcessMicDataEarlyOutNotRecorder()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler2);
        Assert.That(handler2.GetDataSingle, Is.False);
    }
    [Test]
    public void TestProcessMicDataEarlyOutNotRecorder2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler2);
        Assert.That(handler2.GetDataInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataArgExceptionIncompatibleFormats()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler1.Flag = AudioDataTypeFlag.None;
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessMicData(handler1));
    }
    [Test]
    public void TestProcessMicDataArgExceptionIncompatibleFormats2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        manipulator.Flag = AudioDataTypeFlag.None;
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessMicData(handler1));
    }
    [Test]
    public void TestProcessMicDataArgExceptionIncompatibleFormats3()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler1.Flag = AudioDataTypeFlag.Single;
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessMicData(handler1));
    }
    [Test]
    public void TestProcessMicDataArgExceptionIncompatibleFormats4()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler1.Flag = AudioDataTypeFlag.Int16;
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        Assert.Throws<ArgumentException>(() => workflow.ProcessMicData(handler1));
    }
    [Test]
    public void TestProcessMicDataGetData()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataInt16, Is.True);
    }
    [Test]
    public void TestProcessMicDataGetDataRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataSingle, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetData2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataSingle, Is.True);
    }
    [Test]
    public void TestProcessMicDataGetData2RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetData3()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataSingle, Is.True);
    }
    [Test]
    public void TestProcessMicDataGetData3RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetData4()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataSingle, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetData4RedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        workflow.ProcessMicData(handler1);
        Assert.That(handler1.GetDataInt16, Is.True);
    }
    [Test]
    public void TestProcessMicDataGetDataInvalidPacketEarlyOut()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = false;
        workflow.ProcessMicData(handler1);
        Assert.That(manipulator.FromAudioToPacketInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetDataInvalidPacketEarlyOutRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = false;
        workflow.ProcessMicData(handler1);
        Assert.That(manipulator.FromAudioToPacket, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetDataManipulated()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(manipulator.FromAudioToPacketInt16, Is.True);
    }
    [Test]
    public void TestProcessMicDataGetDataManipulatedRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(manipulator.FromAudioToPacket, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetDataManipulated2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(manipulator.FromAudioToPacketInt16, Is.False);
    }
    [Test]
    public void TestProcessMicDataGetDataManipulatedRedLight2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(manipulator.FromAudioToPacket, Is.True);
    }
    [Test]
    public void TestProcessMicDataGetDataManipulatedInvalidPacket()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        manipulator.UseInfo = true;
        manipulator.Info.ValidPacketInfo = false;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSent, Is.EqualTo(0));
    }
    [Test]
    public void TestProcessMicDataGetDataManipulatedInvalidPacketRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        manipulator.UseInfo = true;
        manipulator.Info.ValidPacketInfo = false;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSentTo, Is.Empty);
    }
    [Test]
    public void TestProcessMicDataSuccess()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSent, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestProcessMicDataSuccessRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSentTo[0], Is.EqualTo(2));
    }
    [Test]
    public void TestProcessMicDataSuccess2()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSent, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestProcessMicDataSuccessRedLight2()
    {
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSentTo[0], Is.EqualTo(2));
    }
    [Test]
    public void TestProcessMicDataSuccess3()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSent, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestProcessMicDataSuccessRedLight3()
    {
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Single;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.DataSentTo[0], Is.EqualTo(2));
    }
    [Test]
    public void TestProcessMicDataSuccessValidData()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.SentArray[80], Is.EqualTo(80));
    }
    [Test]
    public void TestProcessMicDataSuccessValidData2()
    {
        for (int i = 0; i < dataRecordedSingle.Length; i++)
        {
            dataRecordedSingle[i] = (float)i;
        }
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(ByteManipulator.ReadSingle(transport.SentArray, 320), Is.EqualTo(80).Within(0.0001));
    }
    [Test]
    public void TestProcessMicDataSuccessValidData3()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Int16;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(transport.SentArray[80], Is.EqualTo(80));
    }
    [Test]
    public void TestProcessMicDataSuccessValidData4()
    {
        for (int i = 0; i < dataRecordedSingle.Length; i++)
        {
            dataRecordedSingle[i] = (float)i;
        }
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.FormatToUse = AudioDataTypeFlag.Single;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(ByteManipulator.ReadSingle(transport.SentArray, 320), Is.EqualTo(80).Within(0.0001));
    }
    [Test]
    public void TestProcessMicDataSuccessValidData5()
    {
        for (int i = 0; i < dataRecordedSingle.Length; i++)
        {
            dataRecordedSingle[i] = (float)i;
        }
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.FormatToUse = AudioDataTypeFlag.Int16;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        transport.Info.Channels = 1;
        transport.Info.Format = AudioDataTypeFlag.Single;
        transport.Info.Frequency = 48000;
        transport.Info.ValidPacketInfo = true;
        handler1.DataRec = dataRecordedSingle;
        handler1.DataRecInt16 = dataRecordedInt16;
        handler1.Info.ValidPacketInfo = true;
        workflow.ProcessMicData(handler1);
        Assert.That(ByteManipulator.ReadSingle(transport.SentArray, 320), Is.EqualTo(80).Within(0.0001));
    }

    #endregion

    //TODO: Controllare tutti i metodi.  activeIdsToSendTo,  SavedDataFilePath1 & 2 ,SavedDataFolderPath , mutedIds , AddVoiceHandler(mutedIds , activeIdsToSendTo) , PRocessMicData (SendToAll) , ProcessIsMutedMessage (early out, activeIdsToSendTo) , IsHandlerMuted (dont remember) , OnDisable (new save system for statuses) , Initialize (activeIdsToSendTo) , RemoveHandler (activeIdsToSendTo) , ProcessIsMutedMessage , IsHandlerMuted , Initialized (mutedIds, UseStoredIdsStatuses) , ClearSavdStatusesFiles , Awake , OnDisable (mutedIds , UseStoredIdsStatuses)
}