using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerSkin : NetworkBehaviour
{
    // With PlayerID And Data In Lobby, Load Skin If I'm Other Player
    public void LoadSkinForOtherPlayer(string nameSKin, string nameExpression)
    {
        if (!NetworkManager.Singleton.IsClient) return;
        if (IsOwner) return;

        PlayerDataClient playerData = GetComponent<PlayerDataClient>();
        Lobby lobby = playerData.GetLobbyCurrent();

        // Get Skins
        List<Material> skins = new List<Material>();
        foreach (SkinSO item in playerData.GetInfoSkinList())
        {
            skins.Add(item.GetSkinMaterial());
        }
        // Get Expression
        List<Material> expressions = new List<Material>();
        foreach (SkinSO item in playerData.GetInfoExpression())
        {
            expressions.Add(item.GetSkinMaterial());
        }

        Material skinFinded = skins.Find((Material material) => { if (material.name == nameSKin) return true; return false; });
        Material expressionFinded = expressions.Find((Material material) => { if (material.name == nameExpression) return true; return false; });
        
        GetComponentInChildren<SkinnedMeshRenderer>().SetMaterials(new List<Material>()
        {
            skinFinded,
            expressionFinded
        });
    }
}
