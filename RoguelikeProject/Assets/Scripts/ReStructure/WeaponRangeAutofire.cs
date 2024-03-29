﻿using System.Collections;
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
    protected void TryFire()
    {
        if (cooldownWait <= 0)
        {
            if (projectile.Entity == null)
            {
                Debug.LogError("Projectile should be instantiated before use");
                return;
            }
            FireRangeDirect(projectile.Entity);
            cooldownWait = cooldown;
        }
    }

    public override void OnPush()
    {
        TryFire();
    }
    public override void OnHold()
    {
        if (autoFire)
        {
            TryFire();
        }
    }
    public override void OnRelease() {}
}
