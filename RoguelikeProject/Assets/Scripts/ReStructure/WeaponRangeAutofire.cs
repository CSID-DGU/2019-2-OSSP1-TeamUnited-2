using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeAutofire : Wieldable
{
    public GameObject bulletType;
    public bool autoFire;
    public double cooldown;

    public override void OnPush()
    {
        if (cooldownWait <= 0)
        {
            Debug.Log("Weapon Pushed");
            FireRangeDirect(bulletType);
            cooldownWait = cooldown;
        }
    }
    public override void OnHold()
    {
        if (autoFire)
        {
            if (cooldownWait <= 0)
            {
                FireRangeDirect(bulletType);
                cooldownWait = cooldown;
            }
        }
    }
}
