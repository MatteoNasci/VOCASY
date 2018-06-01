using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VOCASY;
public class SupportRecorder : VoiceRecorder
{
    public int MicDataReady;
    public bool Enabled;
    public AudioDataTypeFlag Flag;
    public override AudioDataTypeFlag AvailableTypes { get { return Flag; } }

    public override bool IsEnabled { get { return Enabled; } }

    public override int MicDataAvailable { get { return MicDataReady; } }

    public override VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int maxDataCount, out int effectiveDataCount)
    {
        effectiveDataCount = 0;
        return VoicePacketInfo.InvalidPacket;
    }

    public override VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int maxDataCount, out int effectiveDataCount)
    {
        effectiveDataCount = 0;
        return VoicePacketInfo.InvalidPacket;
    }

    public override void StartRecording()
    {
    }

    public override void StopRecording()
    {
    }
}