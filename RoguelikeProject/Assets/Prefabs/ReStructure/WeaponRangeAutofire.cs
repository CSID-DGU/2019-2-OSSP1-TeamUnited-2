using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeAutofire : Wieldable
{
    public bool autoFire;
    public double[] cooldown;
    protected double cooldownWait;
    public override void OnHold()
    {
        if (autoFire)
        {
            if (cooldownWait > 0)
            {
                cooldownWait -= 1.0f / 60.0f;
            }
            else
            {
                OnPush();
                cooldownWait = cooldown[0];
            }
        }
    }
}
