using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VOCASY;
public class SupportWorkflow : VoiceDataWorkflow
{
    public byte[] receivedData;
    public ulong receivedID;
    public Dictionary<ulong, VoiceHandler> Handlers = new Dictionary<ulong, VoiceHandler>();
    public override void AddVoiceHandler(VoiceHandler handler)
    {
        Handlers.Add(handler.NetID, handler);
    }

    public override void Initialize()
    {
    }

    public override void ProcessMicData(VoiceHandler handler)
    {
    }

    public override void ProcessReceivedPacket(byte[] receivedData, int startIndex, int length, ulong netId)
    {
        this.receivedData = new byte[length];
        Array.Copy(receivedData, startIndex, this.receivedData, 0, length);
        this.receivedID = netId;
    }

    public override void RemoveVoiceHandler(VoiceHandler handler)
    {
        Handlers.Remove(handler.NetID);
    }
}