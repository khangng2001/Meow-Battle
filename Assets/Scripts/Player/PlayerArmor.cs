using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerArmor : NetworkBehaviour
{
    private bool isHaveArmor = false;
    private GameObject armor;

    // COROTINE
    public IEnumerator ReadyDestroyArmor()
    {
        if (armor == null) yield break;

        yield return new WaitForSeconds(0.1f);
        armor.GetComponent<ArmorController>().TurnOff();
        yield return new WaitForSeconds(0.5f);
        armor.GetComponent<ArmorController>().TurnOn();
        yield return new WaitForSeconds(0.5f);
        armor.GetComponent<ArmorController>().TurnOff();
        yield return new WaitForSeconds(0.25f);
        armor.GetComponent<ArmorController>().TurnOn();
        yield return new WaitForSeconds(0.25f);
        armor.GetComponent<ArmorController>().TurnOff();

        isHaveArmor = false;
        Destroy(armor.gameObject);
    }

    // GET - SET
    public bool IsHaveArmor
    {
        get
        {
            return isHaveArmor;
        }
        set
        {
            isHaveArmor = value;

            if (value) GetComponent<PlayerAudio>().ShieldCreatedSoundClientRpc();
        }
    }
    public GameObject Armor
    {
        get
        {
            return armor;
        }
        set
        {
            armor = value;
        }
    }
}
