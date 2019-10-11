using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaStrike : MonoBehaviour
{
    // public Vector2 centorPosition;
    public double radius;
    public AttackTypeArea attackType;
    public Vector2 attackerPosition;
    public GameObject attacker;


    public void SetAttacker(Vector2 attackerPosition, GameObject attacker = null)
    {
        this.attackerPosition = attackerPosition;
        this.attacker = attacker;
    }
    public void Activate()
    {
        Strike strike;
        // TODO :: 범위 내 GameObject 객체를 추출하여, 해당 객체가 Unit이라면 GetStrike를 호출하며 Strike 객체를 전달한다.
    }
}
