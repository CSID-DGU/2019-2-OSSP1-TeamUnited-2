using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour, IWieldable
{
    public GameObject[] bulletType;
    public double[] cooldown;
    protected double cooldownWait;
    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 aim;

    public struct projectileAttribute
    {
        int damage;
        double force;
        int areaDamage;
        double areaForce;
        float radius;
    }

    public projectileAttribute[] projectiles;
    public void start()
    {
        
    }

    public void OnPush()
    {
        GameObject projectile = Instantiate(bulletType[0], (Vector2)transform.position + aim * 0.5f, owner.transform.rotation) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
    }

    public void OnHold()
    {

    }

    public void OnRelease()
    {

    }

    protected void Update()
    {
        // TODO :: 무기가 해제당했을 경우 소유자를 해제해 주는 것이 만약을 위해 안전할 것입니다.
    }
}
