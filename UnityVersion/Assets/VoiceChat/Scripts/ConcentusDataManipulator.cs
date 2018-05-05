using VOCASY;
using Concentus.Enums;
using Concentus.Structs;
using VOCASY.Utility;
using UnityEngine;
public class ConcentusDataManipulator : IAudioDataManipulator
{
    public AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Single; } }

    private OpusEncoder encoder;
    private OpusDecoder decoder;

    public ConcentusDataManipulator(int fs, int channels)
    {
        encoder = new OpusEncoder(fs, channels, OpusApplication.OPUS_APPLICATION_VOIP);
        decoder = new OpusDecoder(fs, channels);
    }

    public void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output)
    {
        int startIndex = output.CurrentSeek + (sizeof(int) * 2);
        int frameSize = Mathf.Min(audioDataCount / info.Channels, (info.Frequency / 100) * 6);
        int n = encoder.Encode(audioData, audioDataOffset, frameSize, output.Data, startIndex, output.MaxCapacity - startIndex);
        output.Write(n);
        output.Write(frameSize);
        output.CurrentSeek += n;
    }

    public void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output)
    {
        info.ValidPacketInfo = false;
    }

    public void FromPacketToAudioData(GamePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        int length = packet.ReadInt();
        int frameSize = packet.ReadInt();
        dataCount = decoder.Decode(packet.Data, packet.CurrentSeek, length, out_audioData, out_audioDataOffset, frameSize) * decoder.NumChannels;
    }

    public void FromPacketToAudioDataInt16(GamePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        dataCount = 0;
        info.ValidPacketInfo = false;
    }
}