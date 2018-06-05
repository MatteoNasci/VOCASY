using System.Collections;
using System.Collections.Generic;
using VOCASY;
using GENUtility;
using UnityEngine;
public class SupportHandler : VoiceHandler
{
    public bool GetDataSingle = false;
    public bool GetDataInt16 = false;
    public bool ReceiveDataSingle = false;
    public bool ReceiveDataInt16 = false;
    public bool IsRec;
    public ulong ID;
    public VoicePacketInfo Info;
    public float[] DataRec;
    public byte[] DataRecInt16;
    public float[] ReceivedData;
    public byte[] ReceivedDataInt16;
    public override bool IsRecorder { get { return IsRec; } }
    public AudioDataTypeFlag Flag { get { return this.AvailableTypes; } set { this.AvailableTypes = value; } }

    public override ulong NetID { get { return ID; } }

    public override VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount)
    {
        GetDataSingle = true;
        effectiveMicDataCount = Mathf.Min(buffer.Length - bufferOffset, DataRec.Length, micDataCount);
        ByteManipulator.Write<float>(DataRec, 0, buffer, bufferOffset, effectiveMicDataCount);
        return Info;
    }

    public override VoicePacketInfo GetMicDataInt16(byte[] buffer, int bufferOffset, int micDataCount, out int effectiveMicDataCount)
    {
        GetDataInt16 = true;
        effectiveMicDataCount = Mathf.Min(buffer.Length - bufferOffset, DataRecInt16.Length, micDataCount);
        ByteManipulator.Write<byte>(DataRecInt16, 0, buffer, bufferOffset, effectiveMicDataCount);
        return Info;
    }

    public override void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
    {
        ReceivedData = new float[audioDataCount];
        ByteManipulator.Write(audioData, audioDataOffset, ReceivedData, 0, audioDataCount);
        ReceiveDataSingle = true;
    }

    public override void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
    {
        ReceivedDataInt16 = new byte[audioDataCount];
        ByteManipulator.Write(audioData, audioDataOffset, ReceivedDataInt16, 0, audioDataCount);
        ReceiveDataInt16 = true;
    }
}