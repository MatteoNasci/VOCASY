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

        /// <summary>
        /// Exposed for tests. Internal collection of all registered handlers
        /// </summary>
        [NonSerialized]
        public Dictionary<ulong, VoiceHandler> Internal_handlers = new Dictionary<ulong, VoiceHandler>();

        /// <summary>
        /// Exposed for tests. Internal Single format audio data buffer
        /// </summary>
        [NonSerialized]
        public float[] Internal_dataBuffer;
        /// <summary>
        /// Exposed for tests. Internal Int16 format audio data buffer
        /// </summary>
        [NonSerialized]
        public byte[] Internal_dataBufferInt16;
        /// <summary>
        /// Exposed for tests. Internal BytePacket buffer
        /// </summary>
        [NonSerialized]
        public BytePacket Internal_packetBuffer;

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
            Internal_handlers.Add(handler.Identity.NetworkId, handler);
        }
        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">handler to remove</param>
        public void RemoveVoiceHandler(VoiceHandler handler)
        {
            //handler and callback are removed
            Internal_handlers.Remove(handler.Identity.NetworkId);
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
            Internal_packetBuffer.ResetSeekLength();

            //receive packet
            VoicePacketInfo info = Transport.ProcessReceivedData(Internal_packetBuffer, receivedData, startIndex, length, netId);

            //if packet is invalid or if there is not an handler for the given netid discard the packet received
            if (!info.ValidPacketInfo || !Internal_handlers.ContainsKey(info.NetId))
                return;

            VoiceHandler handler = Internal_handlers[info.NetId];

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
            Internal_packetBuffer.CurrentSeek = 0;

            //Different methods between Int16 and Single format. Data manipulation is done and, if no error occurred, audio data is sent to the handler in order to be used as output sound
            if (useSingle)
            {
                Manipulator.FromPacketToAudioData(Internal_packetBuffer, ref info, Internal_dataBuffer, 0, out count);
                if (info.ValidPacketInfo)
                    handler.ReceiveAudioData(Internal_dataBuffer, 0, count, info);
            }
            else
            {
                Manipulator.FromPacketToAudioDataInt16(Internal_packetBuffer, ref info, Internal_dataBufferInt16, 0, out count);
                if (info.ValidPacketInfo)
                    handler.ReceiveAudioDataInt16(Internal_dataBufferInt16, 0, count, info);
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
                info = handler.GetMicData(Internal_dataBuffer, 0, Internal_dataBuffer.Length, out count);
            else
                info = handler.GetMicDataInt16(Internal_dataBufferInt16, 0, Internal_dataBufferInt16.Length, out count);

            //if data is valid go on
            if (info.ValidPacketInfo)
            {
                //packet buffer used to create the final packet is prepared
                Internal_packetBuffer.ResetSeekLength();

                //data recovered from input is manipulated and stored into the gamepacket
                if (useSingle)
                    Manipulator.FromAudioDataToPacket(Internal_dataBuffer, 0, count, ref info, Internal_packetBuffer);
                else
                    Manipulator.FromAudioDataToPacketInt16(Internal_dataBufferInt16, 0, count, ref info, Internal_packetBuffer);

                Internal_packetBuffer.CurrentSeek = 0;

                //if packet is valid send to transport
                if (info.ValidPacketInfo)
                    Transport.SendToAllOthers(Internal_packetBuffer, info);
            }
        }
        /// <summary>
        /// Exposed for tests. Initializes workflow
        /// </summary>
        public void OnEnable()
        {
            if (Application.isPlaying)
            {
                if ((Manipulator.AvailableTypes & AudioDataTypeFlag.Single) != 0)
                    Internal_dataBuffer = new float[(VoiceChatSettings.MaxFrequency * VoiceChatSettings.MaxChannels) / 20];

                if ((Manipulator.AvailableTypes & AudioDataTypeFlag.Int16) != 0)
                    Internal_dataBufferInt16 = new byte[Internal_dataBuffer.Length * 2];

                Internal_packetBuffer = new BytePacket(Transport.MaxDataLength);
            }
        }
    }
}