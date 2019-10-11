using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strike
{
    public Vector2 attackPosition;
    public AttackTypeBase attackType;
    public GameObject attacker;

    public Strike(int damage, double force, Vector2 attackPosition, GameObject attacker = null)
    {
        this.attackType = new AttackTypeBase(damage, force);
        this.attackPosition = attackPosition;
    }

    public Strike(AttackTypeBase attackType, Vector2 attackPosition, GameObject attacker = null)
    {
        this.attackType = attackType;
        this.attackPosition = attackPosition;
    }

    // TODO : 곱연산자의 오버라이딩 필요, 곱셈으로 AttackTypeBase의 force와 damage를 비율적으로 조작
}
