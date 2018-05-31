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
[TestOf(typeof(VoiceHandler))]
[Category("VOCASY")]
public class VoiceHandlerTest
{
    GameObject go;
    SupportHandler handler;
    SupportWorkflow workflow;
    SupportSettings settings;
    FieldInfo handlerSelfOutputVolume;
    [OneTimeSetUp]
    public void OneTimeSetupReflections()
    {
        handlerSelfOutputVolume = typeof(VoiceHandler).GetField("selfOutputVolume", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    [SetUp]
    public void SetupVoiceHandler()
    {
        go = new GameObject();
        workflow = ScriptableObject.CreateInstance<SupportWorkflow>();
        settings = ScriptableObject.CreateInstance<SupportSettings>();
        workflow.Settings = settings;
        handler = go.AddComponent<SupportHandler>();
        handler.Workflow = workflow;
    }
    [TearDown]
    public void TeardownVoiceHandler()
    {
        workflow.Settings = null;
        handler.Workflow = null;
        GameObject.DestroyImmediate(go);
        ScriptableObject.DestroyImmediate(workflow);
        ScriptableObject.DestroyImmediate(settings);
    }
    [Test]
    public void TestInitFlag()
    {
        handler.Flag = AudioDataTypeFlag.Int16;
        Assert.That(handler.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Int16));
    }
    [Test]
    public void TestInitFlag2()
    {
        handler.Flag = AudioDataTypeFlag.Both;
        Assert.That(handler.AvailableTypes, Is.EqualTo(AudioDataTypeFlag.Both));
    }
    [Test]
    public void TestInitSelfOutputVolume()
    {
        Assert.That(handlerSelfOutputVolume.GetValue(handler), Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestSelfOutputVolume()
    {
        handlerSelfOutputVolume.SetValue(handler, 0.5f);
        Assert.That(handler.SelfOutputVolume, Is.EqualTo(0.5).Within(0.0001));
    }
    [Test]
    public void TestSelfOutputVolume2()
    {
        handler.SelfOutputVolume = 0.8f;
        Assert.That(handlerSelfOutputVolume.GetValue(handler), Is.EqualTo(0.8).Within(0.0001));
    }
    [Test]
    public void TestSelfOutputVolume3()
    {
        handler.SelfOutputVolume = 1.8f;
        Assert.That(handlerSelfOutputVolume.GetValue(handler), Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestSelfOutputVolume4()
    {
        handler.SelfOutputVolume = -1.8f;
        Assert.That(handlerSelfOutputVolume.GetValue(handler), Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestSelfOutputVolume5()
    {
        handler.SelfOutputVolume = 1.8f;
        Assert.That(handler.SelfOutputVolume, Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestSelfOutputVolume6()
    {
        handler.SelfOutputVolume = -1.8f;
        Assert.That(handler.SelfOutputVolume, Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestOutputVolume()
    {
        handler.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        Assert.That(handler.OutputVolume, Is.EqualTo(1).Within(0.0001));
    }
    [Test]
    public void TestOutputVolume2()
    {
        handler.SelfOutputVolume = 0.5f;
        settings.VoiceChatVolume = 1f;
        Assert.That(handler.OutputVolume, Is.EqualTo(0.5).Within(0.0001));
    }
    [Test]
    public void TestOutputVolume3()
    {
        handler.SelfOutputVolume = 0.5f;
        settings.VoiceChatVolume = 0.5f;
        Assert.That(handler.OutputVolume, Is.EqualTo(0.25).Within(0.0001));
    }
    [Test]
    public void TestOutputVolume4()
    {
        handler.SelfOutputVolume = 0f;
        settings.VoiceChatVolume = 1f;
        Assert.That(handler.OutputVolume, Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestOutputVolume5()
    {
        handler.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 0f;
        Assert.That(handler.OutputVolume, Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestOutputVolume6()
    {
        handler.SelfOutputVolume = 0f;
        settings.VoiceChatVolume = 0f;
        Assert.That(handler.OutputVolume, Is.EqualTo(0).Within(0.0001));
    }
    [Test]
    public void TestOutputMuted()
    {
        handler.IsSelfOutputMuted = false;
        handler.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        Assert.That(handler.IsOutputMuted, Is.False);
    }
    [Test]
    public void TestOutputMuted2()
    {
        handler.IsSelfOutputMuted = true;
        handler.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 1f;
        Assert.That(handler.IsOutputMuted, Is.True);
    }
    [Test]
    public void TestOutputMuted3()
    {
        handler.IsSelfOutputMuted = false;
        handler.SelfOutputVolume = 0f;
        settings.VoiceChatVolume = 1f;
        Assert.That(handler.IsOutputMuted, Is.True);
    }
    [Test]
    public void TestOutputMuted4()
    {
        handler.IsSelfOutputMuted = false;
        handler.SelfOutputVolume = 1f;
        settings.VoiceChatVolume = 0f;
        Assert.That(handler.IsOutputMuted, Is.True);
    }
    [Test]
    public void TestOutputMuted5()
    {
        handler.IsSelfOutputMuted = false;
        handler.SelfOutputVolume = 0f;
        settings.VoiceChatVolume = 0f;
        Assert.That(handler.IsOutputMuted, Is.True);
    }
    [Test]
    public void TestOutputMuted6()
    {
        handler.IsSelfOutputMuted = true;
        handler.SelfOutputVolume = 0f;
        settings.VoiceChatVolume = 0f;
        Assert.That(handler.IsOutputMuted, Is.True);
    }
}