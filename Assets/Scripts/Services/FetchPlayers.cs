using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Managers;
using Process;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace Services
{
    public class FetchPlayers : MonoBehaviour
    {
        [SerializeField] private Transform[] standings;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject containPlayer;
        [SerializeField] private Material skinCurrent;
        [SerializeField] private Material expressionCurrent;
        private Dictionary<string, GameObject> playerList = new Dictionary<string, GameObject>();

        private ListRoomManager listRoomManager;
        private GameLauncher gameLauncher;
        private CreateRoom createRoom;
        private FetchListRooms fetchListRooms;
        private SearchRoom searchRoom;
        [SerializeField] private SkinListSO skinListFullSO;

        public Action OnCheckHostLobby;
        public static Action<string> OnFetchFriendsInRoom;

        [Header("Buddy")]
        [SerializeField] private Transform buddyContent;
        [SerializeField] private GameObject memberPrefab;
        [SerializeField] private Transform inviteContent;
        [SerializeField] private GameObject invitePrefab;

        [SerializeField] private FriendsListManager friendsListManager;

        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            listRoomManager = GetComponentInParent<ListRoomManager>();
            createRoom = listRoomManager?.GetComponent<CreateRoom>();
            fetchListRooms = listRoomManager?.GetComponent<FetchListRooms>();
            searchRoom = listRoomManager?.GetComponent<SearchRoom>();

            SubscribeEvent();
        }

        private async Task FetchPlayersInRoom(string lobbyID)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);
                PlayerDetails playerDetails = gameLauncher.GetPlayerDetail();

                // Get Skins
                List<Material> skins = new List<Material>();
                foreach (SkinSO item in skinListFullSO.skinSOList)
                {
                    skins.Add(item.GetSkinMaterial());
                }
                // Get Expression
                List<Material> expressions = new List<Material>();
                foreach (SkinSO item in skinListFullSO.expressionSOList)
                {
                    expressions.Add(item.GetSkinMaterial());
                }

                foreach (Transform child in containPlayer.transform)
                {
                    Destroy(child.gameObject);
                }
                playerList.Clear();

                foreach (Player player in lobby.Players)
                {
                    GameObject playerExisted = Instantiate(playerPrefab,
                        standings[lobby.Players.IndexOf(player)].transform.position,
                        standings[lobby.Players.IndexOf(player)].transform.rotation,
                        containPlayer.transform);

                    // If Create Other Player, Apply Attribute Of Other Player (like: Skin, Expression,...)
                    if (player.Id != playerDetails.unityID)
                    {
                        // Find Skin And Express In Asset
                        Material skinFound = skins.Find((Material material) => { if (material.name == player.Data["PlayerSkin"].Value) return true; return false; });
                        Material expressionFound = expressions.Find((Material material) => { if (material.name == player.Data["PlayerExpression"].Value) return true; return false; });

                        // Apply
                        playerExisted.GetComponentInChildren<SkinnedMeshRenderer>().SetMaterials(new List<Material>()
                        {
                            skinFound,
                            expressionFound
                        });
                    }
                    else // If Myself
                    {
                        Material skinFinded = skins.Find((Material material) => { if (material.name == player.Data["PlayerSkin"].Value) return true; return false; });
                        Material expressionFinded = expressions.Find((Material material) => { if (material.name == player.Data["PlayerExpression"].Value) return true; return false; });

                        if (skinFinded != null) skinCurrent.CopyPropertiesFromMaterial(skinFinded);
                        if (expressionFinded != null) expressionCurrent.CopyPropertiesFromMaterial(expressionFinded);
                    }

                    playerList.Add(player.Data["PlayerID"].Value, playerExisted);
                }

                OnFetchFriendsInRoom?.Invoke(lobbyID);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                await FetchPlayersInRoom(listRoomManager.GetLobbyCurrent().Id);
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }

        }

        private async void FetchFriendsInRoom(string lobbyID)
        {
            try
            {
                Debug.Log("ok");
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);
                PlayerDetails playerDetails = gameLauncher.GetPlayerDetail();

                foreach (Transform memberChild in buddyContent.gameObject.transform)
                {
                    Destroy(memberChild.gameObject);
                }

                foreach (Transform inviteChild in inviteContent.gameObject.transform)
                {
                    Destroy(inviteChild.gameObject);
                }

                foreach (Player player in lobby.Players)
                {
                    Debug.Log(player);
                    GameObject buddyMember = Instantiate(memberPrefab, buddyContent);
                    buddyMember.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = player.Data["PlayerName"].Value;

                    //listRoomManager.GetLobbyCurrent().Id;
                    if (player.Id == playerDetails.unityID)
                    {
                        //buddyMember.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        //{
                        //    friendsListManager.EmitSendFriendsRequest(playerDetails.unityID, player.Id);
                        //});
                        buddyMember.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    else
                    {
                        string friend = "";
                        Debug.Log(playerDetails.listfriends.Length);
                        if (playerDetails.listfriends != null)
                        {
                            foreach (string friends in playerDetails.listfriends)
                            {
                                Debug.Log(friends);
                                if (friends == player.Id)
                                {
                                    //buddyMember.transform.GetChild(1).gameObject.SetActive(false);
                                    friend = friends;
                                    break;
                                }
                            }
                        }

                        if (friend == player.Id)
                        {
                            Debug.Log(friend);
                            buddyMember.GetComponentInChildren<Button>().interactable = false;
                            //buddyMember.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        else
                        {
                            Debug.Log("Check: " + friend + " " + player.Id);
                            buddyMember.GetComponentInChildren<Button>().onClick.AddListener(() =>
                            {
                                friendsListManager.EmitSendFriendsRequest(playerDetails.unityID, player.Id);
                                buddyMember.GetComponentInChildren<Button>().interactable = false;
                                //buddyMember..transform.GetChild(1).GetComponent<Image>().color.
                            });
                        }
                    }
                }

                //if (playerDetails.listfriends == null)
                //{
                //    throw new Exception("listFriends null");
                //}

                foreach (string f in playerDetails.listfriends)
                {
                    PlayerDetails friend = await SingletonAPI.Instance.GetOnePlayer(f);
                    GameObject invite = Instantiate(invitePrefab, inviteContent);
                    invite.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = friend.name;
                    //invite.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = friend.status.ToString();
                    friendsListManager.SocketOnStatus1(friend, invite);
                    if (friend.status == 1)
                    {
                        invite.GetComponentInChildren<Button>().onClick.AddListener(() => { friendsListManager.EmitSendInviteRequest(gameLauncher.GetPlayerDetail().unityID, f, listRoomManager.GetLobbyCurrent().LobbyCode); });
                    }
                    else
                    {
                        invite.GetComponentInChildren<Button>().interactable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                FetchFriendsInRoom(lobbyID);
                Debug.LogWarning(ex.Message);
            }
        }

        //private void CreateMember(string name)
        //{
        //    Instantiate();
        //}

        private async void FetchAllPlayer(string lobbyID)
        {
            await FetchPlayersInRoom(lobbyID);
            OnCheckHostLobby?.Invoke();
        }

        private void SubscribeEvent()
        {
            OnFetchFriendsInRoom += FetchFriendsInRoom;
            createRoom.OnFetchPlayer += FetchAllPlayer;
            fetchListRooms.OnFetchPlayer += FetchAllPlayer;
            listRoomManager.OnFetchPlayer += FetchAllPlayer;
            searchRoom.OnFetchPlayer += FetchAllPlayer;
        }

        private void UnsubscribeEvent()
        {
            OnFetchFriendsInRoom -= FetchFriendsInRoom;
            createRoom.OnFetchPlayer -= FetchAllPlayer;
            fetchListRooms.OnFetchPlayer -= FetchAllPlayer;
            listRoomManager.OnFetchPlayer -= FetchAllPlayer;
            searchRoom.OnFetchPlayer -= FetchAllPlayer;
        }

        private void OnDestroy()
        {
            UnsubscribeEvent();
        }

        // GET - SET
        public Dictionary<string, GameObject> PLayerList
        {
            get
            {
                return playerList;
            }
        }
    }
}
