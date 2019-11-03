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
        if(this.gameObject.name == "Enemy")
        {
            maxSpeed = 0.5;
            acceleration = 1;
        }
        
        // 애니메이터가 있다면 설정해줍니다.
        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();

        InvokeRepeating("findPlayer", 0, 1);
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
    void findPlayer()
    {
        if (target.GetComponent<Player>())
            return;

        if (this.gameObject.name == "Enemy")
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 40.0f);
            foreach (Collider2D col in cols)
            {
                Debug.Log(col);
                if (col.gameObject.GetComponent<Player>())
                {
                    Vector2 direction = col.gameObject.GetComponent<Player>().transform.position - transform.position;
                    Move(direction);
                }
            }
        }
    }
}