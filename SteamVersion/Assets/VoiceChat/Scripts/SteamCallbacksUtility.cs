using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
public static class SteamCallbacksUtility
{
    public static bool IsInitialized { get; private set; }
    static SteamCallbacksUtility()
    {
        IsInitialized = false;
    }
    public delegate void OnP2PSessionRequest(P2PSessionRequest_t info);
    public delegate void OnP2PSessionConnectFail(P2PSessionConnectFail_t info);
    public delegate void OnLobbyChatUpd(LobbyChatUpdate_t info);
    public delegate void OnLobbyEnter(LobbyEnter_t info);

    public static event OnP2PSessionRequest P2PSessionRequest;
    public static event OnP2PSessionConnectFail P2PSessionConnectFail;
    public static event OnLobbyChatUpd LobbyChatUpd;
    public static event OnLobbyEnter LobbyEnter;

    private static Callback<P2PSessionRequest_t> sessionRequest;
    private static Callback<P2PSessionConnectFail_t> sessionConnectFail;
    private static Callback<LobbyChatUpdate_t> lobbyChatUpd;
    private static Callback<LobbyEnter_t> lobbyEnter;

    public static void Init()
    {
        if (IsInitialized)
            return;
        IsInitialized = true;

        sessionRequest = Callback<P2PSessionRequest_t>.Create(info =>
        {
            if (P2PSessionRequest != null)
                P2PSessionRequest.Invoke(info);
        });

        sessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(info =>
        {
            if (P2PSessionConnectFail != null)
                P2PSessionConnectFail.Invoke(info);
        });

        lobbyChatUpd = Callback<LobbyChatUpdate_t>.Create(info =>
        {
            if (LobbyChatUpd != null)
                LobbyChatUpd.Invoke(info);
        });

        lobbyEnter = Callback<LobbyEnter_t>.Create(info =>
        {
            if (LobbyEnter != null)
                LobbyEnter.Invoke(info);
        });
    }
}