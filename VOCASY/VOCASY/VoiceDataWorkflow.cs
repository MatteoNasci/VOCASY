using UnityEngine;
namespace VOCASY
{
    /// <summary>
    /// Class that manages the workflow of audio data from input to output
    /// </summary>
    public abstract class VoiceDataWorkflow : ScriptableObject
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
        /// Adds the handler. Handler should already be initialized before calling this method
        /// </summary>
        /// <param name="handler">handler to add</param>
        public abstract void AddVoiceHandler(VoiceHandler handler);
        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">handler to remove</param>
        public abstract void RemoveVoiceHandler(VoiceHandler handler);
        /// <summary>
        /// Process the received packet data.
        /// </summary>
        /// <param name="receivedData">received raw data</param>
        /// <param name="startIndex">received raw data start index</param>
        /// <param name="length">received raw data length</param>
        /// <param name="netId">sender net id</param>
        public abstract void ProcessReceivedPacket(byte[] receivedData, int startIndex, int length, ulong netId);
        /// <summary>
        /// Processes mic data from the given handler
        /// </summary>
        /// <param name="handler">handler which has available mic data</param>
        public abstract void ProcessMicData(VoiceHandler handler);
        /// <summary>
        /// Processes the ismuted message received
        /// </summary>
        /// <param name="isSelfMuted">true if local slient has been muted by the sender</param>
        /// <param name="senderID">message sender id</param>
        public abstract void ProcessIsMutedMessage(bool isSelfMuted, ulong senderID);
        /// <summary>
        /// Informs the workflow whenever an handler has been muted
        /// </summary>
        /// <param name="handler">handler obj</param>
        /// <param name="sendMsgOnlyIfDiffDetected">true if you wich to send a network message only when the mute status has changed</param>
        public abstract void IsHandlerMuted(VoiceHandler handler, bool sendMsgOnlyIfDiffDetected = true);
        /// <summary>
        /// Initializes workflow , done automatically when SO is loaded. If fields are either not setted when this method is called or changed afterwards the workflow will remain in an incorrect state untill a new call to this method is made with setted fields
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Initialize workflow
        /// </summary>
        protected virtual void OnEnable()
        {
            Initialize();
        }
    }
}