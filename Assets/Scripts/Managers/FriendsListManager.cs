using Managers;
using Newtonsoft.Json.Linq;
using Process;
using Services;
using SocketIOClient;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using WebSocketSharp;

public class FriendsListManager : MonoBehaviour
{
    public SocketIOUnity socket;
    private string serverUrlLink = "https://bomber-backend.onrender.com";

    [Header("ListFriends")]
    [SerializeField] private Transform contentFriends;
    [SerializeField] private GameObject friendsPrefab;

    [Header("FriendsRequest")]
    [SerializeField] private Transform contentFriendsRequest;
    [SerializeField] private GameObject friendsInvitePrefab;
    [SerializeField] private Transform contentFriendsRequestPopup;
    [SerializeField] private GameObject friendsRequestPopup;

    [Header("InviteRequest")]
    [SerializeField] private Transform contentInvtesRequestPopup;
    [SerializeField] private GameObject inviteRequestPopup;


    private GameLauncher gameLauncher;
    private ListRoomManager listRoomManager;
    private SignIn signIn;
    private Action<string> OnMessageListFriednds;

    [SerializeField] private Button closeBtn;

    private void Start()
    {
        Debug.Log("Start");
        var uri = new Uri(serverUrlLink);
        socket = new SocketIOUnity(uri);

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };

        socket.On("showListFriends", ShowFriends);

        socket.On("FriendsRequest", (unityID) =>
        {
            ShowFriendsRequest(unityID);
        });

        socket.On("InviteRequest", (unityID) =>
        {
            ShowInviteRequest(unityID);
        });

        //socket.On("status", (player) =>
        //{
        //    PlayerDetails playerDetails = JsonUtility.FromJson
        //    gameLauncher.SetPlayerDetail();
        //});

        socket.Connect();

        gameLauncher = GetComponentInParent<GameLauncher>();
        listRoomManager = gameLauncher.GetComponentInChildren<ListRoomManager>();
        signIn = transform.parent.transform.parent.GetComponentInChildren<SignIn>();

        closeBtn.onClick.AddListener(() => { gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame); });

        signIn.OnPlayerDetailsSignIn += EmitListFriends;
    }

    private void OnDestroy()
    {
        signIn.OnPlayerDetailsSignIn -= EmitListFriends;
    }

    private void ShowFriends(SocketIOResponse response)
    {
        JArray data = JArray.Parse(response.ToString());
        JToken jsonListFriends = data.First;
        JToken unityIDData = data.First.Next;

        Debug.Log("showFriends: " + unityIDData);
        Debug.Log(gameLauncher.GetPlayerDetail().unityID);
        if (unityIDData.ToString().Equals(gameLauncher.GetPlayerDetail().unityID))
        {
            try
            {
                // Start the coroutine to instantiate the prefabs
                UnityMainThreadDispatcher.Enqueue(async () =>
                {
                    PlayerDetails player = await SingletonAPI.Instance.GetOnePlayer(unityIDData.ToString());
                    gameLauncher.SetPlayerDetail(player);

                    foreach (Transform friendChild in contentFriends.gameObject.transform)
                    {
                        Destroy(friendChild.gameObject);
                    }

                    foreach (string f in player.listfriends)
                    {
                        PlayerDetails friend = await SingletonAPI.Instance.GetOnePlayer(f);

                        GameObject friendPrefab = Instantiate(friendsPrefab, contentFriends);
                        friendPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = friend.name;
                        Debug.Log("friend: " + friend.status + " " + friend.unityID);
                        // socket.on để đăng ký sự kiện trước thui
                        SocketOnStatus(friend, friendPrefab);
                        EmitUpdateStatus(friend.status, friend.unityID); // Chỉ hoạt động với những lần showlistfriend chạy lại từ đầu
                    }

                    //string jsonResponse = jsonListFriends.ToString();
                    //Debug.Log(jsonResponse);
                    //List<PlayerDetails> listFriends = JsonConvert.DeserializeObject<List<PlayerDetails>>(jsonResponse);
                    //if (listFriends != null)
                    //{
                    //    foreach (PlayerDetails friend in listFriends)
                    //    {
                    //        GameObject friendPrefab = Instantiate(friendsPrefab, contentFriends);
                    //        friendPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = friend.name;
                    //        Debug.Log("friend: "+ friend.status + " " + friend.unityID);
                    //        // socket.on để đăng ký sự kiện trước thui
                    //        socket.On("status", (Player) =>
                    //        {
                    //            Debug.Log("status: " + friend.name);

                    //            UnityMainThreadDispatcher.Enqueue(() =>
                    //            {
                    //                PlayerDetails frDetails = JsonUtility.FromJson<PlayerDetails>(Player.GetValue<object>().ToString());

                    //                if (frDetails.unityID.Equals(friend.unityID))
                    //                {
                    //                    Debug.Log("friend: " + friend.status + " " + friend.unityID);

                    //                    friendPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = frDetails.status.ToString();
                    //                    if (listRoomManager.GetLobbyCurrent() != null)
                    //                    {
                    //                        Debug.Log(listRoomManager.GetLobbyCurrent().Id);
                    //                        FetchPlayer.OnFetchFriendsInRoom?.Invoke(listRoomManager.GetLobbyCurrent().Id);
                    //                    }
                    //                    else
                    //                    {
                    //                        Debug.Log("hihu");
                    //                    }
                    //                }
                    //            }); 
                    //        });
                    //        EmitUpdateStatus(friend.status, friend.unityID); // Chỉ hoạt động với những lần showlistfriend chạy lại từ đầu
                    //        // You might want to use 'friend' here to set up the instantiated prefab based on the PlayerDetails data
                    //    }
                    //}

                    if (listRoomManager.GetLobbyCurrent() != null)
                    {
                        Debug.Log(listRoomManager.GetLobbyCurrent().Id);
                        FetchPlayer.OnFetchFriendsInRoom?.Invoke(listRoomManager.GetLobbyCurrent().Id);
                    }
                    else
                    {
                        Debug.Log("hihu");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error Friends List: " + ex.Message);
            }
        }
    }

    private void ShowFriendsRequest(SocketIOResponse objectUnityID)
    {
        try
        {
            string myUnityID = objectUnityID.GetValue<object[]>()[1].ToString(); //MyID cua nguoi nhan
            string friendsUnityID = objectUnityID.GetValue<object[]>()[0].ToString(); //FriendsID cua nguoi nhan (la id cua nguoi nhan emit)

            if (myUnityID == gameLauncher.GetPlayerDetail().unityID)
            {
                // Start the coroutine to instantiate the prefabs
                UnityMainThreadDispatcher.Enqueue(async () =>
                {
                    PlayerDetails fr = await SingletonAPI.Instance.GetOnePlayer(friendsUnityID);

                    GameObject friendsRequest = Instantiate(friendsInvitePrefab, contentFriendsRequest);
                    friendsRequest.GetComponentInChildren<TextMeshProUGUI>().text = fr.name;
                    friendsRequest.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { EmitAcceptFriendsRequest(myUnityID, friendsUnityID); Destroy(friendsRequest); });
                    friendsRequest.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Debug.Log("Decline"); Destroy(friendsRequest); });

                    StartCoroutine(PopupFriendsRequest(friendsRequest, myUnityID, friendsUnityID));
                    //friendsRequest.GetComponentInChildren<TextMeshProUGUI>().text = 
                });
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error Friends Request: " + ex.Message);
        }
    }

    private void ShowInviteRequest(SocketIOResponse objectUnityID)
    {
        UnityMainThreadDispatcher.Enqueue(async () =>
        {
            string myUnityID = objectUnityID.GetValue<object[]>()[1].ToString(); //MyID cua nguoi nhan
            string friendsUnityID = objectUnityID.GetValue<object[]>()[0].ToString();
            string value = objectUnityID.GetValue<object[]>()[2].ToString();

            if (myUnityID.Equals(gameLauncher.GetPlayerDetail().unityID))
            {
                PlayerDetails fr = await SingletonAPI.Instance.GetOnePlayer(friendsUnityID);

                StartCoroutine(PopupInviteRequest(fr, myUnityID, friendsUnityID, value));
            }
        });
    }

    private IEnumerator PopupInviteRequest(PlayerDetails playerDetails ,string myUnityID, string friendsUnityID, string value)
    {
        inviteRequestPopup.SetActive(true);
        GameObject inviteRequest = Instantiate(friendsInvitePrefab, contentInvtesRequestPopup);
        inviteRequest.GetComponentInChildren<TextMeshProUGUI>().text = playerDetails.name;
        inviteRequest.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => 
        {
            Debug.Log(value);
            gameLauncher.GetComponentInChildren<SearchRoom>().JoinRoomByInviteWithCode(value);
            Destroy(inviteRequest);
        });
        inviteRequest.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Destroy(inviteRequest); });

        yield return new WaitForSeconds(5);

        Destroy(inviteRequest);
        inviteRequestPopup.SetActive(false);
    }

    private IEnumerator PopupFriendsRequest(GameObject friendsRequest, string myUnityID, string friendsUnityID)
    {
        friendsRequestPopup.SetActive(true);
        GameObject friendsRequest1 = Instantiate(friendsRequest, contentFriendsRequestPopup);
        friendsRequest1.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { EmitAcceptFriendsRequest(myUnityID, friendsUnityID); Destroy(friendsRequest); Destroy(friendsRequest1); });
        friendsRequest1.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Debug.Log("Decline"); Destroy(friendsRequest); Destroy(friendsRequest1); });

        yield return new WaitForSeconds(5);

        Destroy(friendsRequest1);
        friendsRequestPopup.SetActive(false);
    }

    // Emit
    private void EmitListFriends(PlayerDetails player)
    {
        try
        {
            Debug.Log("ok");
            if (player == null)
            {
                throw new Exception("An Error during the Login: PlayerDeltail Null");
            }

            Debug.Log("player");
            socket.Emit("listFriends", player.unityID);
        }
        catch (Exception ex)
        {
            OnMessageListFriednds?.Invoke(ex.Message);
        }
    }

    public void EmitSendFriendsRequest(string MyunityID, string FriendsUnityid) // add listener for button "+" (add friends with member in room)
    {
        socket.Emit("SendFriendsRequest", new object[] { MyunityID, FriendsUnityid });
    }

    private void EmitAcceptFriendsRequest(string MyunityID, string FriendsUnityID)
    {
        socket.Emit("AcceptFriendsRequest", new object[] { FriendsUnityID, MyunityID });
    }

    public void EmitSendInviteRequest(string MyunityID, string FriendsUnityid, string value)
    {
        socket.Emit("SendInviteFRequest", new object[] { MyunityID, FriendsUnityid, value });
    }

    public void EmitUpdateStatus(int status, string unityID)
    {
        socket.Emit("updateStatus", status, unityID);
    }

    public void SocketOnStatus(PlayerDetails friend, GameObject friendPrefab)
    {
        var status = "New Texttttt";
        socket.On("status", (Player) =>
        {
            Debug.Log("status: " + friend.name);

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                PlayerDetails frDetails = JsonUtility.FromJson<PlayerDetails>(Player.GetValue<object>().ToString());

                if (frDetails.unityID.Equals(friend.unityID))
                {
                    Debug.Log("friend: " + friend.status + " " + friend.unityID);

                    if (friend.status == 0)
                    {
                        status = "Offline";
                    }
                    else if (friend.status == 2)
                    {
                        status = "In Room";
                    }
                    else
                    {
                        status = "Online";
                    }

                    friendPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = status;
                }
            });
        });
        //EmitUpdateStatus(friend.status, friend.unityID); // Chỉ hoạt động với những lần showlistfriend chạy lại từ đầu
    }

    public void SocketOnStatus1(PlayerDetails friend, GameObject friendPrefab)
    {
        var status = "New Texttttt";
        socket.On("status", (Player) =>
        {
            Debug.Log("status: " + friend.name);

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                PlayerDetails frDetails = JsonUtility.FromJson<PlayerDetails>(Player.GetValue<object>().ToString());

                if (frDetails.unityID.Equals(friend.unityID))
                {
                    Debug.Log("friend: " + friend.status + " " + friend.unityID);

                    if (friend.status == 0)
                    {
                        status = "Offline";
                    }
                    else if (friend.status == 2)
                    {
                        status = "In Room";
                    }
                    else
                    {
                        status = "Online";
                    }
                    friendPrefab.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = status;
                    //friendPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = frDetails.status.ToString();
                }
            });
        });
        //EmitUpdateStatus(friend.status, friend.unityID); // Chỉ hoạt động với những lần showlistfriend chạy lại từ đầu
    }

}
