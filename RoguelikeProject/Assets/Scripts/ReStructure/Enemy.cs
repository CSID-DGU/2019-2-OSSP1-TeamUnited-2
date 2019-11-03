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

        InvokeRepeating("FindPlayer", 0, 1);
    }
    protected new void Update()
    {
        base.Update();

        if (target)
        {
            Vector2 direction = target.transform.position - transform.position;
            Move(direction);
        }
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<Player>())
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
    void FindPlayer()
    {
        if (target == null)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 40.0f);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.GetComponent<Player>())
                {
                    target = col.gameObject;
                }
            }
        }
    }
}