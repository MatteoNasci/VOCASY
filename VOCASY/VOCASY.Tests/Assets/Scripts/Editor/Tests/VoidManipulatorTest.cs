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
}