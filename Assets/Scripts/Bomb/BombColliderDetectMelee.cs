using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BombColliderDetectMelee : NetworkBehaviour
{
    private Vector3 newPosition;
    private Vector3 direction;
    private Vector3 directionBack;
    private bool isMove;
    private bool isBack;

    private Vector3 GuessDirection(Vector3 position, Vector3 pointHit)
    {
        Vector3 guessDirection = position - pointHit;

        if (Mathf.Abs(guessDirection.x) >= Mathf.Abs(guessDirection.z)) // Use Mathf, Find (Forward - Back) Or (Right - Left)
        {
            if (guessDirection.x >= 0f)                                 // Use Mathf, Find Right - Left
            {
                directionBack = Vector3.right;
                return Vector3.right;
            }
            else
            {
                directionBack = Vector3.left;
                return Vector3.left;
            }
        }
        else
        {
            if (guessDirection.z >= 0f)                                 // Use Mathf, Find Forward - Back
            {
                directionBack = Vector3.forward;
                return Vector3.forward;
            }
            else
            {
                directionBack = Vector3.back;
                return Vector3.back;
            }
        }
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

    private void Moving(Vector3 dir)
    {
        //transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, newPosition + dir, 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, newPosition + dir) <= 0f)
        {
            isMove = false;
            transform.position = newPosition + dir;
        }
    }

    private void BackOne(Vector3 dirBack)
    {
        //transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, dirBack, 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, dirBack) <= 0f)
        {
            isBack = false;
            transform.position = dirBack;
        }
    }

    public void MovementWhenDetectedByMelee(BombController bombController)
    {
        if (isMove)
        {
            Moving(this.direction);
            if (bombController.TimeExist <= 0)
            {
                if ((int)transform.position.x % 2 != 0 && (int)transform.position.z % 2 != 0)
                {
                    transform.position = new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z);
                    bombController.CanBoom = true;
                }
            }
            else
            {
                bombController.CanBoom = false;
            }
        }
        else if (isBack)
        {
            BackOne(this.directionBack);
            if (bombController.TimeExist <= 0)
            {
                if ((int)transform.position.x % 2 != 0 && (int)transform.position.z % 2 != 0)
                {
                    transform.position = new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z);
                    bombController.CanBoom = true;
                }
            }
            else
            {
                bombController.CanBoom = false;
            }
        }
        else
        {
            bombController.CanBoom = true;
        }
    }

    public void OnCollideMelle(float timeStun, Vector3 pointHit, int strength)
    {
        GetComponent<Rigidbody>().isKinematic = false;

        newPosition = transform.position;
        direction = GuessDirection(newPosition, pointHit);
        direction = direction * strength * 2f;

        isMove = true;
        isBack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")
             && other.gameObject.layer != LayerMask.NameToLayer("Bomb")
             && other.gameObject.layer != LayerMask.NameToLayer("Destructibles")
             && other.gameObject.layer != LayerMask.NameToLayer("Indestructibles")) return;

        if (isMove)
        {
            Vector3 pos = other.gameObject.transform.position;
            directionBack = new Vector3(RoundNumber(pos.x), transform.position.y, RoundNumber(pos.z)) - directionBack * 2f;

            isBack = true;
            isMove = false;
        }
    }

    // GET - SET
    public Vector3 NewPosition
    {
        get
        {
            return newPosition;
        }
        set
        {
            newPosition = value;
        }
    }
    public Vector3 Direction
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;
        }
    }
    public bool IsMove
    {
        get
        {
            return isMove;
        }
        set
        {
            isMove = value;
        }
    }
    public bool IsBack
    {
        get
        {
            return isBack;
        }
        set
        {
            isBack = value;
        }
    }
}
