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
    {
        this.attacket = attacker;
    }
    void OnTriggerEnter2D(Collider2D col)
    {

        // 범위공격 객체는 생성과 동시에 작동합니다.
        // areaStrikeGenerator = new GameObject();
        // areaStrikeGenerator.AddComponent<AreaStrike>();
        // areaStrikeGenerator.GetComponent<AreaStrike>().SetStatus(damage, force, radius);

        // 맞은 대상에게 strike 객체를 전달하여 데미지를 입힙니다.
        Strike strike = new Strike(damage, force, transform.position);
        if (col.gameObject.GetComponent<Unit>())
        {
            Debug.Log("Someone Hit");
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
        }

        // 범위 공격을 하는 객체를 생성하여, 작동합니다.
        GameObject instance = Instantiate(areaEffect, transform.position, Quaternion.identity) as GameObject;


        Destroy(gameObject);
    }
}
