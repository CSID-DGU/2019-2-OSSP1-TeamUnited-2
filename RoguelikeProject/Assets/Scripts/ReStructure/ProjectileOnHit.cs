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
        set { attribute = value; }
    }
    // 현재 attacker의 설정은 ProjectileOnHit 객체에서 발사 직후 이루어집니다
    protected GameObject attacker;
    public GameObject Attacker
    {
        get { return attacker; }
        set { attacker = value; }
    }
    protected bool triggered = false;


    void OnTriggerEnter2D(Collider2D col)
    {
        // --여기서부터 조건처리를 합니다.--
        // 투사체 끼리는 충돌하지 않습니다. (일단은)
        if (col.gameObject.GetComponent<ProjectileOnHit>())
            return;

        // 공격자는 투사체 충돌에 면역입니다.
        if (col.gameObject == attacker)
            return;

        // 형체가 없는 대상에는 반응하지 않습니다.
        if (col.gameObject.GetComponent<Rigidbody2D>() == null)
            return;

        // --조건처리에는 걸리지 않았으나 공격자 없는 투사체일 경우 에러메시지를 송출합니다--
        // 에러메시지가 발생하였으면 어떤 형태로든 잠재적인 문제가 있는것입니다. 이부분을 지우지 마세요.
        if (attacker == null)
        {
            Debug.LogError("Projectile Fired Without Owner, Set Inactive : " + GetInstanceID());
            transform.position = new Vector3(-1000,-1000,-1000);
            return;
        }

        // --여기까지 왔다면 투사체가 무사히 작동한 것입니다.--
        // 이미 작동된 투사체는 중복작동하지 않습니다.
        lock(this)
        {
            if (triggered)
                return;
            else
                triggered = true;
        }

        // 대상이 유닛인 경우 맞은 대상에게 strike 객체를 전달하여 데미지를 입힙니다.
        if (col.gameObject.GetComponent<Unit>())
        {
            Strike strike = new Strike(attribute.damage, attribute.force, transform.position);
            if ((attribute.force >= 0) != true)
                Debug.LogError("Force should be non-negative value" + attribute.force);
            col.gameObject.GetComponent<Unit>().GetStrike(strike);
        }

        // 범위 공격을 하는 객체를 생성하고 부모를 투사체로 설정합니다.
        GameObject areaEffect = new GameObject();
        areaEffect.transform.position = transform.position;
        areaEffect.transform.SetParent(transform);

        // 범위 공격기능을 하는 컴포넌트를 추가하고 공격자와 속성을 설정해줍니다.
        areaEffect.AddComponent<AreaStrike>();
        areaEffect.GetComponent<AreaStrike>().SetStatus(attribute);
        areaEffect.GetComponent<AreaStrike>().SetAttacker(attacker);

        // 범위 공격 객체를 작동시킵니다. 해당 객체는 작동 후 자동 소멸할 것입니다.
        // TODO :: 객체의 소멸 관리가 직접적으로 이루어지지 않는 점이 구조적 불안정을 유발할 가능성이 있습니다. 추후 구조개선이 필요할 수 있습니다.
        areaEffect.GetComponent<AreaStrike>().Activate();

        // 애니메이션이 있다면 재생합니다
        if (attribute.animationExplosion)
        {
            GameObject instance = Instantiate(attribute.animationExplosion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
