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
        GameObject projectile = Instantiate(bulletType[0], (Vector2)owner.transform.position + rotationVector * 0.5f, Quaternion.identity) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(rotationVector * 1000.0f);
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
                FireRange(bulletType[0]);
                cooldownWait = cooldown;
            }
        }
    }
}
