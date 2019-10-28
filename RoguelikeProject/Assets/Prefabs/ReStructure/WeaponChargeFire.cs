using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChargeFire : Wieldable
{
    public bool chargeFire;
    public int MaxLevel;
    public double[] chargeTime;
    protected double chargeWait = 0;
    protected int chargeLevel = 0;
    public double[] cooldown;
    protected double cooldownWait;

    public override void OnPush()
    {

    }

    public override void OnHold()
    {
        if (chargeFire)
        {
            if (chargeWait < chargeTime[chargeLevel])
            {
                chargeWait += 1.0f / 60.0f;
            }
            else
            {
                chargeWait = 0;
                if (chargeLevel < MaxLevel)
                {
                    chargeLevel++;
                }
            }
        }
    }

    public override void OnRelease()
    {
        GameObject projectile = Instantiate(bulletType[chargeLevel], (Vector2)transform.position + aim * 0.5f, owner.transform.rotation) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
        chargeLevel = 0;
        chargeWait = 0;
    }
}
