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
    public AudioDataTypeFlag Flag = AudioDataTypeFlag.Both;
    public override AudioDataTypeFlag AvailableTypes { get { return Flag; } }

    public override bool IsEnabled { get { return Enabled; } }

    public override int MicDataAvailable { get { return MicDataReady; } }

    public override VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int maxDataCount, out int effectiveDataCount)
    {
        effectiveDataCount = MicDataAvailable;
        return new VoicePacketInfo() { Format = AudioDataTypeFlag.Single, Channels = 1, Frequency = 24000, ValidPacketInfo = true };
    }

    public override VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int maxDataCount, out int effectiveDataCount)
    {
        effectiveDataCount = Mathf.Min(maxDataCount, MicDataAvailable * 2);
        return new VoicePacketInfo() { Format = AudioDataTypeFlag.Single, Channels = 1, Frequency = 24000, ValidPacketInfo = true };
    }

    public override void StartRecording()
    {
    }

    public override void StopRecording()
    {
    }
}