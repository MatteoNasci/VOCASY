using VOCASY;
using Concentus.Enums;
using Concentus.Structs;
using GENUtility;
using UnityEngine;
[CreateAssetMenu(fileName = "ConcentusManipulator", menuName = ("VOCASY/DataManipulators/Concentus"))]
public class ConcentusDataManipulator : VoiceDataManipulator
{
    public override AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Single; } }

    public VoiceChatSettings Settings;

    private OpusEncoder encoder;
    private OpusDecoder decoder;

    void OnEnable()
    {
        encoder = new OpusEncoder(Settings.MaxFrequency, Settings.MaxChannels, OpusApplication.OPUS_APPLICATION_VOIP);
        decoder = new OpusDecoder(Settings.MaxFrequency, Settings.MaxChannels);
    }

    public override void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        int startIndex = output.CurrentSeek + (sizeof(int) * 2);
        int frameSize = Mathf.Min(audioDataCount / info.Channels, (info.Frequency / 100) * 6);
        int n = encoder.Encode(audioData, audioDataOffset, frameSize, output.Data, startIndex, output.MaxCapacity - startIndex);
        output.Write(n);
        output.Write(frameSize);
        output.CurrentSeek += n;
    }

    public override void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        info.ValidPacketInfo = false;
    }

    public override int FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset)
    {
        int length = packet.ReadInt();
        int frameSize = packet.ReadInt();
        return decoder.Decode(packet.Data, packet.CurrentSeek, length, out_audioData, out_audioDataOffset, frameSize) * decoder.NumChannels;
    }

    public override int FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset)
    {
        info.ValidPacketInfo = false;
        return 0;
    }
}