using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AreaStrike : MonoBehaviour
{
    public int damage;
    public double force;
    public float radius;
    public float minimumForceRate = 0.25f;
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
    public void SetStatus(ProjectileAttribute a)
    {
        this.damage = a.areaDamage;
        this.force = a.areaForce;
        this.radius = a.areaRadius;
        this.minimumForceRate = 0.25f;
    }

    public void Activate()
    {
        Strike strike = new Strike(damage, force, transform.position, gameObject);
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Unit>())
            {
                // 실제로 상대가 받게되는 물리량을 계산합니다
                Strike actualStrike = new Strike(strike);

                // 공격자에게는 데미지가 들어가지 않습니다 (일단은)
                if (col.gameObject == attacker)
                    actualStrike.damage = 0;

                // 공격자와의 거리를 측정합니다
                Vector2 distanceVector = transform.position - col.transform.position;
                float distance = distanceVector.magnitude;

                // 거리 측정은 피격자의 중심에서 시작하기 때문에, 피격자의 충돌크기가 클 경우 범위보다 넓어질 수 있습니다
                // 우선은 실제 범위보다 피격자와의 거리가 클 경우 최대 범위로 설정해줍니다
                // TODO :: 추후 알고리즘의 수정이 필요할 것입니다.
                if (distance > radius)
                    distance = radius;
                
                // 거리가 멀 수록 넉백 위력은 약해집니다. 최대 거리에서는 minForceRate 퍼센트만의 위력이 전해집니다.
                if (minimumForceRate >= 0 && minimumForceRate <= 100)
                {
                    float forceMod = ((radius - distance) / radius);
                    forceMod = forceMod * (1.0f - minimumForceRate) + minimumForceRate;
                    actualStrike.force *= forceMod;
                }

                // 계산된 물리량을 전달합니다.
                col.gameObject.GetComponent<Unit>().GetStrike(actualStrike);
            }
        }
        Destroy(gameObject);
    }
}
