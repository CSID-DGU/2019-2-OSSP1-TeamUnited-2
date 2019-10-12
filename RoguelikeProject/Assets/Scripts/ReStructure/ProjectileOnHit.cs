using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileOnHit : MonoBehaviour
{
    public int damage;
    public double force;
    public GameObject areaEffect;
    protected GameObject attacker;

    public void SetAttacker(GameObject attacker)
    // 공격자가 투사체에 영향받지 않기를 원한다면 설정합니다 (옵션)
    {
        this.attacker = attacker;
    }
    void OnTriggerStay2D(Collider2D col)
    {
        // 공격자는 투사체에 면역입니다. (일단은)
        if (col == attacker)
            return;

        // 대상이 유닛인 경우 맞은 대상에게 strike 객체를 전달하여 데미지를 입힙니다.
        if (col.gameObject.GetComponent<Unit>())
        {
            Strike strike = new Strike(damage, force, transform.position);
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
        }

        // 범위 공격을 하는 객체를 생성하여, 작동합니다.
        GameObject instance = Instantiate(areaEffect, transform.position, Quaternion.identity) as GameObject;


        Destroy(gameObject);
    }
}
