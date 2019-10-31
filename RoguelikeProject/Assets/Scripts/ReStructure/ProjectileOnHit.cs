using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileOnHit : MonoBehaviour
{
    // 현재 attribute 설정은 ProjcectileManager에서 이루어집니다
    public ProjectileAttribute attribute;
    public ProjectileAttribute Attribute
    {
        get { return attribute; }
        set 
        {   
            Debug.Log("setting attribute(1)");
            attribute.damage                 = value.damage;
            attribute.force                  = value.force;
            attribute.areaDamage             = value.areaDamage;
            Debug.Log("setting attribute(2)");
            attribute.areaForce              = value.areaForce;
            attribute.areaRadius             = value.areaRadius;
            attribute.animationExplosion     = value.animationExplosion;
        }
    }
    // 현재 attacker의 설정은 ProjectileOnHit 객체에서 발사 직후 이루어집니다
    protected GameObject attacker;
    public GameObject Attacker
    {
        get { return attacker; }
        set { attacker = value; }
    }
    protected bool triggered = false;

    Animator EnemyAni; // 애니메이션 쓸거임


    void OnTriggerEnter2D(Collider2D col)
    {
        // 이미 작동된 투사체는 중복작동하지 않습니다.
        lock(this)
        {
            if (triggered)
                return;
            else
                triggered = true;
        }

        // 공격자 없는 투사체가 발사되었을 경우
        if (attacker == null)
        {
            Debug.LogError("Projectile Fired Without Owner, Set Inactive");
            triggered = true;
            return;
        }

        // 공격자는 투사체 충돌에 면역입니다.
        if (col.gameObject == attacker)
            return;

        // 형체가 없는 대상에는 반응하지 않습니다.
        if (col.gameObject.GetComponent<Rigidbody2D>() == null)
            return;


        // 여기까지 왔다면 어떤 형태로든 투사체가 작동한 것입니다.
        // 대상이 유닛인 경우 맞은 대상에게 strike 객체를 전달하여 데미지를 입힙니다.
        if (col.gameObject.GetComponent<Unit>())
        {
            Strike strike = new Strike(attribute.damage, attribute.force, transform.position);
            if ((attribute.force >= 0) != true)
                Debug.LogError("ERROR :: Negative Force" + attribute.force);
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
            
            EnemyAni = col.gameObject.GetComponent<Animator>();
            EnemyAni.SetTrigger("New Trigger");
        }

        // 범위 공격을 하는 객체를 생성합니다.
        // areaEffect = Instantiate(sample, transform.position, Quaternion.identity) as GameObject;
        GameObject areaEffect = new GameObject();
        areaEffect.transform.position = transform.position;

        // 범위 공격기능을 하는 컴포넌트를 추가하고 설정해줍니다.
        areaEffect.AddComponent<AreaStrike>();
        areaEffect.GetComponent<AreaStrike>().SetStatus(attribute);

        // 공격자가 존재한다면 공격자도 설정해 줍니다.
        if (attacker != null)
            areaEffect.GetComponent<AreaStrike>().SetAttacker(attacker);

        // 범위 공격 객체를 작동시킵니다. 해당 객체는 작동 후 자동 소멸할 것입니다.
        // TODO :: 객체의 소멸 관리가 직접적으로 이루어지지 않는 점이 구조적 불안정을 유발할 가능성이 있습니다. 추후 구조개선이 필요할 수 있습니다.
        areaEffect.GetComponent<AreaStrike>().Activate();

        // 애니메이션을 재생합니다
        if (attribute.animationExplosion)
        {
            Instantiate(attribute.animationExplosion, transform.position, Quaternion.identity);
        }
        // GameObject instance = Instantiate(areaEffect, transform.position, Quaternion.identity) as GameObject;

        Destroy(gameObject);
    }
}
