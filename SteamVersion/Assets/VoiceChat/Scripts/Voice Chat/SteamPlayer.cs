using UnityEngine;
using Steamworks;
using VOCASY;
public class SteamPlayer : MonoBehaviour
{
    public VoiceHandler Handler;
    void Awake()
    {
        SteamCallbacksUtility.LobbyChatUpd += OnLobbyUpdate;
    }
    void OnDestroy()
    {
        SteamCallbacksUtility.LobbyChatUpd -= OnLobbyUpdate;
    }
    void OnLobbyUpdate(LobbyChatUpdate_t cb)
    {
        if (!Handler)
            return;

        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange != EChatMemberStateChange.k_EChatMemberStateChangeEntered && cb.m_ulSteamIDUserChanged == Handler.NetID)
            Destroy(Handler.gameObject);
    }
}