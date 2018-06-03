using GENUtility;
using VOCASY;
using VOCASY.Common;
using System.Collections.Generic;
public class SupportTransport : VoiceDataTransport
{
    public List<ulong> DataSentTo = new List<ulong>();
    public int DataSent;
    public int DataReceived;
    public int MaxDataL = 1024;
    public VoicePacketInfo Info;
    public byte[] SentArray;
    public override int MaxDataLength { get { return MaxDataL; } }
    public VoiceDataWorkflow Workflow;

    public override VoicePacketInfo ProcessReceivedData(BytePacket buffer, byte[] dataReceived, int startIndex, int length, ulong netId)
    {
        DataReceived = length;
        buffer.WriteByteData(dataReceived, startIndex, length);
        return Info;
    }

    public override void SendMessageIsMutedTo(ulong receiverID, bool isReceiverMutedByLocal)
    {
        throw new System.NotImplementedException();
    }

    public override void SendToAll(BytePacket data, VoicePacketInfo info, List<ulong> receiversIds)
    {
        SentArray = new byte[data.Data.Length];
        ByteManipulator.Write<byte>(data.Data, 0, SentArray, 0, SentArray.Length);
        DataSent = data.CurrentLength;
        for (int i = 0; i < receiversIds.Count; i++)
        {
            DataSentTo.Add(receiversIds[i]);
        }
    }
}