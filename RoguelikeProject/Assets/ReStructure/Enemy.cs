using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    protected GameObject target;
    public int att;
    public double force;
    Animator anim;
    float once; // 애니메이션 트리거 한번만 작동하게..

    new void Start()
    {
        currentHP = HP;
        anim = GetComponent<Animator>();
    }
    protected new void Update()
    {
        base.Update();
        once -= Time.deltaTime;
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Player")
        {
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(att, force, transform.position));

            if (once < 0) // 트리거 한번만 실행되게..
                anim.SetTrigger("New Trigger");
            once = 1f;
        }
    }
}
