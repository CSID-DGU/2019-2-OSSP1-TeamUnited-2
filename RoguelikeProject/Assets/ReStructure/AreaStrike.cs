using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaStrike : MonoBehaviour
{
    public int damage;
    public double force;
    public float radius;
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
                float distance = (transform.position - col.transform.position).sqrMagnitude;
                float distanceRate = distance / radius;

                col.gameObject.GetComponent<Unit>().GetStrike(actualStrike);
            }
        }
        Destroy(gameObject);
    }
}
