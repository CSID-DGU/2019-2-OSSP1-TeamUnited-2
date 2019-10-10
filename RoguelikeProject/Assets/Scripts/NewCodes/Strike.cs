using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strike : MonoBehaviour
{
    public Vector2 attackPosition;
    public AttackTypeBase attackType;
    public GameObject attacker;

    Strike(Vector2 attackPosition, int damage, double force, GameObject attacker = null)
    {
        this.attackPosition = attackPosition;
        this.attackType = new AttackTypeBase(damage, force);
    }

    // TODO : 곱연산자의 오버라이딩 필요, 곱셈으로 AttackTypeBase의 force와 damage를 비율적으로 조작
}
