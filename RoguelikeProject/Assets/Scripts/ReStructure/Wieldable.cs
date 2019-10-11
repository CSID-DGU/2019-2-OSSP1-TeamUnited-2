using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour
{
    public GameObject[] bulletType;
    public bool autoFire;
    public bool holding;
    public double[] cooldown;
    protected double cooldownWait;
    public GameObject owner;
    public Vector2 aim;

    public void OnPush()
    {
        
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
        mousePosition.x - owner.transform.position.x,
        mousePosition.y - owner.transform.position.y
        );
        direction.Normalize();

        Vector3 pos = owner.transform.position;
        pos.x += direction.x;
        pos.y += direction.y;

        GameObject bullet = Instantiate(bulletType[0], pos, owner.transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody2D>().AddForce(direction * 1000.0f);
    }

    public void OnHold()
    {

    }

    public void OnRelease()
    {

    }

    protected void Update()
    {

    }
}
