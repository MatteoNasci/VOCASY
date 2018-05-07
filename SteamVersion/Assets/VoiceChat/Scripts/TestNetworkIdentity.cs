using UnityEngine;
using VOCASY;
public class TestNetworkIdentity : MonoBehaviour, INetworkIdentity
{
    public ulong NetworkId { get; set; }
    public bool IsLocalPlayer { get; set; }
}