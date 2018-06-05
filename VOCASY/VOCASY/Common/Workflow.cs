using System;
using System.Collections.Generic;
using UnityEngine;
using GENUtility;
using System.IO;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages the workflow of audio data from input to output
    /// </summary>
    [CreateAssetMenu(fileName = "VoiceManager", menuName = "VOCASY/Workflow")]
    public class Workflow : VoiceDataWorkflow
    {
        [Serializable]
        private class StatusesHolder
        {
            public List<ulong> IDs = new List<ulong>();
            public List<MuteStatus> Statuses = new List<MuteStatus>();
            public StatusesHolder()
            {
            }
        }

        /// <summary>
        /// Folder used to store data
        /// </summary>
        public string FolderName
        {
            get { return folderName; }
            set
            {
                if (value == null)
                    return;
                ClearSavedStatusesFiles();
                folderName = value;
                Awake();
                SaveCurrentMuteStatuses();
            }
        }
        /// <summary>
        /// File name used to store statuses
        /// </summary>
        public string FileName
        {
            get { return statusesFileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                ClearSavedStatusesFiles();
                statusesFileName = value;
                Awake();
                SaveCurrentMuteStatuses();
            }
        }
        /// <summary>
        /// Full path to file storing ids
        /// </summary>
        public string SavedDataFilePath { get; private set; }
        /// <summary>
        /// Full path to folder holding saved files
        /// </summary>
        public string SavedDataFolderPath { get; private set; }
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
        /// <summary>
        /// True if you wish to use files to hold net ids and their last saved mute statuses.
        /// </summary>
        public bool UseStoredIdsStatuses = true;

        [SerializeField]
        private string statusesFileName = "MuteStatuses.txt";
        [SerializeField]
        private string folderName = "Communication";

        private AudioDataTypeFlag formatToUse = AudioDataTypeFlag.Int16;

        private List<ulong> activeIdsToSendTo;

        private Dictionary<ulong, VoiceHandler> handlers;

        private Dictionary<ulong, MuteStatus> mutedIds;

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

            ulong id = handler.NetID;
            //handler is added and callback for when mic data is available is set on the handler
            handlers.Add(id, handler);

            IsHandlerMuted(id, handler.IsOutputMuted);

            if (!handler.IsRecorder && (mutedIds[id] & MuteStatus.RemoteHasMutedLocal) == 0)
                activeIdsToSendTo.Add(id);
        }
        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">handler to remove</param>
        public override void RemoveVoiceHandler(VoiceHandler handler)
        {
            ulong id = handler.NetID;

            if (handlers.Remove(id) && (mutedIds[id] & MuteStatus.RemoteHasMutedLocal) == 0 && !handler.IsRecorder)
                activeIdsToSendTo.Remove(id);
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
            if (!info.ValidPacketInfo || !handlers.ContainsKey(netId))
                return;

            VoiceHandler handler = handlers[netId];

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
                    Transport.SendToAll(packetBuffer, info, activeIdsToSendTo);
            }
        }
        /// <summary>
        /// Processes the ismuted message received
        /// </summary>
        /// <param name="isSelfMuted">true if local slient has been muted by the sender</param>
        /// <param name="senderID">message sender id</param>
        public override void ProcessIsMutedMessage(bool isSelfMuted, ulong senderID)
        {
            if (!mutedIds.ContainsKey(senderID))
                mutedIds.Add(senderID, MuteStatus.None);

            bool existsLocally = handlers.ContainsKey(senderID);
            if (existsLocally && handlers[senderID].IsRecorder)
                return;

            MuteStatus curr = mutedIds[senderID];

            bool isMutedRemotely = ((byte)curr & (byte)MuteStatus.RemoteHasMutedLocal) != 0;

            bool diff = isSelfMuted ? !isMutedRemotely : isMutedRemotely;

            if (diff)
            {
                if (!isMutedRemotely)
                {
                    mutedIds[senderID] = curr | MuteStatus.RemoteHasMutedLocal;
                    if (existsLocally)
                        activeIdsToSendTo.Remove(senderID);
                }
                else
                {
                    mutedIds[senderID] = curr & ~MuteStatus.RemoteHasMutedLocal;
                    if (existsLocally)
                        activeIdsToSendTo.Add(senderID);
                }
            }
        }
        /// <summary>
        /// Informs the workflow whenever an handler has been muted
        /// </summary>
        /// <param name="handlerNetId">handler obj net id</param>
        /// <param name="isMuted">is the handler muted</param>
        public override void IsHandlerMuted(ulong handlerNetId, bool isMuted)
        {
            if (!mutedIds.ContainsKey(handlerNetId))
                mutedIds.Add(handlerNetId, MuteStatus.None);

            if (!handlers.ContainsKey(handlerNetId) || handlers[handlerNetId].IsRecorder)
                return;

            MuteStatus curr = mutedIds[handlerNetId];

            bool isMutedLocally = ((byte)curr & (byte)MuteStatus.LocalHasMutedRemote) != 0;

            bool diff = isMuted ? !isMutedLocally : isMutedLocally;

            if (diff)
            {
                if (!isMutedLocally)
                {
                    mutedIds[handlerNetId] = curr | MuteStatus.LocalHasMutedRemote;
                }
                else
                {
                    mutedIds[handlerNetId] = curr & ~MuteStatus.LocalHasMutedRemote;
                }

                Transport.SendMessageIsMutedTo(handlerNetId, isMuted);
            }
        }
        /// <summary>
        /// Initializes workflow , done automatically when SO is loaded. If fields are either not setted when this method is called or changed afterwards the workflow will remain in an incorrect state untill it is re-initialized
        /// </summary>
        public override void Initialize()
        {
            handlers = new Dictionary<ulong, VoiceHandler>();

            activeIdsToSendTo = new List<ulong>();

            if (mutedIds == null)
            {
                mutedIds = new Dictionary<ulong, MuteStatus>();

                LoadSavedMuteStatuses();
            }

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
        /// <summary>
        /// Deletes if they exists all files used to store ids and mute statuses
        /// </summary>
        public void ClearSavedStatusesFiles()
        {
            if (File.Exists(SavedDataFilePath))
                File.Delete(SavedDataFilePath);
        }
        /// <summary>
        /// Loads mute statuses from file if available
        /// </summary>
        public void LoadSavedMuteStatuses()
        {
            if (UseStoredIdsStatuses && mutedIds != null)
            {
                StatusesHolder holder = new StatusesHolder();
                if (File.Exists(SavedDataFilePath))
                {
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(SavedDataFilePath), holder);

                    int length = Mathf.Min(holder.IDs.Count, holder.Statuses.Count);

                    for (int i = 0; i < length; i++)
                    {
                        mutedIds[holder.IDs[i]] = holder.Statuses[i];
                    }
                }
            }
        }
        /// <summary>
        /// Saves current clients mute statuses if available
        /// </summary>
        public void SaveCurrentMuteStatuses()
        {
            if (UseStoredIdsStatuses && mutedIds != null)
            {
                StatusesHolder holder = new StatusesHolder();

                foreach (KeyValuePair<ulong, MuteStatus> item in mutedIds)
                {
                    //Save only information about statuses != than None value and ignore RemoteHasMutedLocal status
                    MuteStatus status = item.Value & ~MuteStatus.RemoteHasMutedLocal;
                    if (status == MuteStatus.None)
                        continue;

                    holder.IDs.Add(item.Key);
                    holder.Statuses.Add(status);
                }

                if (!Directory.Exists(SavedDataFolderPath))
                    Directory.CreateDirectory(SavedDataFolderPath);

                File.WriteAllText(SavedDataFilePath, JsonUtility.ToJson(holder));
            }
        }
        void Awake()
        {
            SavedDataFolderPath = Path.Combine(Application.persistentDataPath, folderName);
            SavedDataFilePath = Path.Combine(SavedDataFolderPath, statusesFileName);
        }
        void OnDisable()
        {
            SaveCurrentMuteStatuses();

            activeIdsToSendTo = null;
            mutedIds = null;
            handlers = null;
            dataBuffer = null;
            dataBufferInt16 = null;
            packetBuffer = null;
        }
    }
}