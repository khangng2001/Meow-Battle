using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Matchmaker;
using UnityEngine;
using System.Threading.Tasks;
using Managers;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

public class StartGameInRoom : MonoBehaviour
{
	private GameLauncher gameLauncher;
	private Lobby currentLobbyTemp;
	private ListRoomManager listRoomManager;
	private InRoomManager inRoomManager;

	private void Awake()
	{
		gameLauncher = GetComponentInParent<GameLauncher>();
		listRoomManager = GetComponentInParent<ListRoomManager>();
		inRoomManager = GetComponent<InRoomManager>();

		SubscribeEvents();
	}

	private void Update()
	{
		if (currentLobbyTemp != null)
		{
			if (currentLobbyTemp.Data["Ip"].Value != "")
			{
				gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
				if (NetworkManager.Singleton.IsClient) NetworkManager.Singleton.Shutdown();

                listRoomManager.SetCurrentHostLobby(listRoomManager.GetLobbyHost(), currentLobbyTemp);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(currentLobbyTemp.Data["Ip"].Value, ushort.Parse(currentLobbyTemp.Data["Port"].Value));
				NetworkManager.Singleton.StartClient();
				
				Debug.Log($"Ip:{currentLobbyTemp.Data["Ip"].Value} ,Port:{ushort.Parse(currentLobbyTemp.Data["Port"].Value)}");
				//gameLauncher.UpdatePageState(GameLauncher.PageState.InGame);
			}

			currentLobbyTemp = null;
		}
	}
	
	private void OnDestroy()
	{
		UnsubscribeEvents();
	}

	private void SubscribeEvents()
	{
		listRoomManager.OnCancelTicket += CancelTicket;
		inRoomManager.OnStartGameAction += OnStartGame;
		//inRoomManager.OnStartGameAction += OnStartGameTesting;
	}
	private void UnsubscribeEvents()
	{
        listRoomManager.OnCancelTicket -= CancelTicket;
		inRoomManager.OnStartGameAction -= OnStartGame;
		//inRoomManager.OnStartGameAction -= OnStartGameTesting;
	}

	// ========================= TEST =======================
	private async void OnStartGameTesting()
	{
        gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
		string ip = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
		string port = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port.ToString();
		await UpdateIPPortToLobby(ip, port);
    }

    private async Task UpdateIPPortToLobby(string Ip, string Port)
    {
        try  
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId: listRoomManager.GetLobbyCurrent().Id);

			// HIDE LOBBY
			listRoomManager.HideLobby(lobby);

			while (lobby.Data["Ip"].Value == Ip || lobby.Data["Port"].Value == Port)
			{
				await Task.Delay(500);
                UpdateLobbyOptions resetIpPortOptions = new UpdateLobbyOptions();
                resetIpPortOptions.Data = new Dictionary<string, DataObject>
                {
                    {
                        "Ip", new DataObject(DataObject.VisibilityOptions.Member, value: "")
                    },
                    {
                        "Port", new DataObject(DataObject.VisibilityOptions.Member, value: "")
                    }
                };
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, resetIpPortOptions);
            }

            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>
                {
                    {
                        "Ip", new DataObject(DataObject.VisibilityOptions.Member, value: Ip)
                    },
                    {
                        "Port", new DataObject(DataObject.VisibilityOptions.Member, value: Port)
                    }
                };

            currentLobbyTemp = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
			//listRoomManager.SetCurrentHostLobby(listRoomManager.GetLobbyHost(), currentLobbyTemp);
        }
        catch (Exception ex)
        {
			await Task.Delay(1000);
			await UpdateIPPortToLobby(Ip, Port);
            Debug.Log(ex.Message);
        }
    }
	// =========================================================

	private bool isCancel = false;
	public void CancelTicket()
	{
		isCancel = true;
	}

    private async void OnStartGame()
	{
		gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
        isCancel = false;

        try
		{
            Debug.Log("THE GAME IS STARTED");
            string ticketId = await CreateTicket();
            // CHECK CANCEL
            if (ticketId == "CancelTicket")
			{
                Debug.Log("Cancel Ticket");
                listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
				listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                return;
			}

            await PollTicketStatus(ticketId);
        }
		catch
		{
            Debug.Log("THE GAME IS STARTED AGAIN");
			OnStartGame();
        }
	}

	private async Task<string> CreateTicket()
	{
		List<Unity.Services.Matchmaker.Models.Player> players = new List<Unity.Services.Matchmaker.Models.Player>();
		Lobby lobby = await LobbyService.Instance.GetLobbyAsync(listRoomManager.GetLobbyCurrent().Id);

        // HIDE LOBBY
        listRoomManager.HideLobby(lobby);

		// CKECK IN LOBBY
		foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
		{
			if (player.Data["ReadyState"].Value == "UnReady")
			{
				return "CancelTicket";
			}
		}

		foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
		{
			Unity.Services.Matchmaker.Models.Player p = new Unity.Services.Matchmaker.Models.Player(player.Id, new Dictionary<string, string>()
			{
				{
					"Map", lobby.Data["Map"].Value
				}
			});
			players.Add(p);
		}
		Debug.Log("Player: " + players.Count);
		// Set options for matchmaking
		CreateTicketOptions options = new CreateTicketOptions(
		  "MeowBattleQueue", // The name of the queue defined in the previous step,
		  new Dictionary<string, object>()
		{
			{
				"RoomSize",  lobby.MaxPlayers
			}
		});

		// Create ticket
		CreateTicketResponse ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
		
		// Print the created ticket id
		Debug.Log(ticketResponse.Id);

        // CHECK CANCEL
        if (isCancel)
		{
			isCancel = false;
            return "CancelTicket";
        }

		return ticketResponse.Id;
	}

	private async Task PollTicketStatus(string ticketID)
	{
		MultiplayAssignment assignment = null;
		bool gotAssignment = false;
		do
		{
			//Rate limit delay
			await Task.Delay(TimeSpan.FromSeconds(1f));

			// Poll ticket
			TicketStatusResponse ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketID);
			if (ticketStatus == null)
			{
				continue;
			}

			//Convert to platform assignment data (IOneOf conversion)
			if (ticketStatus.Type == typeof(MultiplayAssignment))
			{
				assignment = ticketStatus.Value as MultiplayAssignment;
			}

            // CHECK CANCEL
            if (isCancel)
            {
                isCancel = false;
                Debug.Log("Cancel Ticket");
                listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                break;
            }

            switch (assignment?.Status)
			{
				case MultiplayAssignment.StatusOptions.Found:
					Debug.Log("StatusOptions.Found");
					await UpdateIPPortToLobby(assignment);
					gotAssignment = true;
					break;
				case MultiplayAssignment.StatusOptions.InProgress:
					Debug.Log("StatusOptions.InProgress");
                    // CHECK CANCEL
                    if (isCancel)
					{
						isCancel = false;
						gotAssignment = true;
                        Debug.Log("Cancel Ticket");
						listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                        gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                        listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                    }
					break;
				case MultiplayAssignment.StatusOptions.Failed:
					gotAssignment = true;
					Debug.LogError("Failed to get ticket status. Error: " + assignment.Message);
                    listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                    gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                    listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                    break;
				case MultiplayAssignment.StatusOptions.Timeout:
					gotAssignment = true;
					Debug.LogError("Failed to get ticket status. Ticket timed out.");
                    listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                    gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                    listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                    break;
				default:
					throw new InvalidOperationException();
			}

		} while (!gotAssignment);
	}

	private async Task UpdateIPPortToLobby(MultiplayAssignment multiplayAssignment)
	{
		try
		{
			Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId: listRoomManager.GetLobbyCurrent().Id);

            // CKECK IN LOBBY
            foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
            {
                if (player.Data["ReadyState"].Value == "UnReady")
                {
                    isCancel = false;
                    Debug.Log("Cancel Ticket");
                    listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                    gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                    listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                    return;
                }
            }

            while (lobby.Data["Ip"].Value == multiplayAssignment.Ip || lobby.Data["Port"].Value == multiplayAssignment.Port.ToString())
            {
                await Task.Delay(500);
                UpdateLobbyOptions resetIpPortOptions = new UpdateLobbyOptions();
                resetIpPortOptions.Data = new Dictionary<string, DataObject>
                {
                    {
                        "Ip", new DataObject(DataObject.VisibilityOptions.Member, value: "")
                    },
                    {
                        "Port", new DataObject(DataObject.VisibilityOptions.Member, value: "")
                    }
                };
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, resetIpPortOptions);
            }

            UpdateLobbyOptions options = new UpdateLobbyOptions();
			options.Data = new Dictionary<string, DataObject>
				{
					{
						"Ip", new DataObject(DataObject.VisibilityOptions.Member, value: multiplayAssignment.Ip)
					},
					{
						"Port", new DataObject(DataObject.VisibilityOptions.Member, value: multiplayAssignment.Port.ToString())
					}
				};

			// CHECK CANCEL
            if (isCancel)
            {
                isCancel = false;
                Debug.Log("Cancel Ticket");
                listRoomManager.ShowLobby(listRoomManager.GetLobbyCurrent());
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                listRoomManager.OnCheckStateButtonInRoom?.Invoke();
                return;
            }

            currentLobbyTemp = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
			Debug.Log("Give Ticket Success, Can Not Cancel");
		}
		catch (Exception ex)
		{
            await Task.Delay(1000);
            await UpdateIPPortToLobby(multiplayAssignment);
            Debug.LogError(ex.Message);
		}
	}

    // GET - SET
    public Lobby CurrentLobbyTemp
	{
		get
		{
			return currentLobbyTemp;
		}
		set
		{
			currentLobbyTemp = value;
		}
	}
}
