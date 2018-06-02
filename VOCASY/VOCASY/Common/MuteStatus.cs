using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Used to describe mute status
    /// </summary>
    [Serializable]
    [Flags]
    public enum MuteStatus : byte
    {
        /// <summary>
        /// No status
        /// </summary>
        None = 0,
        /// <summary>
        /// Muted locally
        /// </summary>
        LocalHasMutedRemote = 1,
        /// <summary>
        /// Muted remotely
        /// </summary>
        RemoteHasMutedLocal = 2,
        /// <summary>
        /// Muted both locally and remotely
        /// </summary>
        Both = 1 | 2,
    }
}