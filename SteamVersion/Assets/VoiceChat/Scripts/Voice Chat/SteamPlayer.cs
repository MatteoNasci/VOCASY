using UnityEngine;
using Steamworks;
using VOCASY;
[RequireComponent(typeof(IVoiceHandler))]
public class SteamPlayer : MonoBehaviour
{
    private IVoiceHandler handler;
    void Awake()
    {
        SteamCallbacksUtility.LobbyChatUpd += OnLobbyUpdate;
        handler = GetComponent<IVoiceHandler>();
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