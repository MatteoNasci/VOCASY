using UnityEngine;
using VOCASY;
public class TestNetworkIdentity : MonoBehaviour, INetworkIdentity
{
    public uint NetworkId { get; set; }
    public bool IsLocalPlayer { get; set; }
}