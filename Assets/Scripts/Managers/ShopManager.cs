using Managers;
using Process;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private enum ShopState
    {
        Shop,
        Outfit
    }

    public enum SkinListType
    {
        Skin,
        Expression
    }

    private ShopState currentShopState = ShopState.Shop;

    //[SerializeField] private SkinListType skinListType;
    [SerializeField] private SkinSlotUI skinSlotPrefab;
    [SerializeField] private GameObject buyBtn;
    [SerializeField] private GameObject priceText;

    [Header("Shop")]
    [SerializeField] private Transform contentTranformSkinShop;
    [SerializeField] private Transform contentTranformEpsShop;

    [Header("Outfit")]
    [SerializeField] private Transform contentTranformSkinOutfit;
    [SerializeField] private Transform contentTranformEpsOutfit;

    [Header("Toggle")]
    [SerializeField] private ToggleGroup shopToggleGroup;
    [SerializeField] private GameObject AvatarPage;
    [SerializeField] private GameObject EmojiPage;

    [Header("Toggle1")]
    [SerializeField] private ToggleGroup shopToggleGroup1;
    [SerializeField] private GameObject AvatarPage1;
    [SerializeField] private GameObject EmojiPage1;

    [Header("Ribbon")]
    [SerializeField] private Button shopRoomBtn;
    [SerializeField] private GameObject shopRoomSelected;
    [SerializeField] private Button outfitRoomBtn;
    [SerializeField] private GameObject outfitRoomSelected;
    [SerializeField] private GameObject shopRoom;
    [SerializeField] private GameObject outfitRoom;
    [SerializeField] private Button outShopBtn;
    [SerializeField] private TextMeshProUGUI cointxt;

    [SerializeField] private Material skinPlayerInShop;
    [SerializeField] private Material expressionPlayerInShop;

    [SerializeField] private Button shopBtn;

    private GameLauncher gameLauncher;
    private HandlerSkinListSO handlerSkinListSO;
        
	public Action<DataType, SkinSlotUI, Transform, SkinListSO> OnFetchPlayerData;
    public Action<string> OnMessageHandlerSkinListSO;
    public Action<PlayerDetails> OnPlayerBuySKinAction;

    private void Start()
    {
        UpdateUIShop(ShopState.Shop);
        //shopToggleGroup = GetComponentInParent<ToggleGroup>();
        handlerSkinListSO = GetComponent<HandlerSkinListSO>();
        gameLauncher = GetComponentInParent<GameLauncher>();
        shopBtn.onClick.AddListener(() => { OnClickShopBtn(); EmitSoundClick.Emit(); });

        shopRoomBtn.onClick.AddListener(() => { OnClickShopRoomBtn();  EmitSoundClick.Emit(); });
        outfitRoomBtn.onClick.AddListener(() => { OnClickOutRoomBtn(); EmitSoundClick.Emit(); });
        outShopBtn.onClick.AddListener(() => { OnClickOutShop(); EmitSoundClick.Emit(); });
        //AddSkin();

        SubscribeShopToggleGroup(shopToggleGroup, AvatarPage, EmojiPage);
        SubscribeShopToggleGroup(shopToggleGroup1, AvatarPage1, EmojiPage1);
    }

    private void OnClickOutShop()
    {
        gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame);
    }

    private async void OnClickShopBtn()
    {
        gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
        gameLauncher.UpdatePageState(GameLauncher.PageState.Shop);

        var player = await SingletonAPI.Instance.GetOnePlayer(gameLauncher.GetPlayerDetail().unityID);
        gameLauncher.SetPlayerDetail(player);
        cointxt.text = gameLauncher.GetPlayerDetail().coin.ToString();

        OnFetchPlayerData?.Invoke(DataType.Skin, skinSlotPrefab, contentTranformSkinShop, handlerSkinListSO.SkinListNotOwn);
        OnFetchPlayerData?.Invoke(DataType.Expression, skinSlotPrefab, contentTranformEpsShop, handlerSkinListSO.SkinListNotOwn);
        OnFetchPlayerData?.Invoke(DataType.Skin, skinSlotPrefab, contentTranformSkinOutfit, handlerSkinListSO.SkinListOwn);
        OnFetchPlayerData?.Invoke(DataType.Expression, skinSlotPrefab, contentTranformEpsOutfit, handlerSkinListSO.SkinListOwn);
    }

    public async void OnClickBuyButton(SkinSlotUI obj)
    {
        try
        {
            switch (obj.GetSkinType())
            {
                case SkinType.Skin:
                    {
                        Debug.Log(obj.GetSkinID());
                        SkinDetail skin = await SingletonAPI.Instance.GetSkin(obj.GetSkinID());
                        if (skin == null)
                        {
                            throw new Exception("Retrieve information failed !!!");
                        }

                        if (gameLauncher.GetPlayerDetail() == null)
                        {
                            throw new Exception("An error occurred during the purchase process about infomation's Player");
                        }

                        if (gameLauncher.GetPlayerDetail().coin < skin.skinPrice)
                        {
                            throw new Exception("You DON'T ENOUGH coin to buy this Skin !!");
                        }

                        bool check = await SingletonAPI.Instance.PutUpdateBuySkin(gameLauncher.GetPlayerDetail(), skin.skinPrice, skin.skinID);
                        if (!check)
                        {
                            throw new Exception("The Purchase process have a problem !!!");
                        }

                        await handlerSkinListSO.ActionBuySkin();
                        OnFetchPlayerData?.Invoke(DataType.Skin, skinSlotPrefab, contentTranformSkinShop, handlerSkinListSO.SkinListNotOwn);
                        OnFetchPlayerData?.Invoke(DataType.Skin, skinSlotPrefab, contentTranformSkinOutfit, handlerSkinListSO.SkinListOwn);

                        var player = await SingletonAPI.Instance.GetOnePlayer(gameLauncher.GetPlayerDetail().unityID);
                        gameLauncher.SetPlayerDetail(player);
                        cointxt.text = gameLauncher.GetPlayerDetail().coin.ToString();
                        break;
                    }
                case SkinType.Expression:
                    {
                        EpsDetail eps = await SingletonAPI.Instance.GetEps(obj.GetSkinID());
                        if (eps == null)
                        {
                            throw new Exception("Retrieve information failed !!!");
                        }

                        if (gameLauncher.GetPlayerDetail() == null)
                        {
                            throw new Exception("An error occurred during the purchase process about infomation's Player");
                        }

                        if (gameLauncher.GetPlayerDetail().coin < eps.epsPrice)
                        {
                            throw new Exception("You DON'T ENOUGH coin to buy this Skin !!");
                        }

                        bool check = await SingletonAPI.Instance.PutUpdateBuyEps(gameLauncher.GetPlayerDetail(), eps.epsPrice, eps.epsID);
                        if (!check)
                        {
                            throw new Exception("The Purchase process have a problem !!!");
                        }

                        await handlerSkinListSO.ActionBuySkin();
                        OnFetchPlayerData?.Invoke(DataType.Expression, skinSlotPrefab, contentTranformEpsShop, handlerSkinListSO.SkinListNotOwn);
                        OnFetchPlayerData?.Invoke(DataType.Expression, skinSlotPrefab, contentTranformEpsOutfit, handlerSkinListSO.SkinListOwn);

                        var player = await SingletonAPI.Instance.GetOnePlayer(gameLauncher.GetPlayerDetail().unityID);
                        gameLauncher.SetPlayerDetail(player);
                        cointxt.text = gameLauncher.GetPlayerDetail().coin.ToString();
                        break;
                    }
            }


            //6000 - skin.skinPrice
            //bool check = await SingletonAPI.Instance.PutUpdateCoin(gameLauncher.GetPlayerDetail() , -skin.skinPrice);
            //Debug.Log(skin.skinPrice);
            //Debug.Log("Price" + obj.GetPrice());
            //if (!check)
            //{
            //    throw new Exception("The Purchase process have a problem !!!");
            //}

            //bool checkPut = await SingletonAPI.Instance.PutUpdateSkin(gameLauncher.GetPlayerDetail(), skin.skinID);
            //if (!check)
            //{
            //    throw new Exception("The Purchase process have a problem !!!");
            //}



            //Load lại list skin and expression trên data 
            //OnPlayerBuySKinAction?.Invoke(gameLauncher.GetPlayerDetail());
            //OnPlayerDetailsSignIn?.Invoke(player);
            //OnFetchPlayerData?.Invoke(DataType.Skin, skinSlotPrefab, contentTranform, skinListSO);
        }
        catch (Exception ex)
        {
            OnMessageHandlerSkinListSO?.Invoke(ex.Message);
        }
    }

    private void SubscribeShopToggleGroup(ToggleGroup toggleGroup, GameObject avatar, GameObject emoji)
    {
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener((ison) =>
            {
                if (ison)
                {
                    Debug.Log(toggle.name);

                    if (toggle.name == "Avatar")
                    {
                        //Debug.Log(toggle.name);
                        avatar.SetActive(true);
                        emoji.SetActive(false);
                    }
                    else
                    {
                        avatar.SetActive(false);
                        emoji.SetActive(true);
                    }
                }
            });
        }
    }

    private void UpdateUIShop(ShopState newState)
    {
        currentShopState = newState;

        shopRoom.gameObject.SetActive(false);
        outfitRoom.gameObject.SetActive(false);
        shopRoomSelected.SetActive(false);
        outfitRoomSelected.SetActive(false);
        buyBtn.SetActive(false);
        priceText.SetActive(false);

        switch (currentShopState)
        {
            case ShopState.Shop:
                {
                    shopRoom.gameObject.SetActive(true);
                    shopRoomSelected.SetActive(true);
                    buyBtn.SetActive(true);
                    priceText.SetActive(true);
                    break;
                }
            case ShopState.Outfit:
                {
                    outfitRoom.gameObject.SetActive(true);
                    outfitRoomSelected.SetActive(true);
                    break;
                }
        }
    }

    private void OnClickShopRoomBtn()
    {
        UpdateUIShop(ShopState.Shop);
        shopRoomSelected.SetActive(true);
    }

    private void OnClickOutRoomBtn()
    {
        UpdateUIShop(ShopState.Outfit);
        outfitRoomSelected.SetActive(true);
    }

    public void ChangedSkinPlayerInShop(SkinSlotUI obj)
    {
        switch (obj.GetSkinType())
        {
            case SkinType.Skin:
                skinPlayerInShop.CopyPropertiesFromMaterial(obj.GetMaterial());
                break;
            case SkinType.Expression:
                expressionPlayerInShop.CopyPropertiesFromMaterial(obj.GetMaterial());
                break;
        }
    }
}
