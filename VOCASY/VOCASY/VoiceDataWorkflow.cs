using System;
using System.Collections.Generic;
using VOCASY.Utility;
/// <summary>
/// Class that manages the workflow of audio data from input to output
/// </summary>
namespace VOCASY
{
    public static class VoiceDataWorkflow
    {
        /// <summary>
        /// Voice chat settings
        /// </summary>
        public static IVoiceChatSettings Settings { get; private set; }

        private static IAudioDataManipulator manipulator;
        private static IAudioTransportLayer transport;

        private static Dictionary<uint, IVoiceHandler> handlers = new Dictionary<uint, IVoiceHandler>();

        private static float[] micDataBuffer = new float[4800];
        private static float[] receivedDataBuffer = new float[4800];
        private static byte[] micDataBufferInt16 = new byte[19200];
        private static byte[] receivedDataBufferInt16 = new byte[19200];
        private static GamePacket packetReceiver;
        private static GamePacket packetSender;

        /// <summary>
        /// Initializes the workflow
        /// </summary>
        /// <param name="manipulator">manipulator to use</param>
        /// <param name="transport">transport to use</param>
        public static void Init(IAudioDataManipulator dataManipulator, IAudioTransportLayer transportLayer, IVoiceChatSettings settings)
        {
            Settings = settings;

            //if a transport is already set remove the callback
            if (transport != null)
                transport.SetOnPacketAvailable(null);

            //transport and manipulator are set. A callback for when data is available is set on the transport
            transport = transportLayer;
            transport.SetOnPacketAvailable(OnPacketAvailable);

            manipulator = dataManipulator;

            //packets are initialized
            if (packetReceiver != null)
                packetReceiver.DisposePacket();
            packetReceiver = GamePacket.CreatePacket((int)transport.MaxPacketLength);

            if (packetSender != null)
                packetSender.DisposePacket();
            packetSender = GamePacket.CreatePacket((int)transport.MaxPacketLength);
        }
        /// <summary>
        /// Adds the handler
        /// </summary>
        /// <param name="handler">handler to add</param>
        public static void AddVoiceHandler(IVoiceHandler handler)
        {
            //check if workflow is initialized
            if (manipulator == null)
                throw new Exception("The current manipulator is null, be sure to initialize the workflow properly before any other action");

            //Compatibility check between handler to add and manipulator
            AudioDataTypeFlag res = manipulator.AvailableTypes & handler.AvailableTypes;

            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator");

            //handler is added and callback for when mic data is available is set on the handler
            handlers.Add(handler.Identity.NetworkId, handler);
            handler.SetOnMicDataProcessed(OnMicDataProcessed);
        }
        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">handler to remove</param>
        public static void RemoveVoiceHandler(IVoiceHandler handler)
        {
            //handler and callback are removed
            handlers.Remove(handler.Identity.NetworkId);
            handler.SetOnMicDataProcessed(null);
        }
        private static void OnPacketAvailable()
        {
            //throw exception if workflow is not initialized correctly
            if (manipulator == null || transport == null)
                throw new Exception("The current manipulator or transport is null, be sure to initialize the workflow properly before any other action");

            //If voice chat is disabled do nothing
            if (!Settings.VoiceChatEnabled)
                return;

            //Cycle that iterates as long as there are packets available in the transport
            while (transport.IsPacketAvailable)
            {
                //resets packet buffer
                packetReceiver.ResetSeekLength();
                //receive packet
                VoicePacketInfo info = transport.Receive(packetReceiver);

                //if packet is invalid or if there is not an handler for the given netid discard the packet received
                if (!info.ValidPacketInfo || !handlers.ContainsKey(info.NetId))
                    continue;

                IVoiceHandler handler = handlers[info.NetId];

                //Do nothing if handler is either muted or if it is a recorder
                if (handler.IsOutputMuted || handler.IsRecorder)
                    return;

                //Compatibility check between manipulator, handler and packet; if incompatible throw exception
                AudioDataTypeFlag res = manipulator.AvailableTypes & handler.AvailableTypes & info.Format;

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
                    manipulator.FromPacketToAudioData(packetReceiver, ref info, receivedDataBuffer, 0, out count);
                    if (info.ValidPacketInfo)
                        handler.ReceiveAudioData(receivedDataBuffer, 0, count, info);
                }
                else
                {
                    manipulator.FromPacketToAudioDataInt16(packetReceiver, ref info, receivedDataBufferInt16, 0, out count);
                    if (info.ValidPacketInfo)
                        handler.ReceiveAudioDataInt16(receivedDataBufferInt16, 0, count, info);
                }
            }
        }
        private static void OnMicDataProcessed(IVoiceHandler handler)
        {
            //throw exception if workflow is not initialized correctly
            if (manipulator == null || transport == null)
                throw new Exception("The current manipulator or transport is null, be sure to initialize the workflow properly before any other action");

            //If voice chat is disabled or if the given handler is not a recorder do nothing
            if (!Settings.VoiceChatEnabled || Settings.MuteSelf || !handler.IsRecorder)
                return;

            //Compatibility check between manipulator and handler. If they are incompatible throw exception
            AudioDataTypeFlag res = handler.AvailableTypes & manipulator.AvailableTypes;
            if (res == AudioDataTypeFlag.None)
                throw new ArgumentException("the given handler type is incompatible with the current audio data manipulator");

            VoicePacketInfo info;

            //determine which data format to use. Gives priority to Int16 format
            bool useSingle = (res & AudioDataTypeFlag.Int16) == 0;

            //Retrive data from handler input
            uint count;
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
                    manipulator.FromAudioDataToPacket(micDataBuffer, 0, (int)count, ref info, packetSender);
                else
                    manipulator.FromAudioDataToPacketInt16(micDataBufferInt16, 0, (int)count, ref info, packetSender);

                //if packet is valid send to transport
                if (info.ValidPacketInfo)
                    transport.SendToAllOthers(packetSender, info);
            }
        }
    }
}