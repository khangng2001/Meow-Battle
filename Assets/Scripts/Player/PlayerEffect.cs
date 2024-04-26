using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerEffect : NetworkBehaviour
{
    [SerializeField] private GameObject freezeUI;
    [SerializeField] private GameObject boomUI;

    public void TurnOnEFfectUI(EffectUI effectUI)
    {
        switch (effectUI)
        {
            case EffectUI.Freeze:
                TurnOnOffFreezeVolumeClientRpc(true);
                break;
            case EffectUI.Boom:
                TurnOnOffBoomVolumeClientRpc(true);
                break;
        }
    }

    public void TurnOffEFfectUI(EffectUI effectUI)
    {
        switch (effectUI)
        {
            case EffectUI.Freeze:
                TurnOnOffFreezeVolumeClientRpc(false);
                break;
            case EffectUI.Boom:
                TurnOnOffBoomVolumeClientRpc(false);
                break;
        }
    }


    // EVERY EFFECT

    [ClientRpc]
    private void TurnOnOffFreezeVolumeClientRpc(bool onOff)
    {
        if (!IsOwner) return;
        if (freezeUI == null) return;

        if (onOff)
        {
            if (corotineFreezeUI != null) StopCoroutine(corotineFreezeUI);

            freezeUI.GetComponent<Volume>().weight = 1f;
        }
        else
        {
            corotineFreezeUI = TimeDownUI(freezeUI.GetComponent<Volume>());
            StartCoroutine(corotineFreezeUI);
        }
    }

    [ClientRpc]
    private void TurnOnOffBoomVolumeClientRpc(bool onOff)
    {
        if (!IsOwner) return;
        if (boomUI == null) return;

        if (onOff)
        {
            if (corotineBoomUI != null) StopCoroutine(corotineBoomUI);
            
            boomUI.GetComponent<Volume>().weight = 1f;
        }
        else
        {
            corotineBoomUI = TimeDownUI(boomUI.GetComponent<Volume>());
            StartCoroutine(corotineBoomUI);
        }
    }

    // COROTINE

    private IEnumerator corotineBoomUI;
    private IEnumerator corotineFreezeUI;

    IEnumerator TimeDownUI(Volume vol)
    {
        float weight = 1f;
        float time = 0.1f;

        while (weight > 0f)
        {
            yield return new WaitForSeconds(time);
            vol.weight = weight;

            weight -= 0.05f;
        }

        vol.weight = 0f;
    }
}
