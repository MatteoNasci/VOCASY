using UnityEngine;
using GENUtility;
using System.Collections.Generic;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that fakes network state between self and another client. It should be used for tests and debug
    /// </summary>
    [CreateAssetMenu(fileName = "SelfTransport", menuName = "VOCASY/DataTransports/Self")]
    public class SelfDataTransport : Transport
    {
        /// <summary>
        /// Max final packet length
        /// </summary>
        public const int PSelfLength = 5120;
        /// <summary>
        /// Max data length that should be sent to this class
        /// </summary>
        public override int MaxDataLength { get { return PSelfLength - FirstPacketByteAvailable; } }
        private void Awake()
        {
            SendToAllAction = SendAll;
            SendMsgTo = null;
        }
        private void SendAll(byte[] data, int startIndex, int length, List<ulong> receiversIds)
        {
            for (int i = 0; i < receiversIds.Count; i++)
            {
                Workflow.ProcessReceivedPacket(data, startIndex, length, receiversIds[i]);
            }
        }
    }
}