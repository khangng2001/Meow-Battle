using Newtonsoft.Json;
using Process;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SingletonAPI : MonoBehaviour
{
    public static SingletonAPI Instance { get; private set; }

    public Action<string> OnMessageAPI;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }


	//Post
    public async Task<bool> PostAddPlayer(PlayerDetails player)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        string playerdata = JsonUtility.ToJson(player);
        Debug.Log(playerdata);
        UnityWebRequest webRequest = UnityWebRequest.Post("https://bomber-backend.onrender.com/users/add", playerdata, "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
                taskCompletionSource.SetResult(false);
            }
            else
            {
                Debug.Log("Form upload complete!");
                taskCompletionSource.SetResult(true);
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

	//Put
    public async Task<bool> PutUpdateName(PlayerDetails player, string name)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        player.name = name;
        string playerdata = JsonUtility.ToJson(player);
        UnityWebRequest webRequest = UnityWebRequest.Put("https://bomber-backend.onrender.com/users/name", playerdata);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError(webRequest.error);
                taskCompletionSource.SetResult(false);
            }
            else
            {
                Debug.Log("Form upload complete!");
                taskCompletionSource.SetResult(true);
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    public async Task<bool> PutUpdateCoin(PlayerDetails player, int price)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        //player.name = name;
        string playerdata = JsonUtility.ToJson(player);
        UnityWebRequest webRequest = UnityWebRequest.Put($"https://bomber-backend.onrender.com/users/coin/{price}", playerdata);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError(webRequest.error);
                taskCompletionSource.SetResult(false);
            }
            else
            {
                Debug.Log("Form upload complete!");
                taskCompletionSource.SetResult(true);
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    public async Task<bool> PutUpdateSkin(PlayerDetails player, string skinID)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        //player.name = name;
        string playerdata = JsonUtility.ToJson(player);
        UnityWebRequest webRequest = UnityWebRequest.Put($"https://bomber-backend.onrender.com/users/skin/{skinID}", playerdata);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError(webRequest.error);
                taskCompletionSource.SetResult(false);
            }
            else
            {
                Debug.Log("Form upload complete!");
                taskCompletionSource.SetResult(true);
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    public async Task<bool> PutUpdateBuySkin(PlayerDetails player, int price, string skinID)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        //player.name = name;
        string playerdata = JsonUtility.ToJson(player);
        UnityWebRequest webRequest = UnityWebRequest.Put($"https://bomber-backend.onrender.com/users/buySkin/{price}/{skinID}", playerdata);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError(webRequest.error);
                taskCompletionSource.SetResult(false);
            }
            else
            {
                Debug.Log("Form upload complete!");
                taskCompletionSource.SetResult(true);
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    public async Task<bool> PutUpdateBuyEps(PlayerDetails player, int price, string epsID)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        //player.name = name;
        string playerdata = JsonUtility.ToJson(player);
        UnityWebRequest webRequest = UnityWebRequest.Put($"https://bomber-backend.onrender.com/users/buyEps/{price}/{epsID}", playerdata);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogError(webRequest.error);
                taskCompletionSource.SetResult(false);
            }
            else
            {
                Debug.Log("Form upload complete!");
                taskCompletionSource.SetResult(true);
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    //Get
    public async Task<PlayerDetails> GetOnePlayer(string unityID)
    {
        TaskCompletionSource<PlayerDetails> taskCompletionSource = new TaskCompletionSource<PlayerDetails>();

        UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/get/{unityID}");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:

                case UnityWebRequest.Result.ProtocolError:

                case UnityWebRequest.Result.DataProcessingError:
                    {
                        Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
                        break;
                    }
                case UnityWebRequest.Result.Success:
                    {
                        Debug.Log(webRequest.downloadHandler.text);
                        PlayerDetails player = JsonUtility.FromJson<PlayerDetails>(webRequest.downloadHandler.text);
                        taskCompletionSource.SetResult(player);
                        break;
                    }
                default:
                    {
                        //Debug.LogError("Default Error: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
                        break;
                    }
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    public async Task<string[]> GetUserSkin(string unityID)
    {
		TaskCompletionSource<string[]> taskCompletionSource = new TaskCompletionSource<string[]>();

		UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/getUserSkin/{unityID}");

		var req = webRequest.SendWebRequest();

		req.completed += (op) =>
		{
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:

				case UnityWebRequest.Result.ProtocolError:

				case UnityWebRequest.Result.DataProcessingError:
					{
						Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
				case UnityWebRequest.Result.Success:
					{
						Debug.Log(webRequest.downloadHandler.text);
                        //string[] listSkin = JsonUtility.FromJson<string[]>(webRequest.downloadHandler.text);
                        string[] listSkin = JsonConvert.DeserializeObject<string[]>(webRequest.downloadHandler.text);
						taskCompletionSource.SetResult(listSkin);
						break;
					}
				default:
					{
						//Debug.LogError("Default Error: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
			}

			// Clean up the UnityWebRequest
			webRequest.Dispose();
		};

		return await taskCompletionSource.Task;
	}

    public async Task<List<SkinDetail>> GetAllSkin()
    {
		TaskCompletionSource<List<SkinDetail>> taskCompletionSource = new TaskCompletionSource<List<SkinDetail>>();

		UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/get-all-skins");

		var req = webRequest.SendWebRequest();

		req.completed += (op) =>
		{
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:

				case UnityWebRequest.Result.ProtocolError:

				case UnityWebRequest.Result.DataProcessingError:
					{
						Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
				case UnityWebRequest.Result.Success:
					{
						Debug.Log(webRequest.downloadHandler.text);
						//List<SkinDetail> allSkin = JsonUtility.FromJson<List<SkinDetail>>(webRequest.downloadHandler.text);
                        List<SkinDetail> allskin = JsonConvert.DeserializeObject<List<SkinDetail>>(webRequest.downloadHandler.text);
						taskCompletionSource.SetResult(allskin);
						break;
					}
				default:
					{
                        Debug.LogWarning("Default Error: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
						break;
					}
			}

			// Clean up the UnityWebRequest
			webRequest.Dispose();
		};

		return await taskCompletionSource.Task;
	}

    public async Task<SkinDetail> GetSkin(string skinID)
    {
        TaskCompletionSource<SkinDetail> taskCompletionSource = new TaskCompletionSource<SkinDetail>();

        UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/get-skin/{skinID}");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:

                case UnityWebRequest.Result.ProtocolError:

                case UnityWebRequest.Result.DataProcessingError:
                    {
                        Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
                        break;
                    }
                case UnityWebRequest.Result.Success:
                    {
                        Debug.Log(webRequest.downloadHandler.text);
                        //List<SkinDetail> allSkin = JsonUtility.FromJson<List<SkinDetail>>(webRequest.downloadHandler.text);
                        SkinDetail allskin = JsonConvert.DeserializeObject<SkinDetail>(webRequest.downloadHandler.text);
                        taskCompletionSource.SetResult(allskin);
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("Default Error: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
                        break;
                    }
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }

    public async Task<string[]> GetUserExpression(string unityID)
	{
		TaskCompletionSource<string[]> taskCompletionSource = new TaskCompletionSource<string[]>();

		UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/getUserEps/{unityID}");

		var req = webRequest.SendWebRequest();

		req.completed += (op) =>
		{
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:

				case UnityWebRequest.Result.ProtocolError:

				case UnityWebRequest.Result.DataProcessingError:
					{
						Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
				case UnityWebRequest.Result.Success:
					{
						Debug.Log(webRequest.downloadHandler.text);
						//string[] listSkin = JsonUtility.FromJson<string[]>(webRequest.downloadHandler.text);
						string[] listEps = JsonConvert.DeserializeObject<string[]>(webRequest.downloadHandler.text);
						taskCompletionSource.SetResult(listEps);
						break;
					}
				default:
					{
						//Debug.LogError("Default Error: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
			}

			// Clean up the UnityWebRequest
			webRequest.Dispose();
		};

		return await taskCompletionSource.Task;
	}

	public async Task<List<EpsDetail>> GetAllExpression()
	{
		TaskCompletionSource<List<EpsDetail>> taskCompletionSource = new TaskCompletionSource<List<EpsDetail>>();

		UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/get-all-eps");

		var req = webRequest.SendWebRequest();

		req.completed += (op) =>
		{
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:

				case UnityWebRequest.Result.ProtocolError:

				case UnityWebRequest.Result.DataProcessingError:
					{
						Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
				case UnityWebRequest.Result.Success:
					{
						Debug.Log(webRequest.downloadHandler.text);
						//List<SkinDetail> allSkin = JsonUtility.FromJson<List<SkinDetail>>(webRequest.downloadHandler.text);
						List<EpsDetail> allskin = JsonConvert.DeserializeObject<List<EpsDetail>>(webRequest.downloadHandler.text);
						taskCompletionSource.SetResult(allskin);
						break;
					}
				default:
					{
						Debug.LogWarning("Default Error: " + webRequest.error);
						taskCompletionSource.SetResult(null);
						break;
					}
			}

			// Clean up the UnityWebRequest
			webRequest.Dispose();
		};

		return await taskCompletionSource.Task;
	}

    public async Task<EpsDetail> GetEps(string epsID)
    {
        TaskCompletionSource<EpsDetail> taskCompletionSource = new TaskCompletionSource<EpsDetail>();

        UnityWebRequest webRequest = UnityWebRequest.Get($"https://bomber-backend.onrender.com/users/get-eps/{epsID}");

        var req = webRequest.SendWebRequest();

        req.completed += (op) =>
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:

                case UnityWebRequest.Result.ProtocolError:

                case UnityWebRequest.Result.DataProcessingError:
                    {
                        Debug.LogWarning("There is a problem connecting server: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
                        break;
                    }
                case UnityWebRequest.Result.Success:
                    {
                        Debug.Log(webRequest.downloadHandler.text);
                        //List<SkinDetail> allSkin = JsonUtility.FromJson<List<SkinDetail>>(webRequest.downloadHandler.text);
                        EpsDetail allskin = JsonConvert.DeserializeObject<EpsDetail>(webRequest.downloadHandler.text);
                        taskCompletionSource.SetResult(allskin);
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("Default Error: " + webRequest.error);
                        taskCompletionSource.SetResult(null);
                        break;
                    }
            }

            // Clean up the UnityWebRequest
            webRequest.Dispose();
        };

        return await taskCompletionSource.Task;
    }
}
