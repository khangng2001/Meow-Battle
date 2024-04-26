using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin : MonoBehaviour
{
    //private GameLauncher gameLauncher;

    //private void Start()
    //{
    //    gameLauncher = GetComponentInParent<GameLauncher>();
    //}

    //private async void OnClickBuyButton(SkinSlotUI obj)
    //{
    //    try
    //    {
    //        SkinDetail skin = await SingletonAPI.Instance.GetSkin(obj.GetSkinID());
    //        if (skin == null)
    //        {
    //            throw new Exception("Retrieve information failed !!!");
    //        }

    //        //6000 - skin.skinPrice
    //        //bool check = await SingletonAPI.Instance.PutUpdateCoin(gameLauncher.GetPlayerDetail() , -skin.skinPrice);
    //        //Debug.Log(skin.skinPrice);
    //        //Debug.Log("Price" + obj.GetPrice());
    //        //if (!check)
    //        //{
    //        //    throw new Exception("The Purchase process have a problem !!!");
    //        //}

    //        //bool checkPut = await SingletonAPI.Instance.PutUpdateSkin(gameLauncher.GetPlayerDetail(), skin.skinID);
    //        //if (!check)
    //        //{
    //        //    throw new Exception("The Purchase process have a problem !!!");
    //        //}

    //        bool check = await SingletonAPI.Instance.PutUpdateBuySkin(gameLauncher.GetPlayerDetail(), skin.skinPrice, skin.skinID);
    //        if (!check)
    //        {
    //            throw new Exception("The Purchase process have a problem !!!");
    //        }

    //        //Load lại list skin and expression trên data 
    //        //OnPlayerDetailsSignIn?.Invoke(player);
    //        //OnPlayerDetailsSignIn?.Invoke(player);
    //        OnFetchPlayerData?.Invoke(DataType.Skin, skinSlotPrefab, contentTranform, skinListSO);
    //    }
    //    catch (Exception ex)
    //    {
    //        OnMessageHandlerSkinListSO?.Invoke(ex.Message);
    //    }
    //}
}
