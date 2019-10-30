using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeAutofire : Wieldable
{
    public bool autoFire;
    public double cooldown;
    protected double cooldownWait;

    public override void OnPush()
    {
        FireRangeDirect(bulletType[0]);
    }
    public override void OnHold()
    {
        if (autoFire)
        {
            if (cooldownWait > 0)
            {
                cooldownWait -= Time.deltaTime;
            }
            else
            {
                FireRangeDirect(bulletType[0]);
                cooldownWait = cooldown;
            }
        }
    }
}
