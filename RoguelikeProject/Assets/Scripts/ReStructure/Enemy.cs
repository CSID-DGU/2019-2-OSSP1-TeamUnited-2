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
        Destroy(gameObject);
        Debug.Log(GameObject.Find("GameManager").GetComponent<GameManager>().EnemyNum);
        if (--GameObject.Find("GameManager").GetComponent<GameManager>().EnemyNum == 0)
            GameObject.Find("backGround").GetComponent<BackGround>().nextStage();
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
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15.0f);
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