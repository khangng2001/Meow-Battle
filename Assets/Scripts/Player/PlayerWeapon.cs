using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private HavingWeaponState havingWeaponState = HavingWeaponState.NotHave;
    private UsingWeaponState usingWeaponState = UsingWeaponState.NotUsing;
    private float numberOfUse = 1;

    // GET - SET
    public HavingWeaponState HavingWeaponState
    {
        get
        {
            return havingWeaponState;
        }
        set
        {
            havingWeaponState = value;
        }
    }
    public UsingWeaponState UsingWeaponState
    {
        get
        {
            return usingWeaponState;
        }
        set
        {
            usingWeaponState = value;
        }
    }
    public float NumberOfUse
    {
        get
        {
            return numberOfUse;
        }
        set
        {
            numberOfUse = value;

            if (numberOfUse <= 0)
            {
                GetComponent<PlayerItem>().Unequip(numberOfUse);
            }
        }
    }
}
