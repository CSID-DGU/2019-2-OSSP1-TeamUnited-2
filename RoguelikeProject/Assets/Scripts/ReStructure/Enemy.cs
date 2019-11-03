using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    public GameObject target;
    public int meleeDamage;
    public double force;


    protected override void Start()
    {
        currentHP = HP;
        if(this.gameObject.name == "Enemy")
        {
            maxSpeed = 0.5;
            acceleration = 1;
        }
        
        // 애니메이터가 있다면 설정해줍니다.
        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();
    }
    protected new void Update()
    {
        base.Update();
        Vector2 direction = target.transform.position - transform.position;
        Move(direction);
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Player")
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
}