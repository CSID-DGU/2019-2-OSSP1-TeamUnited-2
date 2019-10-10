using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeArea : AttackTypeBase
{
    public double radius;

    AttackTypeArea(int damage, double force, double radius) : base(damage, force)
    {
        this.radius = radius;
    }
}
