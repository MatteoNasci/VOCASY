using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VOCASY.Common;
using VOCASY;
using GENUtility;
using UnityEngine;
[TestFixture]
[TestOf(typeof(Transport))]
[Category("VOCASY")]
public class TransportTest
{
    Transport transport;
    SupportWorkflow workflow;
    List<ulong> targets;
    ulong sender;
    public void SendToAllOthers(byte[] data, int startIndex, int length, List<ulong> receiversIds)
    {
        targets.Clear();
        for (int i = 0; i < receiversIds.Count; i++)
        {
            targets.Add(receiversIds[i]);
        }
        workflow.ProcessReceivedPacket(data, startIndex, length, sender);
    }
    public void SendMSG(ulong targetID, bool isTargetMutedByLocal)
    {
        throw new NotImplementedException();
    }
    [SetUp]
    public void SetupTransport()
    {
        transport = ScriptableObject.CreateInstance<Transport>();
        transport.SendMsgTo = SendMSG;
        transport.SendToAllAction = SendToAllOthers;
        workflow = ScriptableObject.CreateInstance<SupportWorkflow>();
        transport.Workflow = workflow;
    }
    [TearDown]
    public void TeardownTransport()
    {
        sender = 0;
        transport.Workflow = null;
        ScriptableObject.DestroyImmediate(transport);
        ScriptableObject.DestroyImmediate(workflow);
    }
    [Test]
    public void TestPacketPayloadMaxSize()
    {
        Assert.That(transport.MaxDataLength, Is.EqualTo(988));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadLength()
    {
        BytePacket p = new BytePacket(10);
        p.CurrentSeek = 0;
        p.CurrentLength = 10;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(workflow.receivedData.Length, Is.EqualTo(22));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadLengthRedLight()
    {
        BytePacket p = new BytePacket(10);
        p.CurrentSeek = 0;
        p.CurrentLength = 10;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(workflow.receivedData.Length, Is.Not.EqualTo(14));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadInt32(workflow.receivedData, 12), Is.EqualTo(750));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadDataRedLight()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadInt32(workflow.receivedData, 12), Is.Not.EqualTo(0));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData2()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadUInt16(workflow.receivedData, 16), Is.EqualTo(110));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData2RedLight()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadUInt16(workflow.receivedData, 16), Is.Not.EqualTo(10));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData5()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadSByte(workflow.receivedData, 18), Is.EqualTo(-5));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData5RedLight()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadSByte(workflow.receivedData, 18), Is.Not.EqualTo(0));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData3()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadBoolean(workflow.receivedData, 19), Is.False);
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData3RedLight()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)short.MinValue);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadBoolean(workflow.receivedData, 19), Is.Not.True);
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData4()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)-32767);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadInt16(workflow.receivedData, 20), Is.EqualTo(-32767));
    }
    [Test]
    public void TestSendToAllOthersCorrectPayloadData4RedLight()
    {
        BytePacket p = new BytePacket(10);
        p.Write(750);
        p.Write((ushort)110);
        p.Write((sbyte)-5);
        p.Write(false);
        p.Write((short)-32768);
        p.CurrentSeek = 0;
        transport.SendToAll(p, new VoicePacketInfo(), new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadInt16(workflow.receivedData, 20), Is.Not.EqualTo(0));
    }
    [Test]
    public void TestSendToAllOthersCorrectId()
    {
        transport.SendToAll(new BytePacket(1), new VoicePacketInfo(), new List<ulong>() { 0 });
        Assert.That(workflow.receivedID, Is.EqualTo(0));
    }
    [Test]
    public void TestSendToAllOthersCorrectIdRedLight()
    {
        transport.SendToAll(new BytePacket(1), new VoicePacketInfo(), new List<ulong>() { 0 });
        Assert.That(workflow.receivedID, Is.Not.EqualTo(1));
    }
    [Test]
    public void TestSendToAllOthersCorrectFrequency()
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.Frequency = 17898;
        transport.SendToAll(new BytePacket(1), info, new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadUInt16(workflow.receivedData, 8), Is.EqualTo(17898));
    }
    [Test]
    public void TestSendToAllOthersCorrectFrequencyRedLight()
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.Frequency = 5666;
        transport.SendToAll(new BytePacket(1), info, new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadUInt16(workflow.receivedData, 8), Is.Not.EqualTo(17898));
    }
    [Test]
    public void TestSendToAllOthersCorrectChannels()
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 7;
        transport.SendToAll(new BytePacket(1), info, new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadByte(workflow.receivedData, 10), Is.EqualTo(7));
    }
    [Test]
    public void TestSendToAllOthersCorrectChannelsRedLight()
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        transport.SendToAll(new BytePacket(1), info, new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadByte(workflow.receivedData, 10), Is.Not.EqualTo(7));
    }
    [Test]
    public void TestSendToAllOthersCorrectFormat()
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.Format = AudioDataTypeFlag.Int16;
        transport.SendToAll(new BytePacket(1), info, new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadByte(workflow.receivedData, 11), Is.EqualTo((byte)AudioDataTypeFlag.Int16));
    }
    [Test]
    public void TestSendToAllOthersCorrectFormatRedLight()
    {
        VoicePacketInfo info = new VoicePacketInfo();
        info.Format = AudioDataTypeFlag.Single;
        transport.SendToAll(new BytePacket(1), info, new List<ulong>() { 1 });
        Assert.That(ByteManipulator.ReadByte(workflow.receivedData, 11), Is.Not.EqualTo((byte)AudioDataTypeFlag.Int16));
    }
    [Test]
    public void TestProcessReceivedDataLength()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ulong netId = 5;
        transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(buffer.CurrentLength, Is.EqualTo(16));
    }
    [Test]
    public void TestProcessReceivedDataLengthRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ulong netId = 5;
        transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(buffer.CurrentLength, Is.Not.EqualTo(20));
    }
    [Test]
    public void TestProcessReceivedDataId()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 5;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.NetId, Is.EqualTo(5));
    }
    [Test]
    public void TestProcessReceivedDataIdRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 6;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.NetId, Is.Not.EqualTo(5));
    }
    [Test]
    public void TestProcessReceivedDataFreq()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 5;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.Frequency, Is.EqualTo(4588));
    }
    [Test]
    public void TestProcessReceivedDataFreqRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)45858);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 6;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.Frequency, Is.Not.EqualTo(4588));
    }
    [Test]
    public void TestProcessReceivedDataChann()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 5;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.Channels, Is.EqualTo(5));
    }
    [Test]
    public void TestProcessReceivedDataChannRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)6);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 6;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.Channels, Is.Not.EqualTo(5));
    }
    [Test]
    public void TestProcessReceivedDataFormat()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 5;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That((byte)info.Format, Is.EqualTo(99));
    }
    [Test]
    public void TestProcessReceivedDataFormatRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)199);
        ulong netId = 6;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That((byte)info.Format, Is.Not.EqualTo(99));
    }
    [Test]
    public void TestProcessReceivedDataValidPacket()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 5;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [Test]
    public void TestProcessReceivedDataValidPacketRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ulong netId = 6;
        VoicePacketInfo info = transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(info.ValidPacketInfo, Is.Not.False);
    }
    [Test]
    public void TestProcessReceivedDataPayloadIntegrity()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ByteManipulator.Write(receivedData, 4, (int)7788);
        ulong netId = 5;
        transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(buffer.ReadInt(0), Is.EqualTo(7788));
    }
    [Test]
    public void TestProcessReceivedDataPayloadIntegrityRedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ByteManipulator.Write(receivedData, 4, (int)7788);
        ulong netId = 5;
        transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(buffer.ReadInt(0), Is.Not.EqualTo(0));
    }
    [Test]
    public void TestProcessReceivedDataPayloadIntegrity2()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ByteManipulator.Write(receivedData, 15, (int)77889);
        ulong netId = 5;
        transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(buffer.ReadInt(11), Is.EqualTo(77889));
    }
    [Test]
    public void TestProcessReceivedDataPayloadIntegrity2RedLight()
    {
        BytePacket buffer = new BytePacket(20);
        byte[] receivedData = new byte[20];
        ByteManipulator.Write(receivedData, 0, (ushort)4588);
        ByteManipulator.Write(receivedData, 2, (byte)5);
        ByteManipulator.Write(receivedData, 3, (byte)99);
        ByteManipulator.Write(receivedData, 15, (int)77889);
        ulong netId = 5;
        transport.ProcessReceivedData(buffer, receivedData, 0, 20, netId);
        Assert.That(buffer.ReadInt(11), Is.Not.EqualTo(0));
    }
    [Test]
    public void TestProcessNetworkReceivedPacket()
    {
        byte[] data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        transport.ProcessNetworkReceivedPacket(data, 0, 10, 2);
        Assert.That(workflow.receivedData[3], Is.EqualTo(3));
    }
    [Test]
    public void TestProcessNetworkReceivedPacket2()
    {
        byte[] data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        transport.ProcessNetworkReceivedPacket(data, 0, 10, 2);
        Assert.That(workflow.receivedID, Is.EqualTo(2));
    }
    [Test]
    public void TestProcessNetworkIsMutedMessage()
    {
        Assert.That(workflow.HandlersMuteStatuses.Count, Is.EqualTo(0));
    }
    [Test]
    public void TestProcessNetworkIsMutedMessage2()
    {
        transport.ProcessNetworkIsMutedMessage(true, 2);
        Assert.That(workflow.HandlersMuteStatuses.Count, Is.EqualTo(1));
    }
    [Test]
    public void TestProcessNetworkIsMutedMessage3()
    {
        transport.ProcessNetworkIsMutedMessage(true, 2);
        Assert.That(workflow.HandlersMuteStatuses[2], Is.EqualTo(MuteStatus.RemoteHasMutedLocal));
    }
    [Test]
    public void TestProcessNetworkIsMutedMessage4()
    {
        transport.ProcessNetworkIsMutedMessage(false, 2);
        Assert.That(workflow.HandlersMuteStatuses[2], Is.EqualTo(MuteStatus.None));
    }
    [Test]
    public void TestSendMessageIsMutedTo()
    {
        ulong id = 0;
        transport.SendMsgTo = (ulong u, bool b) => { id = u; };
        transport.SendMessageIsMutedTo(1, true);
        Assert.That(id, Is.EqualTo(1));
    }
    [Test]
    public void TestSendMessageIsMutedTo2()
    {
        bool res = false;
        transport.SendMsgTo = (ulong u, bool b) => { res = b; };
        transport.SendMessageIsMutedTo(1, true);
        Assert.That(res, Is.True);
    }
    [Test]
    public void TestSendMessageIsMutedTo3()
    {
        ulong id = 0;
        transport.SendMsgTo = (ulong u, bool b) => { id = u; };
        transport.SendMessageIsMutedTo(1112, true);
        Assert.That(id, Is.EqualTo(1112));
    }
    [Test]
    public void TestSendMessageIsMutedTo4()
    {
        bool res = false;
        transport.SendMsgTo = (ulong u, bool b) => { res = b; };
        transport.SendMessageIsMutedTo(1, false);
        Assert.That(res, Is.False);
    }
    //TODO : SendToAll , ricontrollare tutti i test (attenzione a field sender)
}