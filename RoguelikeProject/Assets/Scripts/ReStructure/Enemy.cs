using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public Wieldable weapon;
    protected GameObject target;
    protected GameObject player;
    public GameObject heartPotion;
    public int meleeDamage;
    public double force;

    protected override void Start()
    {
        currentHP = HP;

        // 애니메이터가 있다면 설정해줍니다.
        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();

        InvokeRepeating("FindPlayer", Random.Range(0.0f, 1.0f), 1);
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
    protected override void SelfDestruction()
    {
        Instantiate(heartPotion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<Player>())
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
    void FindPlayer()
    {
        player = GameObject.Find("Player");

        float distance = Vector2.Distance(player.transform.position, transform.position);
        if (distance < 15.0f)
        {
            Debug.Log("Tracking" + distance);
            target = player;
        }
        else
        {
            Debug.Log("idle");
            target = null;
        }

        // if (target == null)
        // {
        //     Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15.0f);
        //     foreach (Collider2D col in cols)
        //     {
        //         if (col.gameObject.GetComponent<Player>())
        //         {
        //             target = col.gameObject;
        //         }
        //     }
        // }
    }
}