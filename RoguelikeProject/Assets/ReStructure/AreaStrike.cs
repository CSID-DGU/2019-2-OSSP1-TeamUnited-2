using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AreaStrike : MonoBehaviour
{
    public int damage;
    public double force;
    public float radius;
    public float minForceRate = 50f;
    protected Vector2 attackerPosition;
    protected GameObject attacker;

    public void SetAttacker(Vector2 attackerPosition, GameObject attacker = null)
    {
        this.attackerPosition = attackerPosition;
        this.attacker = attacker;
    }
    public void SetAttacker(GameObject attacker)
    {
        this.attackerPosition = attacker.transform.position;
        this.attacker = attacker;
    }
    public void SetStatus(int damage, double force, float radius)
    {
        this.damage = damage;
        this.force = force;
        this.radius = radius;
    }

    public void Activate()
    {
        Strike strike = new Strike(damage, force, transform.position, gameObject);
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Unit>())
            {
                Strike actualStrike = new Strike(strike);
                if (col.gameObject == attacker)
                {
                    actualStrike.damage = 0;
                }
                // 공격자와의 거리를 측정합니다                
                float distance = (transform.position - col.transform.position).sqrMagnitude;
                
                // 거리가 멀 수록 위력은 약해집니다. 최대 거리에서는 minForceRate 퍼센트만의 위력이 전해집니다.
                float forceMod = (distance / radius) * (1 - minForceRate) + minForceRate;
                actualStrike.force *= forceMod;

                col.gameObject.GetComponent<Unit>().GetStrike(actualStrike);
            }
        }
        Destroy(gameObject);
    }
}
