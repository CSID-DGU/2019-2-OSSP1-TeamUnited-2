using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaStrike : MonoBehaviour
{
    public int damage;
    public double force;
    public double radius;
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
    public void SetStatus(int damage, double force, double radius)
    {
        this.damage = damage;
        this.force = force;
        this.radius = radius;
    }

    // protected IEnumerator SelfDestruct()
    // {
    //     yield return new WaitForSeconds(5);
    //     Destroy(gameObject);
    // }
    public void Activate()
    {
        Strike strike = new Strike(damage, force, transform.position, gameObject);
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, (float)radius);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Unit>())
            {
                if (col.gameObject == attacker)
                {
                    Strike emptyStrike = new Strike(strike);
                    emptyStrike.damage = 0;
                    col.gameObject.GetComponent<Unit>().GetStrike(emptyStrike);
                }
                else
                {
                    col.gameObject.GetComponent<Unit>().GetStrike(strike);
                }
            }
        }
        // StartCoroutine(SelfDestruct());
        Destroy(gameObject);
    }
}
