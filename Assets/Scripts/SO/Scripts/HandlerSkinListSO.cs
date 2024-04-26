using Process;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HandlerSkinListSO : MonoBehaviour
{
    [SerializeField] private SkinListSO skinListFullSO;
    [SerializeField] private SkinListSO skinListOwn;
    [SerializeField] private SkinListSO skinListNotOwn;

    [SerializeField] private Material skinCurrent;
    [SerializeField] private Material expressionCurrent;
    //[SerializeField] private Renderer playerMaterial;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyBtn;

    //private List<SkinSlotUI> skinList = new List<SkinSlotUI>();

    private ShopManager shopManager;

    private SignIn signIn;

    private InRoomManager inRoomManager;

    public Action<string> OnMessageHandlerSkinListSO;

    private void Awake()
    {
        signIn = transform.parent.transform.parent.GetComponentInChildren<SignIn>();
        inRoomManager = transform.parent.GetComponentInChildren<InRoomManager>();
        shopManager = GetComponent<ShopManager>();

        SubscribeEvents();
    }

    private void OnDestroy()
    {
        //signIn.OnPlayerDetailsSignIn -= SetUserSkinSOAction;
        //signIn.OnPlayerDetailsSignIn -= SetUserExpressionSOAction;

        shopManager.OnFetchPlayerData -= AddSkin;
        inRoomManager.OnFetchPlayerData -= AddSkin;
        signIn.OnPlayerDetailsSignIn -= SetAllSkinExpressionSOAction;

        //shopManager.OnPlayerBuySKinAction -= SetUserExpressionSOAction;
        //shopManager.OnPlayerBuySKinAction -= SetUserSkinSOAction;
    }

    private void Start()
    {
        skinListOwn.skinSOList.Clear();
        skinListNotOwn.skinSOList.Clear();
        skinListOwn.expressionSOList.Clear();
        skinListNotOwn.expressionSOList.Clear();
        //await SetAllSkinSO();
        //await SetAllExpressionSO();
        //Debug.Log(await SingletonAPI.Instance.GetAllSkin());
    }

    private void SubscribeEvents()
    {
        //signIn.OnPlayerDetailsSignIn += SetUserSkinSOAction;
        //      signIn.OnPlayerDetailsSignIn += SetUserExpressionSOAction;
        //shopManager.OnPlayerBuySKinAction += SetUserExpressionSOAction;
        //shopManager.OnPlayerBuySKinAction += SetUserSkinSOAction;
        shopManager.OnFetchPlayerData += AddSkin;
        inRoomManager.OnFetchPlayerData += AddSkin;
        signIn.OnPlayerDetailsSignIn += SetAllSkinExpressionSOAction;
    }

    private async void SetAllSkinExpressionSOAction(PlayerDetails player)
    {
        try
        {
            if (player == null)
            {
                throw new Exception("An Error during the Login: PlayerDeltail Null");
            }

            List<SkinDetail> listSkinAll = new List<SkinDetail>();
            listSkinAll = await SingletonAPI.Instance.GetAllSkin();

            List<EpsDetail> listEpsAll = new List<EpsDetail>();
            listEpsAll = await SingletonAPI.Instance.GetAllExpression();

            if (listSkinAll == null)
            {
                throw new Exception("NO INTERNET YEAHHHH AHHH 7UPPERCUT");
            }

            if (listEpsAll == null)
            {
                throw new Exception("NO INTERNET YEAHHHH AHHH 7UPPERCUT");
            }

            MessageBox.OnCloseBtn = null;

            SetAllSkinSO(listSkinAll);
            SetAllExpressionSO(listEpsAll);

            try
            {
                if (AuthenticationService.Instance.PlayerId == null)
                {
                    throw new Exception("An Error occurred during the fetch player's skin: PlayerID Null");
                }

                skinListOwn.skinSOList.Clear();
                skinListNotOwn.skinSOList.Clear();
                skinListOwn.expressionSOList.Clear();
                skinListNotOwn.expressionSOList.Clear();

                await SetUserSkinSO(); // Chạy lại API để add Skin vô SO
                await SetUserExpressionSO(); // Chạy lại API để add Eps vô SO
            }
            catch (Exception ex)
            {
                OnMessageHandlerSkinListSO?.Invoke(ex.Message);
            }


        }
        catch (Exception ex)
        {
            MessageBox.OnCloseBtn = () => { SetAllSkinExpressionSOAction(player); };
            OnMessageHandlerSkinListSO?.Invoke(ex.Message);
        }

    }

    //Expression 
    private void SetAllExpressionSO(List<EpsDetail> listEpsAll)
    {
        //Debug.Log(list.Count);
        foreach (SkinSO skinSO in skinListFullSO.expressionSOList)
        {
            foreach (EpsDetail eps in listEpsAll)
            {
                //Debug.Log(eps.epsID);
                if (skinSO.GetSkinID() == eps.epsID)
                {
                    //Debug.Log(skin.skinName);
                    skinSO.SetSkinName(eps.epsName);
                    skinSO.SetSkinPrice(eps.epsPrice);
                    // skinListNotOwn.skinSOList.Add(skinSO);
                }
            }
        }
    }

    //private async void SetUserExpressionSOAction(PlayerDetails player)
    //{
    //    if (player != null)
    //    {
    //        await SetUserExpressionSO();
    //        // shopManager.AddSkin();
    //    }
    //} // Khong su dung

    private async Task SetUserExpressionSO()
    {
        string[] list = new string[] { };
        list = await SingletonAPI.Instance.GetUserExpression(AuthenticationService.Instance.PlayerId);

        foreach (SkinSO skinSO in skinListFullSO.expressionSOList)
        {
            bool check = false;
            foreach (string skinID in list)
            {
                if (skinSO.GetSkinID() == skinID)
                {
                    skinListOwn.expressionSOList.Add(skinSO);
                    check = true;
                    break;
                }
            }
            if (!check) skinListNotOwn.expressionSOList.Add(skinSO);
        }
    }

    //Skin
    private void SetAllSkinSO(List<SkinDetail> listSkinAll)
    {
        //Debug.Log(list.Count);
        foreach (SkinSO skinSO in skinListFullSO.skinSOList)
        {
            foreach (SkinDetail skin in listSkinAll)
            {
                if (skinSO.GetSkinID() == skin.skinID)
                {
                    //Debug.Log(skin.skinName);
                    skinSO.SetSkinName(skin.skinName);
                    skinSO.SetSkinPrice(skin.skinPrice);
                    // skinListNotOwn.skinSOList.Add(skinSO);
                }
            }
        }
    }

    //private async void SetUserSkinSOAction(PlayerDetails player)
    //{
    //	if (player != null)
    //	{
    //		await SetUserSkinSO();
    //           await SetUserExpressionSO();
    //		//AddSkin(DataType.Skin, skinSlotPrefab, contentTranform, skinListSO);
    //           // shopManager.AddSkin();
    //       }
    //   } // Khong su dung

    private async Task SetUserSkinSO()
    {
        string[] list = new string[] { };
        list = await SingletonAPI.Instance.GetUserSkin(AuthenticationService.Instance.PlayerId);

        foreach (SkinSO skinSO in skinListFullSO.skinSOList)
        {
            bool check = false;
            foreach (string skinID in list)
            {
                if (skinSO.GetSkinID() == skinID)
                {
                    skinListOwn.skinSOList.Add(skinSO);
                    check = true;
                    break;
                }
            }
            if (!check) skinListNotOwn.skinSOList.Add(skinSO);
        }
    }


    public async Task ActionBuySkin()
    {
        try
        {
            if (AuthenticationService.Instance.PlayerId == null)
            {
                throw new Exception("An Error occurred during the fetch player's skin: PlayerID Null");
            }

            skinListOwn.skinSOList.Clear();
            skinListNotOwn.skinSOList.Clear();
            skinListOwn.expressionSOList.Clear();
            skinListNotOwn.expressionSOList.Clear();

            await SetUserSkinSO(); // Chạy lại API để add Skin vô SO
            await SetUserExpressionSO(); // Chạy lại API để add Eps vô SO
        }
        catch (Exception ex)
        {
            OnMessageHandlerSkinListSO?.Invoke(ex.Message);
        }
    }

    private void AddSkin(DataType type, SkinSlotUI prefab, Transform content, SkinListSO listSO)
    {
        //if (player != null)
        //{
        //clear Content

        for (var i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        //}
        switch (type)
        {
            case DataType.Skin:


                if (listSO.skinSOList.Count >= 1)
                {
                    foreach (SkinSO skin in listSO.skinSOList) // tat ca trong SO
                    {
                        SkinSlotUI skinSlotUI = Instantiate(prefab, content);

                        skinSlotUI.GetComponent<Transform>().localScale = Vector3.one;
                        skinSlotUI.SetData(skin);
                        skinSlotUI.GetComponentInChildren<TextMeshProUGUI>().text = skin.GetSkinMaterial().name;
                        // skinList.Add(skinSlotUI);
                        skinSlotUI.onSkinSlotClicked += SkinSlotUI_onSkinSlotClicked;
                    }
                }
                break;
            case DataType.Expression:


                if (listSO.expressionSOList.Count >= 1)
                {
                    foreach (SkinSO skin in listSO.expressionSOList) // tat ca trong SO
                    {
                        SkinSlotUI skinSlotUI = Instantiate(prefab, content);

                        skinSlotUI.GetComponent<Transform>().localScale = Vector3.one;
                        skinSlotUI.SetData(skin);
                        skinSlotUI.GetComponentInChildren<TextMeshProUGUI>().text = skin.GetSkinMaterial().name;
                        // skinList.Add(skinSlotUI);
                        skinSlotUI.onSkinSlotClicked += SkinSlotUI_onSkinSlotClicked;
                    }
                }
                break;
        }
    }

    private void SkinSlotUI_onSkinSlotClicked(SkinSlotUI obj)
    {
        //DeSelectedAllSkins();
        //obj.Selected();
        buyBtn.onClick.RemoveAllListeners();

        //Update PriceUI
        priceText.text = obj.GetPrice().ToString() + " $";

        //Update Player
        // playerMaterial.material = obj.GetMaterial();
        Debug.Log(obj.GetMaterial());
        shopManager.ChangedSkinPlayerInShop(obj);

        buyBtn.interactable = true;

        buyBtn.onClick.AddListener(() =>
        {
            buyBtn.interactable = false;
            shopManager.OnClickBuyButton(obj);
            EmitSoundClick.Emit();
        });
    }

    // private void DeSelectedAllSkins()
    // {
    //     foreach (var skin in skinList)
    //     {
    //Debug.Log("DeSelectedAllSkins");
    //         skin.DeSelected();
    //     }
    // }

    // GET - SET
    public SkinListSO SkinListFullSO
    {
        get
        {
            return skinListFullSO;
        }
    }
    public SkinListSO SkinListOwn
    {
        get
        {
            return skinListOwn;
        }
    }
    public SkinListSO SkinListNotOwn
    {
        get
        {
            return skinListNotOwn;
        }
    }

    public Material SkinCurrent
    {
        get
        {
            return skinCurrent;
        }
        set
        {
            skinCurrent = value;
        }
    }
    public Material ExpressionCurrent
    {
        get
        {
            return expressionCurrent;
        }
        set
        {
            expressionCurrent = value;
        }
    }
}
