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
[TestOf(typeof(VoidManipulator))]
[Category("VOCASY")]
public class VoidManipulatorTest
{
    VoidManipulator manipulator;
    [SetUp]
    public void SetupManipulator()
    {
        manipulator = ScriptableObject.CreateInstance<VoidManipulator>();
    }
    [TearDown]
    public void TeardownManipulator()
    {
        ScriptableObject.DestroyImmediate(manipulator);
    }
    [Test]
    public void TestValidFlag()
    {
        Assert.That(manipulator.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Both));
    }
    [Test]
    public void TestFromAudioSingleToPacketHeaderValue()
    {
        float[] audiodata = new float[100];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadInt(0), Is.EqualTo(100));
    }
    [Test]
    public void TestFromAudioSingleToPacketHeaderValue2()
    {
        float[] audiodata = new float[50];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadInt(0), Is.EqualTo(50));
    }
    [Test]
    public void TestFromAudioSingleToPacketHeaderValue3()
    {
        float[] audiodata = new float[200];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadInt(0), Is.EqualTo(100));
    }
    [Test]
    public void TestFromAudioSingleToPacketValid()
    {
        float[] audiodata = new float[200];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [Test]
    public void TestFromAudioSingleToPacketOutputValues()
    {
        float[] audiodata = new float[100];
        for (int i = 0; i < audiodata.Length; i++)
        {
            audiodata[i] = i;
        }
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadFloat(4), Is.EqualTo(0).Within(0.00001));
    }
    [Test]
    public void TestFromAudioSingleToPacketOutputValues2()
    {
        float[] audiodata = new float[100];
        for (int i = 0; i < audiodata.Length; i++)
        {
            audiodata[i] = i;
        }
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadFloat(204), Is.EqualTo(50).Within(0.00001));
    }
    [Test]
    public void TestFromAudioSingleToPacketOutputValues3()
    {
        float[] audiodata = new float[100];
        for (int i = 0; i < audiodata.Length; i++)
        {
            audiodata[i] = i;
        }
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadFloat(400), Is.EqualTo(99).Within(0.00001));
    }
    [Test]
    public void TestFromAudioSingleToPacketInvalid()
    {
        float[] audiodata = new float[200];
        BytePacket output = new BytePacket(0);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioSingleToPacketInvalid2()
    {
        float[] audiodata = new float[100];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 400;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioSingleToPacketInvalid3()
    {
        float[] audiodata = new float[0];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioSingleToPacketInvalid4()
    {
        float[] audiodata = new float[100];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 500;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacket(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioInt16ToPacketHeaderValue()
    {
        byte[] audiodata = new byte[400];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadInt(0), Is.EqualTo(400));
    }
    [Test]
    public void TestFromAudioInt16ToPacketHeaderValue2()
    {
        byte[] audiodata = new byte[50];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadInt(0), Is.EqualTo(50));
    }
    [Test]
    public void TestFromAudioInt16ToPacketHeaderValue3()
    {
        byte[] audiodata = new byte[800];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadInt(0), Is.EqualTo(400));
    }
    [Test]
    public void TestFromAudioInt16ToPacketValid()
    {
        byte[] audiodata = new byte[200];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [Test]
    public void TestFromAudioInt16ToPacketOutputValues()
    {
        byte[] audiodata = new byte[400];
        int j = 0;
        for (int i = 0; i < audiodata.Length; i += 2, j++)
        {
            ByteManipulator.Write(audiodata, i, (ushort)j);
        }
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadUShort(4), Is.EqualTo(0));
    }
    [Test]
    public void TestFromAudioInt16ToPacketOutputValues2()
    {
        byte[] audiodata = new byte[400];
        int j = 0;
        for (int i = 0; i < audiodata.Length; i += 2, j++)
        {
            ByteManipulator.Write(audiodata, i, (ushort)j);
        }
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadUShort(204), Is.EqualTo(100));
    }
    [Test]
    public void TestFromAudioInt16ToPacketOutputValues3()
    {
        byte[] audiodata = new byte[400];
        int j = 0;
        for (int i = 0; i < audiodata.Length; i += 2, j++)
        {
            ByteManipulator.Write(audiodata, i, (ushort)j);
        }
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(output.ReadUShort(402), Is.EqualTo(199));
    }
    [Test]
    public void TestFromAudioInt16ToPacketInvalid()
    {
        byte[] audiodata = new byte[200];
        BytePacket output = new BytePacket(0);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioInt16ToPacketInvalid2()
    {
        byte[] audiodata = new byte[200];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 400;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioInt16ToPacketInvalid3()
    {
        byte[] audiodata = new byte[0];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 0;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromAudioInt16ToPacketInvalid4()
    {
        byte[] audiodata = new byte[200];
        BytePacket output = new BytePacket(404);
        output.CurrentSeek = 500;
        output.CurrentLength = 0;
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        manipulator.FromAudioDataToPacketInt16(audiodata, 0, audiodata.Length, ref info, output);
        Assert.That(info.ValidPacketInfo, Is.False);
    }

    [Test]
    public void TestFromPacketToAudioDataSingleCount()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(count, Is.EqualTo(25));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleCount2()
    {
        BytePacket packet = new BytePacket(404);
        packet.Write(100);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(count, Is.EqualTo(100));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleCount3()
    {
        BytePacket packet = new BytePacket(804);
        packet.Write(200);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(count, Is.EqualTo(100));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleValidPacket()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [Test]
    public void TestFromPacketToAudioDataSingleOutputValues()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        for (int i = 0; i < (packet.Data.Length - 4) / 4; i++)
        {
            packet.Write((float)i);
        }
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(output[0], Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleOutputValues2()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        for (int i = 0; i < (packet.Data.Length - 4) / 4; i++)
        {
            packet.Write((float)i);
        }
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(output[24], Is.EqualTo(24).Within(0.0001));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleOutputValues3()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        for (int i = 0; i < (packet.Data.Length - 4) / 4; i++)
        {
            packet.Write((float)i);
        }
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(output[9], Is.EqualTo(9).Within(0.0001));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleInvalidPacket()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 100);

        Assert.That(count, Is.EqualTo(0));
    }
    [Test]
    public void TestFromPacketToAudioDataSingleInvalidPacket2()
    {
        BytePacket packet = new BytePacket(4);
        packet.Write(25);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 4;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromPacketToAudioDataSingleInvalidPacket3()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(-1);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        float[] output = new float[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioData(packet, ref info, output, 0);

        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromPacketToAudioDataInt16Count()
    {
        BytePacket packet = new BytePacket(54);
        packet.Write(25);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(count, Is.EqualTo(25));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16Count2()
    {
        BytePacket packet = new BytePacket(404);
        packet.Write(100);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(count, Is.EqualTo(100));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16Count3()
    {
        BytePacket packet = new BytePacket(804);
        packet.Write(200);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(count, Is.EqualTo(100));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16ValidPacket()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(info.ValidPacketInfo, Is.True);
    }
    [Test]
    public void TestFromPacketToAudioDataInt16OutputValues()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(100);
        for (int i = 0; i < (packet.Data.Length - 4); i++)
        {
            packet.Write((byte)i);
        }
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(output[0], Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16OutputValues2()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(100);
        for (int i = 0; i < (packet.Data.Length - 4); i++)
        {
            packet.Write((byte)i);
        }
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(output[24], Is.EqualTo(24).Within(0.0001));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16OutputValues3()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(25);
        for (int i = 0; i < (packet.Data.Length - 4); i++)
        {
            packet.Write((byte)i);
        }
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(output[9], Is.EqualTo(9).Within(0.0001));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16InvalidPacket()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(100);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 100);

        Assert.That(count, Is.EqualTo(0));
    }
    [Test]
    public void TestFromPacketToAudioDataInt16InvalidPacket2()
    {
        BytePacket packet = new BytePacket(4);
        packet.Write(100);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 4;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(info.ValidPacketInfo, Is.False);
    }
    [Test]
    public void TestFromPacketToAudioDataInt16InvalidPacket3()
    {
        BytePacket packet = new BytePacket(104);
        packet.Write(-1);
        packet.CurrentSeek = 0;
        packet.CurrentLength = 104;
        byte[] output = new byte[100];
        VoicePacketInfo info = new VoicePacketInfo();
        info.ValidPacketInfo = true;
        info.NetId = 1;
        info.Frequency = 48000;
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;

        int count = manipulator.FromPacketToAudioDataInt16(packet, ref info, output, 0);

        Assert.That(info.ValidPacketInfo, Is.False);
    }
}