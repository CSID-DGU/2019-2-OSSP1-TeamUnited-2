using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    protected GameObject target;
    public int att;
    public double force;
    protected Animator enemyAnimator;
    

    new void Start()
    {
        currentHP = HP;
        enemyAnimator = GetComponent<Animator>();
    }
    protected new void Update()
    {
        base.Update();
    }

    public new void GetStrike(Strike strike)
    {
        base.GetStrike(strike);
        enemyAnimator.SetTrigger("New Trigger");
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Player")
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(att, force, transform.position));
    }
}