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
        for (int i = 0; i < audioDataCount; i++)
        {
            output.Write(audioData[i + audioDataOffset]);
        }
        if (UseInfo)
            info = this.Info;
    }

    public override void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        FromAudioToPacketInt16 = true;
        output.WriteByteData(audioData, audioDataOffset, audioDataCount);
        if (UseInfo)
            info = this.Info;
    }

    public override int FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset)
    {
        int dataCount = Mathf.Min(packet.CurrentLength - packet.CurrentSeek, out_audioData.Length - out_audioDataOffset);
        for (int i = 0; i < dataCount / sizeof(float); i++)
        {
            out_audioData[i + out_audioDataOffset] = packet.ReadFloat();
        }
        FromPacketToAudio = true;
        if (UseInfo)
            info = this.Info;
        return dataCount;
    }

    public override int FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset)
    {
        int dataCount = Mathf.Min(packet.CurrentLength - packet.CurrentSeek, out_audioData.Length - out_audioDataOffset);
        packet.ReadByteData(out_audioData, out_audioDataOffset, dataCount);
        FromPacketToAudioInt16 = true;
        if (UseInfo)
            info = this.Info;
        return dataCount;
    }
}