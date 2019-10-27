using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeAutofire : Wieldable
{
    public GameObject[] bulletType;
    public bool autoFire;
    public double[] chargeTime;
    protected double charge;
    public double[] cooldown;
    protected double cooldownWait;
    protected bool holding;
    public GameObject owner;
    public Vector2 aim;
    public void OnPush()
    {

    }

    public void OnHold()
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

    public void OnRelease()
    {

    }
    protected void Update()
    {

    }
}
