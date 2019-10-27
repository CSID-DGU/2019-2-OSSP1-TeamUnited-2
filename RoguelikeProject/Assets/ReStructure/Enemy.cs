using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    protected GameObject target;
    public int att;
    public double force;

    new void Start()
    {
        currentHP = HP;
    }
    protected new void Update()
    {
        base.Update();
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Player")
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(att, force, transform.position));
    }
}
