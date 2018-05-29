using System;
using System.Collections.Generic;
using UnityEngine;
using GENUtility;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages the workflow of audio data from input to output
    /// </summary>
    [CreateAssetMenu(fileName = "VoiceManager", menuName = "VOCASY/Workflow")]
    public class Workflow : VoiceDataWorkflow
    {
        /// <summary>
        /// Format to use. This is only a preference and will only be followed when more than 1 format is available for use in the current setup. It can only refer to a single format
        /// </summary>
        public AudioDataTypeFlag FormatToUse
        {
            get { return formatToUse; }
            set
            {
                if (value == AudioDataTypeFlag.None || value == AudioDataTypeFlag.Both)
                    return;
                formatToUse = value;
            }
        }
        private AudioDataTypeFlag formatToUse = AudioDataTypeFlag.Int16;

        private Dictionary<ulong, VoiceHandler> handlers;

        private float[] dataBuffer;
        private byte[] dataBufferInt16;
        private BytePacket packetBuffer;

        /// <summary>
        /// Adds the handler. Handler should already be initialized before calling this method
        /// </summary>
        /// <param name="handler">handler to add</param>
        public override void AddVoiceHandler(VoiceHandler handler)
        {
            //Compatibility check between handler to add and manipulator
            AudioDataTypeFlag res = Manipulator.AvailableTypes & handler.AvailableTypes;

            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator");

            //handler is added and callback for when mic data is available is set on the handler
            handlers.Add(handler.NetID, handler);
        }
        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">handler to remove</param>
        public override void RemoveVoiceHandler(VoiceHandler handler)
        {
            //handler and callback are removed
            handlers.Remove(handler.NetID);
        }
        /// <summary>
        /// Process the received packet data.
        /// </summary>
        /// <param name="receivedData">received raw data</param>
        /// <param name="startIndex">received raw data start index</param>
        /// <param name="length">received raw data length</param>
        /// <param name="netId">sender net id</param>
        public override void ProcessReceivedPacket(byte[] receivedData, int startIndex, int length, ulong netId)
        {
            //If voice chat is disabled do nothing
            if (!Settings.VoiceChatEnabled)
                return;

            //resets packet buffer
            packetBuffer.ResetSeekLength();

            //receive packet
            VoicePacketInfo info = Transport.ProcessReceivedData(packetBuffer, receivedData, startIndex, length, netId);

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

            //determine which data format to use.
            if (res == AudioDataTypeFlag.Both)
                res = formatToUse;

            bool useSingle = res == AudioDataTypeFlag.Single;

            //packet received Seek to zero to prepare for data manipulation
            packetBuffer.CurrentSeek = 0;

            //Different methods between Int16 and Single format. Data manipulation is done and, if no error occurred, audio data is sent to the handler in order to be used as output sound
            if (useSingle)
            {
                int count = Manipulator.FromPacketToAudioData(packetBuffer, ref info, dataBuffer, 0);
                if (info.ValidPacketInfo)
                    handler.ReceiveAudioData(dataBuffer, 0, count, info);
            }
            else
            {
                int count = Manipulator.FromPacketToAudioDataInt16(packetBuffer, ref info, dataBufferInt16, 0);
                if (info.ValidPacketInfo)
                    handler.ReceiveAudioDataInt16(dataBufferInt16, 0, count, info);
            }
        }
        /// <summary>
        /// Processes mic data from the given handler
        /// </summary>
        /// <param name="handler">handler which has available mic data</param>
        public override void ProcessMicData(VoiceHandler handler)
        {
            //If voice chat is disabled or if the given handler is not a recorder do nothing
            if (!Settings.VoiceChatEnabled || Settings.MuteSelf || !handler.IsRecorder)
                return;

            //Compatibility check between manipulator and handler. If they are incompatible throw exception
            AudioDataTypeFlag res = handler.AvailableTypes & Manipulator.AvailableTypes;
            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator");

            VoicePacketInfo info;

            //determine which data format to use.
            if (res == AudioDataTypeFlag.Both)
                res = formatToUse;

            bool useSingle = res == AudioDataTypeFlag.Single;

            //Retrive data from handler input
            int count;
            if (useSingle)
                info = handler.GetMicData(dataBuffer, 0, dataBuffer.Length, out count);
            else
                info = handler.GetMicDataInt16(dataBufferInt16, 0, dataBufferInt16.Length, out count);

            //if data is valid go on
            if (info.ValidPacketInfo)
            {
                //packet buffer used to create the final packet is prepared
                packetBuffer.ResetSeekLength();

                //data recovered from input is manipulated and stored into the gamepacket
                if (useSingle)
                    Manipulator.FromAudioDataToPacket(dataBuffer, 0, count, ref info, packetBuffer);
                else
                    Manipulator.FromAudioDataToPacketInt16(dataBufferInt16, 0, count, ref info, packetBuffer);

                packetBuffer.CurrentSeek = 0;

                //if packet is valid send to transport
                if (info.ValidPacketInfo)
                    Transport.SendToAllOthers(packetBuffer, info);
            }
        }
        /// <summary>
        /// Initializes workflow , done automatically when SO is loaded. If fields are either not setted when this method is called or changed afterwards the workflow will remain in an incorrect state untill it is re-initialized
        /// </summary>
        public override void Initialize()
        {
            handlers = new Dictionary<ulong, VoiceHandler>();

            if (Manipulator)
            {
                int length = (Settings.MaxFrequency * Settings.MaxChannels) / 20;

                dataBuffer = (Manipulator.AvailableTypes & AudioDataTypeFlag.Single) != 0 ? new float[length] : null;

                dataBufferInt16 = (Manipulator.AvailableTypes & AudioDataTypeFlag.Int16) != 0 ? new byte[length * 2] : null;
            }
            else
            {
                dataBuffer = null;
                dataBufferInt16 = null;
            }

            packetBuffer = Transport ? new BytePacket(Transport.MaxDataLength) : null;
        }
        void OnDisable()
        {
            handlers = null;
            dataBuffer = null;
            dataBufferInt16 = null;
            packetBuffer = null;
        }
    }
}