using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    public GameObject target;
    public int meleeDamage;
    public double force;

    protected override void Start()
    {
        currentHP = HP;
    }
    protected new void Update()
    {
        base.Update();
        Vector2 direction = target.transform.position - transform.position;
        Move(direction);

        float angle = Mathf.Atan2(transform.position.y - target.transform.position.y, transform.position.x - target.transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }
    protected override void SelfDestruction()
    {
        Destroy(gameObject);
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<Player>())
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
}