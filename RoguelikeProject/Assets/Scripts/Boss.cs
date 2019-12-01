using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    public static int BossHP;
    public int meleeDamage;
    public double force;

    protected override void SelfDestruction()
    {
        gameObject.transform.parent.parent.gameObject.SetActive(false);
        //Destroy(gameObject.transform.parent.parent.gameObject);
    }

    protected override void Start()
    {
        BossHP = 5000;
    }

    public override int GetDamage(int damage)
    {
        // 실제로 받게 될 데미지를 추적연산합니다.
        int actualDamage = damage;

        // 실제 연산
        BossHP -= actualDamage;
        Debug.Log("Boss Hit!! : " + actualDamage + ", " + BossHP);
        // 추적된 값을 반환합니다.        
        return actualDamage;
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<Player>())
            coll.gameObject.GetComponent<Player>().GetStrike(new Strike(meleeDamage, force, transform.position));
    }
    protected new void Update()
    {
        if (BossHP <= 0)
        {
            SelfDestruction();
        }
    }
}
