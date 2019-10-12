using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour
{
    public GameObject[] bulletType;
    public bool autoFire;
    protected bool holding;
    public double[] cooldown;
    protected double cooldownWait;
    public GameObject owner;
    public Vector2 aim;

    public void SetHold()
    {
        holding = true;
    }
    public void SetUnhold()
    {
        holding = false;
    }

    public void OnPush()
    {
        // Vector2 direction = aim;
        // direction.Normalize();

        // Vector3 pos = owner.transform.position;
        // pos.x += direction.x;
        // pos.y += direction.y;

        GameObject bullet = Instantiate(bulletType[0], (Vector2)transform.position + aim, owner.transform.rotation) as GameObject;
        bullet.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        bullet.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
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
