using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeArea
{
    
    public int damage;
    public double force;
    public double radius;

    public AttackTypeArea(int damage, double force, double radius)
    {
        this.damage = damage;
        this.force = force;
        this.radius = radius;
    }
}
