using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChargeFire : Wieldable
{
    public GameObject[] bulletType;
    public bool chargeFire;
    public double[] chargeTime;
    protected double chargeWait = 0;
    protected int chargeLevel = 0;

    public override void OnPush()
    {
        if (!chargeFire)
        {
            if (cooldownWait < 0)
            {
                FireRangeDirect(bulletType[0]);
            }
        }
    }

    public override void OnHold()
    {
        if (chargeFire)
        {
            if (chargeWait < chargeTime[chargeLevel])
            {
                chargeWait += Time.deltaTime;
            }
            else
            {
                chargeWait = 0;
                if (chargeLevel < chargeTime.Length)
                {
                    chargeLevel++;
                }
            }
        }
    }

    public override void OnRelease()
    {
        FireRangeDirect(bulletType[chargeLevel]);
        chargeLevel = 0;
        chargeWait = 0;
    }
}
