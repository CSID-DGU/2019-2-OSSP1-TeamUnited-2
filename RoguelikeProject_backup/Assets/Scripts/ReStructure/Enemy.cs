using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    protected GameObject target;
    public int meleeDamage;
    public double force;
    
    protected override void Start()
    {
        currentHP = HP;
        
        // 애니메이터가 있다면 설정해줍니다.
        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();
    }
    protected new void Update()
    {
        base.Update();
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Player")
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
}