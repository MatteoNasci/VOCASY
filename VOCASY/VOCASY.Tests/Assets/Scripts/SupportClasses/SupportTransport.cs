using GENUtility;
using VOCASY;
using VOCASY.Common;
public class SupportTransport : VoiceDataTransport
{
    public int DataSentTo;
    public int DataSent;
    public int DataReceived;
    public int MaxDataL = 1024;
    public VoicePacketInfo Info;
    public byte[] SentArray;
    public override int MaxDataLength { get { return MaxDataL; } }

    public override VoicePacketInfo ProcessReceivedData(BytePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
    {
        Info.NetId = netId;
        DataReceived = length;
        buffer.WriteByteData(dataReceived, startIndex, length);
        return Info;
    }

    public override void SendTo(BytePacket data, VoicePacketInfo info, ulong receiverID)
    {
        SentArray = new byte[data.Data.Length];
        ByteManipulator.Write<byte>(data.Data, 0, SentArray, 0, SentArray.Length);
        DataSentTo = data.CurrentLength;
    }

    public override void SendToAllOthers(BytePacket data, VoicePacketInfo info)
    {
        SentArray = new byte[data.Data.Length];
        ByteManipulator.Write<byte>(data.Data, 0, SentArray, 0, SentArray.Length);
        DataSent = data.CurrentLength;
    }
}