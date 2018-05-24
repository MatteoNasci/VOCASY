using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VOCASY;
using VOCASY.Common;
using UnityEngine;
[TestFixture]
[TestOf(typeof(VoiceDataWorkflow))]
[Category("VOCASY/Common")]
public class VoiceDataWorkflowTest
{
    VoiceDataWorkflow workflow;
    VoiceChatSettings settings;
    VoiceDataManipulator manipulator;
    VoiceDataTransport transport;
    [SetUp]
    public void Setup()
    {
        workflow = new VoiceDataWorkflow();
        workflow.Settings = settings;
        workflow.Transport = transport;
        workflow.Manipulator = manipulator;
    }
    [TearDown]
    public void Teardown()
    {
    }
    [Test]
    public void Test()
    {

    }
}