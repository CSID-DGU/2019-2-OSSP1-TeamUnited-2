using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour, IWieldable
{
    public GameObject[] bulletType;
    public bool autoFire;
    public double[] cooldown;
    protected double cooldownWait;
    public GameObject owner;
    public Vector2 aim;

    public void OnPush()
    {
        GameObject projectile = Instantiate(bulletType[0], (Vector2)transform.position + aim * 0.5f, owner.transform.rotation) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
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
        // TODO :: 무기가 해제당했을 경우 소유자를 해제해 주는 것이 만약을 위해 안전할 것입니다.
    }
}
