using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VOCASY;
using UnityEngine;
public class SupportReceiver : VoiceReceiver
{
    public AudioDataTypeFlag Flag;
    public override float Volume { get; set; }

    public override AudioDataTypeFlag AvailableTypes { get { return Flag; } }

    public override void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
    {
    }

    public override void ReceiveAudioData(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
    {
    }
}