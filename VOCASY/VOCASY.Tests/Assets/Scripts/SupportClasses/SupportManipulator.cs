using UnityEngine;
using GENUtility;
using VOCASY.Common;
using VOCASY;
public class SupportManipulator : VoiceDataManipulator
{
    public bool FromAudioToPacket = false;
    public bool FromAudioToPacketInt16 = false;
    public bool FromPacketToAudio = false;
    public bool FromPacketToAudioInt16 = false;
    public AudioDataTypeFlag Flag;
    public VoicePacketInfo Info;
    public bool UseInfo;
    public override AudioDataTypeFlag AvailableTypes { get { return Flag; } }

    public override void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        FromAudioToPacket = true;
        if (UseInfo)
            info = this.Info;
    }

    public override void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        FromAudioToPacketInt16 = true;
        if (UseInfo)
            info = this.Info;
    }

    public override void FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        dataCount = packet.CurrentLength;
        FromPacketToAudio = true;
        if (UseInfo)
            info = this.Info;
    }

    public override void FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        dataCount = packet.CurrentLength;
        FromPacketToAudioInt16 = true;
        if (UseInfo)
            info = this.Info;
    }
}