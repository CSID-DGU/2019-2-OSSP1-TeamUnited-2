using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileOnHit : MonoBehaviour
{
    public int damage;
    public double force;
    public int areaDamage;         
    public double areaForce;
    public float areaRadius;
    public GameObject Animation;
    protected GameObject areaEffect;
    protected GameObject attacker;
    protected bool triggered = false;
    public void SetAttacker(GameObject attacker)
    // 공격자가 투사체에 영향받지 않기를 원한다면 설정합니다 (옵션)
    {
        this.attacker = attacker;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 공격자는 투사체에 면역입니다. (일단은)
        if (col.gameObject == attacker)
            return;

        // 형체가 없는 대상에는 반응하지 않습니다.
        if (col.gameObject.GetComponent<Rigidbody2D>() == null)
            return;

        // 이미 작동된 투사체는 중복작동하지 않습니다.
        lock(this)
        {
            if (triggered)
                return;
            else
                triggered = true;
        }

        // 여기까지 왔다면 어떤 형태로든 투사체가 작동한 것입니다.
        // 대상이 유닛인 경우 맞은 대상에게 strike 객체를 전달하여 데미지를 입힙니다.
        if (col.gameObject.GetComponent<Unit>())
        {
            Strike strike = new Strike(damage, force, transform.position);
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
        }

        // 범위 공격을 하는 객체를 생성하여, 작동합니다.
        areaEffect = new GameObject();
        areaEffect.transform.position = transform.position;
        // areaEffect = Instantiate(sample, transform.position, Quaternion.identity) as GameObject;
        areaEffect.AddComponent<AreaStrike>();
        areaEffect.GetComponent<AreaStrike>().SetStatus(areaDamage, areaForce, areaRadius);
        areaEffect.GetComponent<AreaStrike>().SetAttacker(attacker);
        areaEffect.GetComponent<AreaStrike>().Activate();

        // 애니메이션을 재생합니다
        if (Animation)
        {
            Instantiate(Animation, transform.position, Quaternion.identity);
        }
        // GameObject instance = Instantiate(areaEffect, transform.position, Quaternion.identity) as GameObject;

        Destroy(gameObject);
    }
}
