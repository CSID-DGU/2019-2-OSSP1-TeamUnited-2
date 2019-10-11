using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileOnHit : MonoBehaviour
{
    public AttackTypeBase singleAttack;
    public AttackTypeArea areaAttack;

    void OnTriggerEnter2D(Collider2D col)
    {

    }
}
