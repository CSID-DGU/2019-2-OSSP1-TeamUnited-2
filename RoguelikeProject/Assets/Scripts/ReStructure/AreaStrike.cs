using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaStrike : MonoBehaviour
{
    // public AttackTypeArea attackType;
    public int damage;
    public double force;
    public double radius;
    public Vector2 attackerPosition;
    public GameObject attacker;


    // public void SetStatus(int damage, double force, double radius)
    // {
    //     this.damage = damage;
    //     this.force = force;
    //     this.radius = radius;
    // }
    public void SetAttacker(Vector2 attackerPosition, GameObject attacker = null)
    {
        this.attackerPosition = attackerPosition;
        this.attacker = attacker;
    }
    public void Awake()
    {
        Strike strike = new Strike(damage, force, transform.position, gameObject);
        // TODO :: 범위 내 GameObject 객체를 추출하여, 해당 객체가 Unit이라면 GetStrike를 호출하며 Strike 객체를 전달한다.
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, (float)radius);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Unit>())
            {
                col.gameObject.GetComponent<Unit>().GetStrike(strike);
            }
        }

        Destroy(gameObject);
    }
}
