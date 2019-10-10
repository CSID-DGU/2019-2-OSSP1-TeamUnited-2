using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeBase : MonoBehaviour
{
    public int damage;
    public double force;

    public AttackTypeBase(int damage, double force)
    {
        this.damage = damage;
        this.force = force;
    }
}
