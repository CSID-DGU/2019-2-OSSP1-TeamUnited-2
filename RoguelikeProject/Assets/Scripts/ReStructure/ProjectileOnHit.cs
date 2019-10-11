using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileOnHit : MonoBehaviour
{
    public AttackTypeBase singleAttack;
    public GameObject areaStrikeGenerator;
    public int damage;
    public double force;

    private void Start()
    {
        if (singleAttack == null)
        {
            singleAttack = new AttackTypeBase(damage, force);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 맞은 대상에게 strike 객체를 전달하여 데미지를 입힙니다.
        Strike strike = new Strike(singleAttack, transform.position, gameObject);
        if (col.gameObject.GetComponent<Unit>())
        {
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
        }

        // 범위 공격을 하는 객체를 생성하여, 작동합니다.
        GameObject instance = Instantiate(areaStrikeGenerator, transform.position, Quaternion.identity) as GameObject;
        instance.transform.SetParent(gameObject.transform);
        instance.GetComponent<AreaStrike>().Activate();

        Destroy(gameObject);
    }
}
