using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VOCASY.Common;
using VOCASY;
using GENUtility;
using System.Reflection;
using UnityEngine;
[TestFixture]
[TestOf(typeof(Receiver))]
[Category("VOCASY")]
public class ReceiverTest
{
    GameObject go;
    SupportSettings settings;
    Receiver receiver;
    AudioSource source;
    MethodInfo recOnEnable;
    MethodInfo recOnDisable;
    MethodInfo recOnAudioFilterRead;
    FieldInfo recBuffer;
    FieldInfo recReadIndex;
    FieldInfo recWriteIndex;
    FieldInfo recSource;
    [OneTimeSetUp]
    public void OneTimeSetupReflections()
    {
        Type type = typeof(Receiver);
        recOnEnable = type.GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        recOnDisable = type.GetMethod("OnDisable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        recOnAudioFilterRead = type.GetMethod("OnAudioFilterRead", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        recBuffer = type.GetField("cyclicAudioBuffer", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        recReadIndex = type.GetField("readIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        recWriteIndex = type.GetField("writeIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        recSource = type.GetField("source", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    }
    [SetUp]
    public void SetupReceiver()
    {
        go = new GameObject();
        source = go.AddComponent<AudioSource>();
        settings = ScriptableObject.CreateInstance<SupportSettings>();
        receiver = go.AddComponent<Receiver>();
        receiver.Settings = settings;
        recOnEnable.Invoke(receiver, new object[0]);
    }
    [TearDown]
    public void TeardownReceiver()
    {
        receiver.Settings = null;
        GameObject.DestroyImmediate(go);
        ScriptableObject.DestroyImmediate(settings);
    }
    [Test]
    public void TestAvailableTypesValue()
    {
        Assert.That(receiver.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Both));
    }
    [Test]
    public void TestNoAudioSourceNoNullRefException()
    {
        GameObject go2 = new GameObject();

        Receiver rec2 = go2.AddComponent<Receiver>();

        recSource.SetValue(rec2, null);

        Assert.DoesNotThrow(() => recOnEnable.Invoke(rec2, new object[] { }));

        GameObject.DestroyImmediate(go2);
    }
    [Test]
    public void TestInitializationSourceEnabled()
    {
        Assert.That(source.isActiveAndEnabled, Is.True);
    }
    [Test]
    public void TestDeInitializationSourcePlaying()
    {
        recOnDisable.Invoke(receiver, new object[0]);
        Assert.That(source.isPlaying, Is.False);
    }
    [Test]
    public void TestDeInitializationSourceEnabled()
    {
        recOnDisable.Invoke(receiver, new object[0]);
        Assert.That(source.isActiveAndEnabled, Is.False);
    }
    [Test]
    public void TestDeInitializationReadIndex()
    {
        recReadIndex.SetValue(receiver, 100);
        recOnDisable.Invoke(receiver, new object[0]);
        Assert.That(recReadIndex.GetValue(receiver), Is.EqualTo(0));
    }
    [Test]
    public void TestDeInitializationWriteIndex()
    {
        recWriteIndex.SetValue(receiver, 100);
        recOnDisable.Invoke(receiver, new object[0]);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(0));
    }
    [Test]
    public void TestInitializationReadIndex()
    {
        Assert.That(recReadIndex.GetValue(receiver), Is.EqualTo(0));
    }
    [Test]
    public void TestInitializationWriteIndex()
    {
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(0));
    }
    [Test]
    public void TestInitializationBuffer()
    {
        Assert.That(recBuffer.GetValue(receiver), Is.Null);
    }
    [Test]
    public void TestVolumeSourceSyncGet()
    {
        source.volume = 0.3f;
        Assert.That(receiver.Volume, Is.EqualTo(0.3).Within(0.00001));
    }
    [Test]
    public void TestVolumeSourceSyncSet()
    {
        receiver.Volume = 0.8f;
        Assert.That(source.volume, Is.EqualTo(0.8).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataEarlyOutBufferNull()
    {
        float[] data = new float[20];
        int channels = 2;
        recWriteIndex.SetValue(receiver, 20);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(data[9], Is.EqualTo(0).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataEarlyOutNotEnoughStoredData()
    {
        float[] data = new float[20];
        int channels = 2;
        recBuffer.SetValue(receiver, new float[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f });
        recWriteIndex.SetValue(receiver, 20);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        data = new float[20];
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(data[1], Is.EqualTo(0).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataWritten()
    {
        float[] data = new float[20];
        int channels = 2;
        recBuffer.SetValue(receiver, new float[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f });
        recWriteIndex.SetValue(receiver, 20);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(data[12], Is.EqualTo(12).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataWritten2()
    {
        float[] data = new float[20];
        int channels = 2;
        recBuffer.SetValue(receiver, new float[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f });
        recWriteIndex.SetValue(receiver, 10);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(data[10], Is.EqualTo(0).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataWritten3()
    {
        float[] data = new float[20];
        int channels = 2;
        recBuffer.SetValue(receiver, new float[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f });
        recWriteIndex.SetValue(receiver, 5);
        recReadIndex.SetValue(receiver, 18);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(data[4], Is.EqualTo(2).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataWrittenChangedReadIndex()
    {
        float[] data = new float[20];
        int channels = 2;
        recBuffer.SetValue(receiver, new float[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f });
        recReadIndex.SetValue(receiver, 0);
        recWriteIndex.SetValue(receiver, 20);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(recReadIndex.GetValue(receiver), Is.EqualTo(20).Within(0.00001));
    }
    [Test]
    public void TestOnAudioFilterReadDataWrittenChangedReadIndex2()
    {
        float[] data = new float[20];
        int channels = 2;
        recBuffer.SetValue(receiver, new float[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f });
        recReadIndex.SetValue(receiver, 18);
        recWriteIndex.SetValue(receiver, 5);
        recOnAudioFilterRead.Invoke(receiver, new object[] { data, channels });
        Assert.That(recReadIndex.GetValue(receiver), Is.EqualTo(5).Within(0.00001));
    }
    [Test]
    public void TestReceiveAudioDataInt16BufferInit()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recBuffer.GetValue(receiver), Is.Not.Null);
    }
    [Test]
    public void TestReceiveAudioDataInt16BufferInit2()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[]).Length, Is.EqualTo(12000));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(10));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate2()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(5));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate3()
    {
        byte[] data = new byte[20];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(1));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate4()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(20));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate5()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(10));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate6()
    {
        byte[] data = new byte[20];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(3));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate7()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(40));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate8()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(20));
    }
    [Test]
    public void TestReceiveAudioDataInt16WriteIndexUpdate9()
    {
        byte[] data = new byte[20];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (byte)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(7));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 1);
        Assert.That((recBuffer.GetValue(receiver) as float[])[0], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity2()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 2);
        Assert.That((recBuffer.GetValue(receiver) as float[])[1], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity3()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 3);
        Assert.That((recBuffer.GetValue(receiver) as float[])[0], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity4()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 1);
        Assert.That((recBuffer.GetValue(receiver) as float[])[2], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity5()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 1);
        Assert.That((recBuffer.GetValue(receiver) as float[])[2], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity6()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 5);
        Assert.That((recBuffer.GetValue(receiver) as float[])[1], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity7()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 1);
        Assert.That((recBuffer.GetValue(receiver) as float[])[8], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity8()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 3);
        Assert.That((recBuffer.GetValue(receiver) as float[])[8], Is.EqualTo(f).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataInt16DataIntegrity9()
    {
        byte[] data = new byte[10];
        for (int i = 0; i < 5; i++)
        {
            ByteManipulator.Write(data, i * 2, (short)i);
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        float f = Mathf.InverseLerp(short.MinValue, short.MaxValue, 1);
        Assert.That((recBuffer.GetValue(receiver) as float[])[0], Is.EqualTo(f).Within(0.0001));
    }

    [Test]
    public void TestReceiveAudioDataSingleBufferInit()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recBuffer.GetValue(receiver), Is.Not.Null);
    }
    [Test]
    public void TestReceiveAudioDataSingleBufferInit2()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[]).Length, Is.EqualTo(12000));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(20));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate2()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(10));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate3()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(3));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate4()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(40));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate5()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(20));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate6()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(7));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate7()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(80));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate8()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(40));
    }
    [Test]
    public void TestReceiveAudioDataSingleWriteIndexUpdate9()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That(recWriteIndex.GetValue(receiver), Is.EqualTo(15));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[1], Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity2()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[5], Is.EqualTo(5).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity3()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 48000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[2], Is.EqualTo(9).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity4()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[2], Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity5()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[2], Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity6()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 24000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[1], Is.EqualTo(3).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity7()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 1;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[8], Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity8()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 2;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 0);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[12], Is.EqualTo(3).Within(0.0001));
    }
    [Test]
    public void TestReceiveAudioDataSingleDataIntegrity9()
    {
        float[] data = new float[10];
        for (int i = 0; i < 10; i++)
        {
            data[i] = (float)i;
        }
        VoicePacketInfo info = new VoicePacketInfo();
        info.Channels = 5;
        info.Format = AudioDataTypeFlag.Both;
        info.ValidPacketInfo = true;
        info.Frequency = 12000;
        recWriteIndex.SetValue(receiver, 11999);

        receiver.ReceiveAudioData(data, 0, 10, info);
        Assert.That((recBuffer.GetValue(receiver) as float[])[0], Is.EqualTo(1).Within(0.0001));
    }
}