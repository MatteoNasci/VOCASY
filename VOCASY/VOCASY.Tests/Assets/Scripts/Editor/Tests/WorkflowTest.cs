using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using VOCASY.Common;
using System.Reflection;
using VOCASY;
using GENUtility;
using System.IO;

[TestFixture]
[TestOf(typeof(Workflow))]
[Category("VOCASY")]
public class WorkflowTest
{
    Workflow workflow;
    Settings settings;
    SupportManipulator manipulator;
    SupportTransport transport;
    GameObject go1;
    GameObject go2;
    SupportHandler handler1;
    SupportHandler handler2;

    object[] empty;
    byte[] dataReceived;
    byte[] dataRecordedInt16;
    float[] dataRecordedSingle;

    MethodInfo workflowOnDisable;
    MethodInfo workflowAwake;
    FieldInfo workflowActiveIdsToSendTo;
    FieldInfo workflowHandlers;
    FieldInfo workflowDataBufferInt16;
    FieldInfo workflowDataBuffer;
    FieldInfo workflowPacketBuffer;
    FieldInfo workflowMutedIds;

    #region Setup

    [OneTimeSetUp]
    public void SetupReflections()
    {
        empty = new object[0];

        Type type = typeof(Workflow);
        workflowOnDisable = type.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowAwake = type.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowDataBuffer = type.GetField("dataBuffer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowDataBufferInt16 = type.GetField("dataBufferInt16", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowPacketBuffer = type.GetField("packetBuffer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowHandlers = type.GetField("handlers", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowMutedIds = type.GetField("mutedIds", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        workflowActiveIdsToSendTo = type.GetField("activeIdsToSendTo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
        string path1 = workflow.SavedDataFilePath;
        string path2 = settings.SavedCustomValuesPath;

        ScriptableObject.DestroyImmediate(workflow);
        ScriptableObject.DestroyImmediate(settings);
        ScriptableObject.DestroyImmediate(manipulator);
        ScriptableObject.DestroyImmediate(transport);

        GameObject.DestroyImmediate(go1);
        GameObject.DestroyImmediate(go2);

        if (File.Exists(path1))
            File.Delete(path1);
        if (File.Exists(path2))
            File.Delete(path2);

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
    [Test]
    public void TestRemoveHandlerActiveIds()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(0));
    }
    [Test]
    public void TestRemoveHandlerActiveIdsRedLight()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.Not.EqualTo(1));
    }
    [Test]
    public void TestRemoveHandlerActiveIds2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler1);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestRemoveHandlerActiveIdsRedLight2()
    {
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler1);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.Not.EqualTo(0));
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

    [Test]
    public void TestAwakeSavedDataFolderPath()
    {
        workflowAwake.Invoke(workflow, empty);
        Assert.That(workflow.SavedDataFolderPath.Equals(Path.Combine(Application.persistentDataPath, workflow.FolderName)), Is.True);
    }
    [Test]
    public void TestAwakeSavedDataFilePath()
    {
        workflowAwake.Invoke(workflow, empty);
        Assert.That(workflow.SavedDataFilePath.Equals(Path.Combine(Application.persistentDataPath, Path.Combine(workflow.FolderName, workflow.FileName))), Is.True);
    }
    [Test]
    public void TestSavedDataFolderPath()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FolderName = "Ciaone";
        Assert.That(workflow.SavedDataFolderPath.Equals(Path.Combine(Application.persistentDataPath, "Ciaone")), Is.True);
    }
    [Test]
    public void TestSavedDataFilePath()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FileName = "Pippo.txt";
        Assert.That(workflow.SavedDataFilePath.Equals(Path.Combine(Application.persistentDataPath, Path.Combine(workflow.FolderName, "Pippo.txt"))), Is.True);
    }
    [Test]
    public void TestSavedDataFolderPath2()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FolderName = "Ciaone";
        Assert.That(File.Exists(workflow.SavedDataFilePath), Is.True);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestSavedDataFilePath2()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FileName = "Pippo.txt";
        Assert.That(File.Exists(workflow.SavedDataFilePath), Is.True);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestSavedDataFolderPath3()
    {
        workflowAwake.Invoke(workflow, empty);
        string s = workflow.SavedDataFilePath;
        workflow.FolderName = "Ciaone";
        Assert.That(File.Exists(s), Is.False);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestSavedDataFilePath3()
    {
        workflowAwake.Invoke(workflow, empty);
        string s = workflow.SavedDataFilePath;
        workflow.FileName = "Pippo.txt";
        Assert.That(File.Exists(s), Is.False);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestSavedDataFolderPath4()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FolderName = "Ciaone";
        string s = workflow.SavedDataFilePath;
        workflow.FolderName = "Ciaone2";
        Assert.That(File.Exists(s), Is.False);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestSavedDataFilePath4()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FileName = "Pippo.txt";
        string s = workflow.SavedDataFilePath;
        workflow.FileName = "Pippo2.txt";
        Assert.That(File.Exists(s), Is.False);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestClearSavedFiles()
    {
        workflowAwake.Invoke(workflow, empty);
        workflow.FileName = "Pippo.txt";
        workflow.ClearSavedStatusesFiles();
        Assert.That(File.Exists(workflow.SavedDataFilePath), Is.False);
    }
    [Test]
    public void TestSave()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        workflow.SaveCurrentMuteStatuses();
        string text = File.ReadAllText(workflow.SavedDataFilePath);
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.SaveCurrentMuteStatuses();
        string text2 = File.ReadAllText(workflow.SavedDataFilePath);
        Assert.That(text2.Equals(text), Is.False);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestSave2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        workflow.SaveCurrentMuteStatuses();
        string text = File.ReadAllText(workflow.SavedDataFilePath);
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.SaveCurrentMuteStatuses();
        string text2 = File.ReadAllText(workflow.SavedDataFilePath);
        Assert.That(text2.Equals(text), Is.True);
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestLoad()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.SaveCurrentMuteStatuses();
        File.ReadAllText(workflow.SavedDataFilePath);

        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.LocalHasMutedRemote));
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestLoad2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.SaveCurrentMuteStatuses();
        File.ReadAllText(workflow.SavedDataFilePath);
        handler2.IsSelfOutputMuted = false;
        workflow.IsHandlerMuted(handler2);

        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.None));
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestLoad3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.SaveCurrentMuteStatuses();
        File.ReadAllText(workflow.SavedDataFilePath);
        handler2.IsSelfOutputMuted = false;
        workflow.IsHandlerMuted(handler2);
        workflow.LoadSavedMuteStatuses();

        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.LocalHasMutedRemote));
        File.Delete(workflow.SavedDataFilePath);
    }
    [Test]
    public void TestAddHandlerActiveSendToAdd()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        Assert.That(workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>, Is.Empty);
    }
    [Test]
    public void TestAddHandlerActiveSendToAdd2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        Assert.That(workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>, Is.Not.Empty);
    }
    [Test]
    public void TestAddHandlerActiveSendToAdd3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestAddHandlerActiveSendToAdd4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>)[0], Is.EqualTo(2));
    }
    [Test]
    public void TestAddHandlerActiveSendToAdd5()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.ProcessIsMutedMessage(true, 2);
        workflow.AddVoiceHandler(handler2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(0));
    }
    [Test]
    public void TestAddHandlerIsHandlerMutedUpdate()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(2, MuteStatus.LocalHasMutedRemote);
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestAddHandlerIsHandlerMutedUpdate2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(2, MuteStatus.None);
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestAddHandlerIsHandlerMutedUpdate3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestAddHandlerIsHandlerMutedUpdate4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestAddHandlerIsHandlerMutedUpdate5()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedAddMuteId()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestIsHandlerMutedAddMuteId2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 0f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        workflow.IsHandlerMuted(handler2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.None));
    }
    [Test]
    public void TestIsHandlerMuted()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        workflow.AddVoiceHandler(handler2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestIsHandlerMuted2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.LocalHasMutedRemote));
    }
    [Test]
    public void TestIsHandlerMuted3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        workflow.AddVoiceHandler(handler2);
        workflow.IsHandlerMuted(handler2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.None));
    }
    [Test]
    public void TestIsHandlerMutedDiffSendMessage()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(2, MuteStatus.None);
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedDiffSendMessage2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(2, MuteStatus.LocalHasMutedRemote);
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedDiffSendMessage3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(2, MuteStatus.LocalHasMutedRemote);
        workflow.AddVoiceHandler(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedEarlyOut()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = false;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(2, MuteStatus.LocalHasMutedRemote);
        workflow.IsHandlerMuted(handler2);
        Assert.That(transport.MessageSent, Is.False);
    }
    [Test]
    public void TestIsHandlerMutedEarlyOut2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler1.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler1.IsSelfOutputMuted = false;
        (workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Add(1, MuteStatus.LocalHasMutedRemote);
        workflow.AddVoiceHandler(handler1);
        Assert.That(transport.MessageSent, Is.False);
    }
    [Test]
    public void TestInitializeActiveIds()
    {
        workflowActiveIdsToSendTo.SetValue(workflow, null);
        workflow.Initialize();
        Assert.That(workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>, Is.Empty);
    }
    [Test]
    public void TestInitializeMuteStat()
    {
        workflowMutedIds.SetValue(workflow, null);
        workflow.Initialize();
        Assert.That(workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>, Is.Empty);
    }
    [Test]
    public void TestInitializeLoad()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.SaveCurrentMuteStatuses();
        workflowMutedIds.SetValue(workflow, null);
        workflow.Initialize();
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.LocalHasMutedRemote));
    }
    [Test]
    public void TestOnDisableActiveIds()
    {
        workflow.Initialize();
        workflowOnDisable.Invoke(workflow, empty);
        Assert.That(workflowActiveIdsToSendTo.GetValue(workflow), Is.Null);
    }
    [Test]
    public void TestOnDisableMuteIds()
    {
        workflow.Initialize();
        workflowOnDisable.Invoke(workflow, empty);
        Assert.That(workflowMutedIds.GetValue(workflow), Is.Null);
    }
    [Test]
    public void TestOnDisableSaveMuteIds()
    {
        workflow.Initialize();

        if (File.Exists(workflow.SavedDataFilePath))
            File.Delete(workflow.SavedDataFilePath);

        workflowOnDisable.Invoke(workflow, empty);
        Assert.That(File.Exists(workflow.SavedDataFilePath), Is.True);
    }
    [Test]
    public void TestProcessIsMutedMessage()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.ProcessIsMutedMessage(false, 5);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestProcessIsMutedMessage2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.ProcessIsMutedMessage(true, 2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.RemoteHasMutedLocal));
    }
    [Test]
    public void TestProcessIsMutedMessage3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(true, 2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.RemoteHasMutedLocal | MuteStatus.LocalHasMutedRemote));
    }
    [Test]
    public void TestProcessIsMutedMessage4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(false, 2);
        Assert.That((workflowMutedIds.GetValue(workflow) as Dictionary<ulong, MuteStatus>)[2], Is.EqualTo(MuteStatus.LocalHasMutedRemote));
    }
    [Test]
    public void TestProcessIsMutedMessageActiveIds()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(true, 2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(0));
    }
    [Test]
    public void TestProcessIsMutedMessageActiveIds2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(false, 2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(1));
    }
    [Test]
    public void TestProcessIsMutedMessageActiveIds3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(false, 2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>)[0], Is.EqualTo(2));
    }
    [Test]
    public void TestProcessIsMutedMessageActiveIds4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(false, 2);
        workflow.ProcessIsMutedMessage(true, 2);
        Assert.That((workflowActiveIdsToSendTo.GetValue(workflow) as List<ulong>).Count, Is.EqualTo(0));
    }
    [Test]
    public void TestGetMuteStatus()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.AddVoiceHandler(handler1);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        workflow.ProcessIsMutedMessage(false, 2);
        Assert.That(workflow.GetMuteStatus(2), Is.EqualTo(MuteStatus.LocalHasMutedRemote));
    }
    [Test]
    public void TestGetMuteStatus2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.ProcessIsMutedMessage(true, 10);
        Assert.That(workflow.GetMuteStatus(10), Is.EqualTo(MuteStatus.RemoteHasMutedLocal));
    }
    [Test]
    public void TestGetMuteStatus3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler2);
        workflow.ProcessIsMutedMessage(true, 2);
        handler2.IsSelfOutputMuted = true;
        workflow.IsHandlerMuted(handler2);
        Assert.That(workflow.GetMuteStatus(2), Is.EqualTo(MuteStatus.Both));
    }
    [Test]
    public void TestGetCurrentTrackedIds()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedIds(arr);
        Assert.That(arr[0], Is.EqualTo(1));
    }
    [Test]
    public void TestGetCurrentTrackedIds2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedIds(arr);
        Assert.That(arr[1], Is.EqualTo(2));
    }
    [Test]
    public void TestGetCurrentTrackedIds3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler1);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedIds(arr);
        Assert.That(arr[0], Is.EqualTo(2));
    }
    [Test]
    public void TestGetCurrentTrackedIds4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler1);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedIds(arr);
        Assert.That(arr[1], Is.EqualTo(0));
    }
    [Test]
    public void TestGetCurrentActiveTrackedIds()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedActiveIds(arr);
        Assert.That(arr[0], Is.EqualTo(2));
    }
    [Test]
    public void TestGetCurrentActiveTrackedIds2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedActiveIds(arr);
        Assert.That(arr[1], Is.EqualTo(0));
    }
    [Test]
    public void TestGetCurrentActiveTrackedIds3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedActiveIds(arr);
        Assert.That(arr[0], Is.EqualTo(0));
    }
    [Test]
    public void TestGetCurrentActiveTrackedIds4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedActiveIds(arr);
        Assert.That(arr[1], Is.EqualTo(0));
    }
    [Test]
    public void TestGetCurrentActiveTrackedIds5()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.ProcessIsMutedMessage(true, 2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedActiveIds(arr);
        Assert.That(arr[0], Is.EqualTo(0));
    }
    [Test]
    public void TestGetCurrentActiveTrackedIds6()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler1);
        workflow.ProcessIsMutedMessage(true, 2);
        ulong[] arr = new ulong[2];
        workflow.GetCurrentTrackedActiveIds(arr);
        Assert.That(arr[1], Is.EqualTo(0));
    }
    [Test]
    public void TestGetTrackedHandlerById()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        Assert.That(workflow.GetTrackedHandlerById(2), Is.SameAs(handler2));
    }
    [Test]
    public void TestGetTrackedHandlerById2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        Assert.That(workflow.GetTrackedHandlerById(1), Is.SameAs(handler1));
    }
    [Test]
    public void TestGetTrackedHandlerById3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        Assert.That(workflow.GetTrackedHandlerById(5), Is.Null);
    }
    [Test]
    public void TestGetTrackedHandlerById4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        workflow.AddVoiceHandler(handler1);
        workflow.AddVoiceHandler(handler2);
        workflow.RemoveVoiceHandler(handler1);
        Assert.That(workflow.GetTrackedHandlerById(1), Is.Null);
    }
    [Test]
    public void TestIsHandlerMutedMsgSentCondition()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        transport.MessageSent = false;
        workflow.IsHandlerMuted(handler2);
        Assert.That(transport.MessageSent, Is.False);
    }
    [Test]
    public void TestIsHandlerMutedMsgSentCondition2()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        transport.MessageSent = false;
        workflow.IsHandlerMuted(handler2, false);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedMsgSentCondition3()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        transport.MessageSent = false;
        handler2.IsSelfOutputMuted = false;
        workflow.IsHandlerMuted(handler2);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedMsgSentCondition4()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        transport.MessageSent = false;
        handler2.IsSelfOutputMuted = false;
        workflow.IsHandlerMuted(handler2, false);
        Assert.That(transport.MessageSent, Is.True);
    }
    [Test]
    public void TestIsHandlerMutedMsgSentCondition5()
    {
        workflowAwake.Invoke(workflow, empty);
        manipulator.Flag = AudioDataTypeFlag.Both;
        workflow.Initialize();
        handler1.Flag = AudioDataTypeFlag.Both;
        handler2.Flag = AudioDataTypeFlag.Both;
        handler2.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        handler2.IsSelfOutputMuted = true;
        workflow.AddVoiceHandler(handler2);
        transport.MessageSent = false;
        workflow.IsHandlerMuted(handler2, false);
        transport.MessageSent = false;
        workflow.IsHandlerMuted(handler2, false);
        transport.MessageSent = false;
        workflow.IsHandlerMuted(handler2, false);
        transport.MessageSent = false;
        workflow.IsHandlerMuted(handler2, false);
        Assert.That(transport.MessageSent, Is.True);
    }
}