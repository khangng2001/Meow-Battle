using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBomb : NetworkBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombLength;
    [SerializeField] private float bombCount;

    [SerializeField] private LayerMask bombLayerMask;

    private float bombQuantity;

    public void CountingBombQuantity(float count)
    {
        bombQuantity = count;
    }

    public void PutBomb(GameObject owner, Vector3 location, float explosionLength, PlayerAudio audio)
    {
        if (bombPrefab == null) return;
        if (bombQuantity <= 0) return;
        if (!CheckHaveBombInLocation(location)) return;

        bombQuantity -= 1;

        GameObject bombGO = Instantiate(bombPrefab, RoundBombPos(location), Quaternion.identity);
        NetworkObject bombNB = bombGO.GetComponent<NetworkObject>();
        bombNB.Spawn();

        BombController bombController = bombGO.GetComponent<BombController>();
        bombController.Owner = owner;
        bombController.ExplosionLength = explosionLength;
        bombController.ExplosionLengthNetWork = (int)explosionLength;

        // AUDIO
        audio.PlaceBombSoundClientRpc();
    }

    private Vector3 RoundBombPos(Vector3 positionOfPlayer)
    {
        Vector3 pos = positionOfPlayer;
        pos = new Vector3(RoundNumber(pos.x), this.gameObject.transform.position.y, RoundNumber(pos.z));
        return pos;
    }

    private float RoundNumber(float a)
    {
        int b = (int)a;

        if (b >= 0)
        {
            if (b % 2 == 0 && b == Mathf.Round(a))       //a chan , a < a.5   :   a.099 - a.499 -> Round = a
            {
                if (a < 0 && Mathf.Round(a) == 0) a = a - 2; // Special Case    -   When -0.49 to 0

                a = Mathf.Round(a) + 1;
                return a;
            }
            if (b % 2 != 0 && b < Mathf.Round(a))       //a le , a >= a.5  :   a.5 - a.999 -> Round = a+1
            {
                a = Mathf.Round(a) - 1;
                return a;
            }
        }
        else if (b < 0)
        {
            if (b % 2 != 0 && b > Mathf.Round(a))       //-a le , -a < -a.5   :   -a.999  -a.5 -> Round = -a-1
            {
                a = Mathf.Round(a) + 1;
                return a;
            }
            if (b % 2 == 0 && b == Mathf.Round(a))      //-a chan , -a > -a.5   :   -a.499  -a.099 -> Round = -a
            {
                a = Mathf.Round(a) - 1;
                return a;
            }
        }
        return Mathf.Round(a);
    }

    private bool CheckHaveBombInLocation(Vector3 location)
    {
        Vector3 locationPutBomb = RoundBombPos(location);
        Vector3 directionFromPlayerToLocationPutBomb = locationPutBomb - location;
        //Debug.DrawRay(location, directionFromPlayerToLocationPutBomb + Vector3.up, Color.yellow, 2f);
        Ray ray = new Ray(location, directionFromPlayerToLocationPutBomb + Vector3.up);
        bool haveBombInPlace = Physics.Raycast(ray, out RaycastHit hit, 1f, bombLayerMask);
        if (haveBombInPlace)
        {
            return false;
        }

        return true;
    }

    public void ReturnBomb(float count)
    {
        bombQuantity += count;
    }

    // GET - SET
    public float BombLength 
    { 
        get
        {
            return bombLength;
        }
        set
        {
            bombLength = value;
        }
    }
    public float BombCount
    {
        get
        {
            return bombCount;
        }
        set
        {
            bombQuantity += value - bombCount;
            bombCount = value;
        }
    }
}
