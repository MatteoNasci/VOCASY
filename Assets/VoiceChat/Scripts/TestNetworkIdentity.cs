using UnityEngine;
public class TestNetworkIdentity : MonoBehaviour, INetworkIdentity
{
    public ulong NetworkId { get; set; }
    public bool IsLocalPlayer { get; set; }
}