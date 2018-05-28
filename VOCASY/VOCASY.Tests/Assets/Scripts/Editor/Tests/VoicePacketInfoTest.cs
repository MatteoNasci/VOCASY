using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VOCASY.Common;
using VOCASY;
using GENUtility;
[TestFixture]
[TestOf(typeof(VoicePacketInfo))]
[Category("VOCASY")]
public class VoicePacketInfoTest
{
    VoicePacketInfo info;
    [Test]
    public void TestInvaildPacketNetId()
    {
        Assert.That(VoicePacketInfo.InvalidPacket.NetId, Is.EqualTo(0));
    }
    [Test]
    public void TestInvaildPacketFrequency()
    {
        Assert.That(VoicePacketInfo.InvalidPacket.Frequency, Is.EqualTo(0));
    }
    [Test]
    public void TestInvaildPacketChannels()
    {
        Assert.That(VoicePacketInfo.InvalidPacket.Channels, Is.EqualTo(0));
    }
    [Test]
    public void TestInvaildPacketFormat()
    {
        Assert.That(VoicePacketInfo.InvalidPacket.Format, Is.EqualTo(AudioDataTypeFlag.None));
    }
    [Test]
    public void TestInvaildPacketValidBool()
    {
        Assert.That(VoicePacketInfo.InvalidPacket.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestDefaultPacketNetId()
    {
        Assert.That(info.NetId, Is.EqualTo(0));
    }
    [Test]
    public void TestDefaultPacketFrequency()
    {
        Assert.That(info.Frequency, Is.EqualTo(0));
    }
    [Test]
    public void TestDefaultPacketChannels()
    {
        Assert.That(info.Channels, Is.EqualTo(0));
    }
    [Test]
    public void TestDefaultPacketFormat()
    {
        Assert.That(info.Format, Is.EqualTo(AudioDataTypeFlag.None));
    }
    [Test]
    public void TestDefaultPacketValidBool()
    {
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestInitPacketNetId()
    {
        info = new VoicePacketInfo(11, 5, 88, AudioDataTypeFlag.Single, true);
        Assert.That(info.NetId, Is.EqualTo(11));
    }
    [Test]
    public void TestInitPacketFrequency()
    {
        info = new VoicePacketInfo(11, 5, 88, AudioDataTypeFlag.Single, true);
        Assert.That(info.Frequency, Is.EqualTo(5));
    }
    [Test]
    public void TestInitPacketChannels()
    {
        info = new VoicePacketInfo(11, 5, 88, AudioDataTypeFlag.Single, true);
        Assert.That(info.Channels, Is.EqualTo(88));
    }
    [Test]
    public void TestInitPacketFormat()
    {
        info = new VoicePacketInfo(11, 5, 88, AudioDataTypeFlag.Single, true);
        Assert.That(info.Format, Is.EqualTo(AudioDataTypeFlag.Single));
    }
    [Test]
    public void TestInitPacketValidBool()
    {
        info = new VoicePacketInfo(11, 5, 88, AudioDataTypeFlag.Single, true);
        Assert.That(info.ValidPacketInfo, Is.True);
    }
}