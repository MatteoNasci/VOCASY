namespace VOCASY
{
    /// <summary>
    /// Data structure that stores various data to identify the status of an audio packet
    /// </summary>
    public struct VoicePacketInfo
    {
        /// <summary>
        /// Invalid packet
        /// </summary>
        public static readonly VoicePacketInfo InvalidPacket;
        static VoicePacketInfo()
        {
            InvalidPacket = new VoicePacketInfo(0, 0, AudioDataTypeFlag.None, false);
        }
        /// <summary>
        /// Frequency at which data is stored
        /// </summary>
        public ushort Frequency;
        /// <summary>
        /// Channels at which data is stored
        /// </summary>
        public byte Channels;
        /// <summary>
        /// Is the packet valid ? false if there are problems
        /// </summary>
        public bool ValidPacketInfo;
        /// <summary>
        /// Format of the stored data
        /// </summary>
        public AudioDataTypeFlag Format;
        /// <summary>
        /// Create a new struct instance
        /// </summary>
        /// <param name="frequency">audio frequency</param>
        /// <param name="channels">audio channels</param>
        /// <param name="format">audio format</param>
        /// <param name="valid">is packet valid?</param>
        public VoicePacketInfo(ushort frequency, byte channels, AudioDataTypeFlag format, bool valid = true)
        {
            Frequency = frequency;
            Channels = channels;
            Format = format;
            ValidPacketInfo = valid;
        }
    }
}