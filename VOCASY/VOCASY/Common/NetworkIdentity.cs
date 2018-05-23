using System;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that represents an object which will be treated as unique on the nerwork
    /// </summary>
    [Serializable]
    public class NetworkIdentity
    {
        /// <summary>
        /// Unique network id that identifies this specific object in the network. This value should not change
        /// </summary>
        public ulong NetworkId;
        /// <summary>
        /// True if this object is owned by the local player. This value should not change
        /// </summary>
        public bool IsLocalPlayer;
        /// <summary>
        /// True if INetworkIdentity has been set correctly
        /// </summary>
        public bool IsInitialized;
    }
}