using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VOCASY;
using VOCASY.Common;

public class SupportWorkflow : VoiceDataWorkflow
{
    public byte[] receivedData;
    public ulong receivedID;
    public bool ProcessData = false;
    public Dictionary<ulong, VoiceHandler> Handlers = new Dictionary<ulong, VoiceHandler>();
    public Dictionary<ulong, VOCASY.Common.MuteStatus> HandlersMuteStatuses = new Dictionary<ulong, VOCASY.Common.MuteStatus>();
    public override void AddVoiceHandler(VoiceHandler handler)
    {
        Handlers.Add(handler.NetID, handler);
        HandlersMuteStatuses.Add(handler.NetID, VOCASY.Common.MuteStatus.None);
    }

    public override void Initialize()
    {
    }

    public override void IsHandlerMuted(VoiceHandler handler, bool sendMsgOnlyWithDiffDetected = true)
    {
        ulong handlerNetId = handler.NetID;

        if (!HandlersMuteStatuses.ContainsKey(handlerNetId))
            HandlersMuteStatuses.Add(handlerNetId, MuteStatus.None);

        bool isMuted = handler.IsOutputMuted;

        MuteStatus curr = HandlersMuteStatuses[handlerNetId];

        bool isMutedLocally = ((byte)curr & (byte)MuteStatus.LocalHasMutedRemote) != 0;

        bool diff = isMuted ? !isMutedLocally : isMutedLocally;

        if (diff)
        {
            HandlersMuteStatuses[handlerNetId] = !isMutedLocally ? curr | MuteStatus.LocalHasMutedRemote : curr & ~MuteStatus.LocalHasMutedRemote;
            Transport.SendMessageIsMutedTo(handlerNetId, isMuted);
        }
    }

    public override void ProcessIsMutedMessage(bool isSelfMuted, ulong senderID)
    {
        if (!HandlersMuteStatuses.ContainsKey(senderID))
            HandlersMuteStatuses.Add(senderID, MuteStatus.None);

        MuteStatus curr = HandlersMuteStatuses[senderID];

        HandlersMuteStatuses[senderID] = isSelfMuted ? curr | MuteStatus.RemoteHasMutedLocal : curr & ~MuteStatus.RemoteHasMutedLocal;
    }

    public override void ProcessMicData(VoiceHandler handler)
    {
        ProcessData = true;
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