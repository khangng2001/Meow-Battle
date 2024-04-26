using Managers;
using Process;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TranslateToPlayerNetcode : MonoBehaviour
{
    private GameLauncher gameLauncher;
    private ListRoomManager listRoomManager;
    private HandlerSkinListSO handlerSkinListSO;

    //// === FUNC ===
    public static Func<PlayerDetails> OnGetInfoPlayer;
    public static Func<Lobby> OnGetInfoLobby;
    public static Func<List<ScriptableObject>> OnGetInfoSkinList;
    public static Func<List<ScriptableObject>> OnGetInfoExpressionList;
    public static Func<GameLauncher> OnGetGameLauncher;

    private void Awake()
    {
        gameLauncher = GetComponentInParent<GameLauncher>();
        listRoomManager = gameLauncher.GetComponentInChildren<ListRoomManager>();
        handlerSkinListSO = gameLauncher.GetComponentInChildren<HandlerSkinListSO>();

        //

        OnGetInfoPlayer = gameLauncher.GetPlayerDetail;
        OnGetInfoLobby = listRoomManager.GetLobbyCurrent;
        OnGetInfoSkinList = () => { return handlerSkinListSO.SkinListFullSO.skinSOList; };
        OnGetInfoExpressionList = () => { return handlerSkinListSO.SkinListFullSO.expressionSOList; };
        OnGetGameLauncher = () => { return gameLauncher; };
    }
}
