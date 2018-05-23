using UnityEngine;
using Steamworks;
using VOCASY;
using VOCASY.Common;
[RequireComponent(typeof(VoiceHandler))]
public class SteamPlayer : MonoBehaviour
{
    private VoiceHandler handler;
    void Awake()
    {
        SteamCallbacksUtility.LobbyChatUpd += OnLobbyUpdate;
        handler = GetComponent<VoiceHandler>();
    }
    void OnDestroy()
    {
        SteamCallbacksUtility.LobbyChatUpd -= OnLobbyUpdate;
    }
    void OnLobbyUpdate(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange != EChatMemberStateChange.k_EChatMemberStateChangeEntered && cb.m_ulSteamIDUserChanged == handler.Identity.NetworkId)
            Destroy(this.gameObject);
    }
}