namespace VOCASY
{
    /// <summary>
    /// Enum that holds most common frequencies used
    /// </summary>
    public enum FrequencyType : ushort
    {
        /// <summary>
        /// Low end frequency quality
        /// </summary>
        LowerThanAverageQuality = 12000,
        /// <summary>
        /// Normal frequency quality
        /// </summary>
        VoipQuality = 16000,
        /// <summary>
        /// Above normal frequency quality
        /// </summary>
        AboveAverageQuality = 24000,
        /// <summary>
        /// High frequency quality
        /// </summary>
        HighQuality = 44100,
        /// <summary>
        /// Best frequency quality
        /// </summary>
        BestQuality = 48000,
    }
}