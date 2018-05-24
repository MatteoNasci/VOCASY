using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using VOCASY.Common;
using VOCASY;
using GENUtility;
[TestFixture]
[TestOf(typeof(VoiceDataWorkflow))]
[Category("VOCASY/Common")]
public class VoiceDataWorkflowTest
{
    VoiceDataWorkflow workflow;
    VoiceChatSettings settings;
    TestManipulator manipulator;
    TestTransport transport;
    GameObject go1;
    GameObject go2;
    VoiceHandler handler1;
    VoiceHandler handler2;
    Receiver receiver1;
    Receiver receiver2;
    Recorder recorder1;
    Recorder recorder2;

    byte[] dataReceived;
    byte[] dataRecordedInt16;
    float[] dataRecordedSingle;
    [OneTimeSetUp]
    public void OneTimeSetupArrays()
    {
        dataReceived = new byte[100];
        ByteManipulator.Write(dataReceived, 0, (ushort)48000);
        ByteManipulator.Write(dataReceived, sizeof(ushort), (byte)1);
        ByteManipulator.Write(dataReceived, sizeof(ushort) + sizeof(byte), (byte)AudioDataTypeFlag.Int16);
        ByteManipulator.Write(dataReceived, sizeof(ushort) + sizeof(byte) + sizeof(byte), (int)92);
        for (int i = sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(int); i < dataReceived.Length; i++)
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
            dataRecordedSingle[i] = i * 0.1f;
        }
    }
    [OneTimeTearDown]
    public void OneTimeTeardownArrays()
    {
        dataRecordedSingle = null;
        dataRecordedInt16 = null;
        dataReceived = null;
    }
    [SetUp]
    public void Setup()
    {
        workflow = ScriptableObject.CreateInstance<VoiceDataWorkflow>();
        settings = ScriptableObject.CreateInstance<VoiceChatSettings>();
        manipulator = ScriptableObject.CreateInstance<TestManipulator>();
        transport = ScriptableObject.CreateInstance<TestTransport>();

        workflow.Settings = settings;
        workflow.Manipulator = manipulator;
        workflow.Transport = transport;

        manipulator.Flag = AudioDataTypeFlag.Both;
        transport.Workflow = workflow;

        workflow.OnEnable();

        go1 = new GameObject();

        recorder1 = go1.AddComponent<Recorder>();
        receiver1 = go1.AddComponent<Receiver>();
        handler1 = go1.AddComponent<VoiceHandler>();
        handler1.Internal_selfOutputVolume = 1f;
        handler1.Workflow = workflow;
        handler1.Receiver = receiver1;
        handler1.Recorder = recorder1;
        handler1.Identity.IsLocalPlayer = true;
        handler1.Identity.NetworkId = 1;
        handler1.Identity.IsInitialized = true;

        go2 = new GameObject();

        recorder2 = go2.AddComponent<Recorder>();
        receiver2 = go2.AddComponent<Receiver>();
        handler2 = go2.AddComponent<VoiceHandler>();
        handler2.Internal_selfOutputVolume = 1f;
        handler2.Workflow = workflow;
        handler2.Receiver = receiver2;
        handler2.Recorder = recorder2;
        handler2.Identity.IsLocalPlayer = false;
        handler2.Identity.NetworkId = 2;
        handler2.Identity.IsInitialized = true;
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

        workflow = null;
        settings = null;
        manipulator = null;
        transport = null;
        go1 = null;
        go2 = null;
        handler1.Workflow = null;
        handler1.Recorder = null;
        handler1.Receiver = null;
        handler1.Identity = null;
        handler1 = null;
        handler2.Workflow = null;
        handler2.Recorder = null;
        handler2.Receiver = null;
        handler2.Identity = null;
        handler2 = null;
        receiver1 = null;
        receiver2 = null;
        recorder1 = null;
        recorder2 = null;
    }

    #region Workflow Init

    [Test]
    public void TestNotInitializedHandlers()
    {
        workflow.OnDisable();
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.OnEnable();
        Assert.That(workflow.Internal_handlers, Is.Empty);
    }
    [Test]
    public void TestNotInitializedPacketBuffer()
    {
        workflow.OnDisable();
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.OnEnable();
        Assert.That(workflow.Internal_packetBuffer, Is.Null);
    }
    [Test]
    public void TestNotInitializedDataBufferInt16()
    {
        workflow.OnDisable();
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.OnEnable();
        Assert.That(workflow.Internal_dataBufferInt16, Is.Null);
    }
    [Test]
    public void TestNotInitializedDataBuffer()
    {
        workflow.OnDisable();
        workflow.Manipulator = null;
        workflow.Settings = null;
        workflow.Transport = null;
        workflow.OnEnable();
        Assert.That(workflow.Internal_dataBuffer, Is.Null);
    }
    [Test]
    public void TestInitializedHandlers()
    {
        Assert.That(workflow.Internal_handlers, Is.Not.Null);
    }
    [Test]
    public void TestInitializedPacketBuffer()
    {
        Assert.That(workflow.Internal_packetBuffer, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16()
    {
        Assert.That(workflow.Internal_dataBufferInt16, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBuffer()
    {
        Assert.That(workflow.Internal_dataBuffer, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16FormatInt16()
    {
        workflow.OnDisable();
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.OnEnable();
        Assert.That(workflow.Internal_dataBufferInt16, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferFormatInt16()
    {
        workflow.OnDisable();
        manipulator.Flag = AudioDataTypeFlag.Int16;
        workflow.OnEnable();
        Assert.That(workflow.Internal_dataBuffer, Is.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16FormatSingle()
    {
        workflow.OnDisable();
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.OnEnable();
        Assert.That(workflow.Internal_dataBufferInt16, Is.Null);
    }
    [Test]
    public void TestInitializedDataBufferFormatSingle()
    {
        workflow.OnDisable();
        manipulator.Flag = AudioDataTypeFlag.Single;
        workflow.OnEnable();
        Assert.That(workflow.Internal_dataBuffer, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferInt16FormatBoth()
    {
        Assert.That(workflow.Internal_dataBufferInt16, Is.Not.Null);
    }
    [Test]
    public void TestInitializedDataBufferFormatBoth()
    {
        Assert.That(workflow.Internal_dataBuffer, Is.Not.Null);
    }
    [Test]
    public void TestFinalizeHandlers()
    {
        workflow.OnDisable();
        Assert.That(workflow.Internal_handlers, Is.Null);
    }
    [Test]
    public void TestFinalizeBufferInt16()
    {
        workflow.OnDisable();
        Assert.That(workflow.Internal_dataBuffer, Is.Null);
    }
    [Test]
    public void TestFinalizeBufferSingle()
    {
        workflow.OnDisable();
        Assert.That(workflow.Internal_dataBufferInt16, Is.Null);
    }
    [Test]
    public void TestFinalizePacketBuffer()
    {
        workflow.OnDisable();
        Assert.That(workflow.Internal_packetBuffer, Is.Null);
    }
    [Test]
    public void TestAddHandlerCount()
    {
        handler1.Internal_InitUpdate();
        Assert.That(workflow.Internal_handlers.Count, Is.EqualTo(1));
    }
    [Test]
    public void TestAddHandlerCountRedLight()
    {
        handler1.Internal_InitUpdate();
        Assert.That(workflow.Internal_handlers.Count, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestAddHandlerNetId()
    {
        handler1.Internal_InitUpdate();
        Assert.That(workflow.Internal_handlers.ContainsKey(1), Is.True);
    }
    [Test]
    public void TestAddHandlerNetIdRedLight()
    {
        handler1.Internal_InitUpdate();
        Assert.That(workflow.Internal_handlers.ContainsKey(1), Is.Not.False);
    }
    [Test]
    public void TestAddHandlerRef()
    {
        handler1.Internal_InitUpdate();
        Assert.That(workflow.Internal_handlers[1], Is.SameAs(handler1));
    }
    [Test]
    public void TestAddHandlerRefRedLight()
    {
        handler1.Internal_InitUpdate();
        Assert.That(workflow.Internal_handlers[1], Is.Not.SameAs(handler2));
    }
    [Test]
    public void TestAddHandlerFlagArgException()
    {
        workflow.OnDisable();
        manipulator.Flag = AudioDataTypeFlag.None;
        workflow.OnEnable();
        Assert.Throws<ArgumentException>(() => handler1.Internal_InitUpdate());
    }
    [Test]
    public void TestAddHandlerFlagArgExceptionRedLight()
    {
        workflow.OnDisable();
        manipulator.Flag = AudioDataTypeFlag.None;
        workflow.OnEnable();
        Assert.Throws<ArgumentException>(() => handler2.Internal_InitUpdate());
    }
    [Test]
    public void TestAddHandlerKeyArgException()
    {
        handler1.Internal_InitUpdate();
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler1));
    }
    [Test]
    public void TestAddHandlerKeyArgExceptionRedLight()
    {
        handler2.Internal_InitUpdate();
        Assert.Throws<ArgumentException>(() => workflow.AddVoiceHandler(handler2));
    }
    [Test]
    public void TestRemoveHandlerDoubleNoException()
    {
        handler1.Internal_InitUpdate();
        workflow.RemoveVoiceHandler(handler1);
        Assert.DoesNotThrow(() => workflow.RemoveVoiceHandler(handler1));
    }
    [Test]
    public void TestRemoveHandlerDoubleNoExceptionRedLight()
    {
        handler2.Internal_InitUpdate();
        workflow.RemoveVoiceHandler(handler2);
        Assert.DoesNotThrow(() => workflow.RemoveVoiceHandler(handler2));
    }
    [Test]
    public void TestRemoveHandlerCount()
    {
        handler1.Internal_InitUpdate();
        workflow.RemoveVoiceHandler(handler1);
        Assert.That(workflow.Internal_handlers.Count, Is.EqualTo(0));
    }
    [Test]
    public void TestRemoveHandlerCountRedLight()
    {
        handler1.Internal_InitUpdate();
        workflow.RemoveVoiceHandler(handler1);
        Assert.That(workflow.Internal_handlers.Count, Is.Not.EqualTo(1));
    }

    #endregion

    #region ProcessReceivedPacket

    [Test]
    public void TestProcessReceivedPacketEarlyOutChatDisabled()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        settings.VoiceChatEnabled = false;
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.EqualTo(0));
    }
    [Test]
    public void TestProcessReceivedPacketEarlyOutInvalidId()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 3);
        Assert.That(receiver2.Internal_writeIndex, Is.EqualTo(0));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.EqualTo(92));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessInt16RedLight()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.Not.EqualTo(0));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSingle()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        ByteManipulator.Write(dataReceived, 3, (byte)AudioDataTypeFlag.Single);
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.EqualTo(46));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSingleRedLight()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        ByteManipulator.Write(dataReceived, 3, (byte)AudioDataTypeFlag.Single);
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.Not.EqualTo(92));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSingle2()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        ByteManipulator.Write(dataReceived, 3, (byte)AudioDataTypeFlag.Single);
        ByteManipulator.Write(dataReceived, 2, (byte)2);
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.EqualTo(23));
    }
    [Test]
    public void TestProcessReceivedPacketSuccessSingle2RedLight()
    {
        handler1.Internal_InitUpdate();
        handler2.Internal_InitUpdate();
        ByteManipulator.Write(dataReceived, 3, (byte)AudioDataTypeFlag.Single);
        ByteManipulator.Write(dataReceived, 2, (byte)2);
        workflow.ProcessReceivedPacket(dataReceived, 0, dataReceived.Length, 2);
        Assert.That(receiver2.Internal_writeIndex, Is.Not.EqualTo(46));
    }

    #endregion

    #region ProcessMicData

    [Test]
    public void TestProcessMicDataSuccess()
    {

    }
    [Test]
    public void TestProcessMicDataSuccessRedLight()
    {

    }

    #endregion
}
#region TestClasses
public class TestTransport : VoiceDataTransport
{
    /// <summary>
    /// Header size
    /// </summary>
    public const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte);
    /// <summary>
    /// Max final packet length
    /// </summary>
    public const int pLength = 1024;
    /// <summary>
    /// Max data length that should be sent to this class
    /// </summary>
    public override int MaxDataLength { get { return Internal_packetDataSize; } }
    /// <summary>
    /// To which id fake packets should be sent
    /// </summary>
    public ulong ReceiverId;

    /// <summary>
    /// Exposed for tests. Max payload size that can be processed
    /// </summary>
    [NonSerialized]
    public int Internal_packetDataSize = pLength - FirstPacketByteAvailable;

    /// <summary>
    /// Process packet data
    /// </summary>
    /// <param name="buffer">GamePacket of which data will be stored</param>
    /// <param name="dataReceived">Raw data received from network</param>
    /// <param name="startIndex">Raw data start index</param>
    /// <param name="length">Raw data length</param>
    /// <param name="netId">Sender net id</param>
    /// <returns>data info</returns>
    public override VoicePacketInfo ProcessReceivedData(BytePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.NetId = netId;
        info.Frequency = ByteManipulator.ReadUInt16(dataReceived, startIndex);
        startIndex += sizeof(ushort);
        info.Channels = ByteManipulator.ReadByte(dataReceived, startIndex);
        startIndex += sizeof(byte);
        info.Format = (AudioDataTypeFlag)ByteManipulator.ReadByte(dataReceived, startIndex);
        startIndex += sizeof(byte);
        info.ValidPacketInfo = true;

        buffer.WriteByteData(dataReceived, startIndex, length - sizeof(ushort) - sizeof(byte) - sizeof(byte));

        return info;
    }
    /// <summary>
    /// Sends a packet to all the other clients that need it
    /// </summary>
    /// <param name="data">GamePacket that stores the data to send</param>
    /// <param name="info">data info</param>
    public override void SendToAllOthers(BytePacket data, VoicePacketInfo info)
    {
        //Debug.Log("packet sent to all others");
        BytePacket toSend = new BytePacket(Internal_packetDataSize);
        toSend.Write(ReceiverId, 0);
        toSend.Write(info.Frequency);
        toSend.Write(info.Channels);
        toSend.Write((byte)info.Format);

        int n;
        toSend.Copy(data, out n);

        toSend.CurrentSeek = sizeof(ulong);

        //Workflow.ProcessReceivedPacket(toSend.Data, sizeof(ulong), toSend.CurrentLength - sizeof(ulong), ReceiverId);

    }
    /// <summary>
    /// Performs a normal SendToAllOthers
    /// </summary>
    /// <param name="data">GamePacket that stores the data to send</param>
    /// <param name="info">data info</param>
    /// <param name="receiverID">Receiver to which the packet should be sent</param>
    public override void SendTo(BytePacket data, VoicePacketInfo info, ulong receiverID)
    {
        SendToAllOthers(data, info);
    }
}
public class TestManipulator : VoiceDataManipulator
{
    public AudioDataTypeFlag Flag;
    public override AudioDataTypeFlag AvailableTypes { get { return Flag; } }
    /// <summary>
    /// Processes audio data in format Single into a GamePacket
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data startIndex</param>
    /// <param name="audioDataCount">number of bytes to process</param>
    /// <param name="info">data info</param>
    /// <param name="output">gamepacket on which data will be written</param>
    public override void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        int outputAvailableSpace = output.Data.Length - output.CurrentSeek;
        if (outputAvailableSpace < (audioDataCount * sizeof(float)) + sizeof(int))
            audioDataCount = (outputAvailableSpace / sizeof(float)) - sizeof(int);

        output.Write(audioDataCount);
        int length = audioDataCount + audioDataOffset;
        for (int i = audioDataOffset; i < length; i++)
        {
            output.Write(audioData[i]);
        }
    }
    /// <summary>
    /// Processes audio data in format Int16 into a GamePacket
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data startIndex</param>
    /// <param name="audioDataCount">number of bytes to process</param>
    /// <param name="info">data info</param>
    /// <param name="output">gamepacket on which data will be written</param>
    public override void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        int outputAvailableSpace = output.Data.Length - output.CurrentSeek;
        if (outputAvailableSpace < audioDataCount + sizeof(int))
            audioDataCount = outputAvailableSpace - sizeof(int);

        output.Write(audioDataCount);
        output.WriteByteData(audioData, audioDataOffset, audioDataCount);
    }
    /// <summary>
    /// Processes a Gamepacket into audio data in format Single
    /// </summary>
    /// <param name="packet">GamePacket to process</param>
    /// <param name="info">data info</param>
    /// <param name="out_audioData">output array on which data will be written</param>
    /// <param name="out_audioDataOffset">output array start index</param>
    /// <param name="dataCount">total number of bytes written</param>
    public override void FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        dataCount = packet.ReadInt() / 4;
        int length = dataCount + out_audioDataOffset;
        for (int i = out_audioDataOffset; i < length; i++)
        {
            out_audioData[i] = packet.ReadFloat();
        }
    }
    /// <summary>
    /// Processes a Gamepacket into audio data in format Int16
    /// </summary>
    /// <param name="packet">GamePacket to process</param>
    /// <param name="info">data info</param>
    /// <param name="out_audioData">output array on which data will be written</param>
    /// <param name="out_audioDataOffset">output array start index</param>
    /// <param name="dataCount">total number of bytes written</param>
    public override void FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        dataCount = packet.ReadInt();
        packet.ReadByteData(out_audioData, out_audioDataOffset, dataCount);
    }
}
#endregion