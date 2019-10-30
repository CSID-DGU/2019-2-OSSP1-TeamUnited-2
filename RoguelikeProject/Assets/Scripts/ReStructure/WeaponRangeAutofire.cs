using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeAutofire : Wieldable
{
    public ProjectileManager projectile;
    public bool autoFire;
    public double cooldown;

    public override void OnPush()
    {
        if (cooldownWait <= 0)
        {
            FireRangeDirect(projectile.Entity);
            cooldownWait = cooldown;
        }
    }
    public override void OnHold()
    {
        if (autoFire)
        {
            if (cooldownWait <= 0)
            {
                FireRangeDirect(projectile.Entity);
                cooldownWait = cooldown;
            }
        }
    }
}
