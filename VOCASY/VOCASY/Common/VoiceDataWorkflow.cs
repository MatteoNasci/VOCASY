using System;
using System.Collections.Generic;
using VOCASY.Utility;
using UnityEngine;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages the workflow of audio data from input to output
    /// </summary>
    [CreateAssetMenu(fileName = "VoiceManager", menuName = "VOCASY/Workflow")]
    public class VoiceDataWorkflow : ScriptableObject
    {
        /// <summary>
        /// Voice chat settings
        /// </summary>
        public VoiceChatSettings Settings;
        /// <summary>
        /// Manipulator used
        /// </summary>
        public VoiceDataManipulator Manipulator;
        /// <summary>
        /// Transport used
        /// </summary>
        public VoiceDataTransport Transport;

        private Dictionary<ulong, VoiceHandler> handlers = new Dictionary<ulong, VoiceHandler>();

        private float[] micDataBuffer;
        private float[] receivedDataBuffer;
        private byte[] micDataBufferInt16;
        private byte[] receivedDataBufferInt16;
        private GamePacket packetReceiver;
        private GamePacket packetSender;

        void OnEnable()
        {
            if ((Manipulator.AvailableTypes & AudioDataTypeFlag.Single) != 0)
            {
                micDataBuffer = new float[(VoiceChatSettings.MaxFrequency * VoiceChatSettings.MaxChannels) / 20];
                receivedDataBuffer = new float[(VoiceChatSettings.MaxFrequency * VoiceChatSettings.MaxChannels) / 20];
            }
            if ((Manipulator.AvailableTypes & AudioDataTypeFlag.Int16) != 0)
            {
                micDataBufferInt16 = new byte[micDataBuffer.Length * 2];
                receivedDataBufferInt16 = new byte[receivedDataBuffer.Length * 2];
            }

            packetReceiver = GamePacket.CreatePacket(Transport.MaxDataLength);

            packetSender = GamePacket.CreatePacket(Transport.MaxDataLength);
        }
        /// <summary>
        /// Adds the handler. Handler should already be initialized before calling this method
        /// </summary>
        /// <param name="handler">handler to add</param>
        public void AddVoiceHandler(VoiceHandler handler)
        {
            //Compatibility check between handler to add and manipulator
            AudioDataTypeFlag res = Manipulator.AvailableTypes & handler.AvailableTypes;

            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator");

            //handler is added and callback for when mic data is available is set on the handler
            handlers.Add(handler.Identity.NetworkId, handler);
        }
        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">handler to remove</param>
        public void RemoveVoiceHandler(VoiceHandler handler)
        {
            //handler and callback are removed
            handlers.Remove(handler.Identity.NetworkId);
        }
        /// <summary>
        /// Process the received packet data.
        /// </summary>
        /// <param name="receivedData">received raw data</param>
        /// <param name="startIndex">received raw data start index</param>
        /// <param name="length">received raw data length</param>
        /// <param name="netId">sender net id</param>
        public void ProcessReceivedPacket(byte[] receivedData, int startIndex, int length, ulong netId)
        {
            //If voice chat is disabled do nothing
            if (!Settings.VoiceChatEnabled)
                return;

            //resets packet buffer
            packetReceiver.ResetSeekLength();

            //receive packet
            VoicePacketInfo info = Transport.ProcessReceivedData(packetReceiver, receivedData, startIndex, length, netId);

            //if packet is invalid or if there is not an handler for the given netid discard the packet received
            if (!info.ValidPacketInfo || !handlers.ContainsKey(info.NetId))
                return;

            VoiceHandler handler = handlers[info.NetId];

            //Do nothing if handler is either muted or if it is a recorder
            if (handler.IsOutputMuted || handler.IsRecorder)
                return;

            //Compatibility check between manipulator, handler and packet; if incompatible throw exception
            AudioDataTypeFlag res = Manipulator.AvailableTypes & handler.AvailableTypes & info.Format;

            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator and the received packet format");

            //determine which data format to use. Gives priority to Int16 format
            bool useSingle = (res & AudioDataTypeFlag.Int16) == 0;

            int count;
            //packet received Seek to zero to prepare for data manipulation
            packetReceiver.CurrentSeek = 0;

            //Different methods between Int16 and Single format. Data manipulation is done and, if no error occurred, audio data is sent to the handler in order to be used as output sound
            if (useSingle)
            {
                Manipulator.FromPacketToAudioData(packetReceiver, ref info, receivedDataBuffer, 0, out count);
                if (info.ValidPacketInfo)
                    handler.ReceiveAudioData(receivedDataBuffer, 0, count, info);
            }
            else
            {
                Manipulator.FromPacketToAudioDataInt16(packetReceiver, ref info, receivedDataBufferInt16, 0, out count);
                if (info.ValidPacketInfo)
                    handler.ReceiveAudioDataInt16(receivedDataBufferInt16, 0, count, info);
            }
        }
        /// <summary>
        /// Processes mic data from the given handler
        /// </summary>
        /// <param name="handler">handler which has available mic data</param>
        public void ProcessMicData(VoiceHandler handler)
        {
            //If voice chat is disabled or if the given handler is not a recorder do nothing
            if (!Settings.VoiceChatEnabled || Settings.MuteSelf || !handler.IsRecorder)
                return;

            //Compatibility check between manipulator and handler. If they are incompatible throw exception
            AudioDataTypeFlag res = handler.AvailableTypes & Manipulator.AvailableTypes;
            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator");

            VoicePacketInfo info;

            //determine which data format to use. Gives priority to Int16 format
            bool useSingle = (res & AudioDataTypeFlag.Int16) == 0;

            //Retrive data from handler input
            int count;
            if (useSingle)
                info = handler.GetMicData(micDataBuffer, 0, micDataBuffer.Length, out count);
            else
                info = handler.GetMicDataInt16(micDataBufferInt16, 0, micDataBufferInt16.Length, out count);

            //if data is valid go on
            if (info.ValidPacketInfo)
            {
                //packet buffer used to create the final packet is prepared
                packetSender.ResetSeekLength();

                //data recovered from input is manipulated and stored into the gamepacket
                if (useSingle)
                    Manipulator.FromAudioDataToPacket(micDataBuffer, 0, count, ref info, packetSender);
                else
                    Manipulator.FromAudioDataToPacketInt16(micDataBufferInt16, 0, count, ref info, packetSender);

                packetSender.CurrentSeek = 0;

                //if packet is valid send to transport
                if (info.ValidPacketInfo)
                    Transport.SendToAllOthers(packetSender, info);
            }
        }
    }
}