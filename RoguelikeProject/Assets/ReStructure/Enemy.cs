using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    protected GameObject target;
    public int att;

    protected new void Update()
    {
        base.Update();
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Player") ;
            //coll.gameObject.GetComponent<Player>().color

    }
}
