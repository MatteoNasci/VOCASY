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
[TestOf(typeof(SelfDataTransport))]
[Category("VOCASY")]
public class SelfDataTransportTest
{
    SelfDataTransport transport;
    [SetUp]
    public void SetupTransport()
    {
        transport = ScriptableObject.CreateInstance<SelfDataTransport>();
    }
    [TearDown]
    public void TeardownTransport()
    {
        ScriptableObject.DestroyImmediate(transport);
    }
    [Test]
    public void TestPacketPayloadMaxSize()
    {
        Assert.That(transport.MaxDataLength, Is.EqualTo(1016));
    }

}