using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeAutofire : Wieldable
{
    public ProjectileManager projectile;
    public bool autoFire;
    public double cooldown;
    void Start()
    {
        Debug.Log("Range Autofire Initialize");
        ProjectileInstantiation(projectile);
    }
    protected void Fire()
    {
        lock(this)
        {
            if (cooldownWait <= 0)
            {
                FireRangeDirect(projectile.Entity);
                cooldownWait = cooldown;
            }
        }
    }

    public override void OnPush()
    {
        if (!autoFire)
        {
            Fire();
        }
    }
    public override void OnHold()
    {
        if (autoFire)
        {
            Fire();
        }
    }
}
