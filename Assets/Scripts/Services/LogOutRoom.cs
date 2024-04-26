using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace Services
{
    public class LogOutRoom : MonoBehaviour
    {
        [SerializeField] private GameObject containPlayer;
        [SerializeField] private GameObject buddyContent;
        
        [Header("EVENTS TO SUBSCRIBE")]
        private GameLauncher gameLauncher;
        private InRoomManager inRoomManager;
        private ListRoomManager listRoomManager;

        [Header("EVENTS TO PUBLISH")]
        public Action OnExitRoomEvent;
        public Action<string> OnExitRoomEventMessage;
        
        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            listRoomManager = GetComponentInParent<ListRoomManager>();
            inRoomManager = GetComponent<InRoomManager>();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            inRoomManager.OnLogOutRoomAction = async () =>
            {
                await OnExitRoom();
            };
        }

        private async Task OnExitRoom()
        {
            try
            {
                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                //playerDetails return playerDetailsCurrent;
                await DelegateHostRole();
                await listRoomManager.SubscribeLobbyEvents(false);
                await LobbyService.Instance.RemovePlayerAsync(listRoomManager.GetLobbyCurrent().Id, gameLauncher.GetPlayerDetail().unityID);
                listRoomManager.SetCurrentHostLobby(null, null);
                OnExitRoomEvent?.Invoke(); //event invoke to fetch the list room for exited player
                inRoomManager.SetCodeRoom("");
                inRoomManager.SwitchButtonState(StartGameButtonState.None);
                DestroyPlayerInRoom();
                gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                OnExitRoomEventMessage?.Invoke(e.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }
        }

        //private async void OnApplicationQuit()
        //{
        //    // await OnExitRoom();
        //}

        private async Task DelegateHostRole()
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(listRoomManager.GetLobbyCurrent().Id);
                if (listRoomManager.GetLobbyHost() != null && lobby.Players.Count > 1)
                {
                    UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions();
                    updateLobbyOptions.HostId = lobby.Players[1].Id;
                    lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                await DelegateHostRole();
            }
        }

        private void DestroyPlayerInRoom()
        {
            foreach (Transform child in containPlayer.transform)
            {
                Destroy(child.gameObject);
            }

            //foreach (Transform memberChild in buddyContent.gameObject.transform)
            //{
            //    Destroy(memberChild.gameObject);
            //}
        }
    }
}
