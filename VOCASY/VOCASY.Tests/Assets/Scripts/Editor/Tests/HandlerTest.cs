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
    MethodInfo handlerOnDestroy;
    [OneTimeSetUp]
    public void OneTimeSetupReflections()
    {
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
        handlerOnDestroy = t.GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    [SetUp]
    public void SetupVoiceHandler()
    {
        go = new GameObject();
        workflow = ScriptableObject.CreateInstance<SupportWorkflow>();
        settings = ScriptableObject.CreateInstance<SupportSettings>();
        workflow.Settings = settings;
        recorder = go.AddComponent<Recorder>();
        receiver = go.AddComponent<Receiver>();
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
}