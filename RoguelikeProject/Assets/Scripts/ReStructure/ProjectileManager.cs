using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileManager
{
    public ProjectileAttribute attribute;
    public GameObject shape; // Sprite, Collider로 구성된 형체 오브젝트
    protected GameObject entity; // 실제 투사체를 생성하기 위한 복제본. 발사가능한 투사체와 동일한 객체여야 합니다.\
    public ProjectileManager()
    {
        if (!shape)
            Debug.LogError("Projectile without shape");
        entity = shape;
        entity.AddComponent<ProjectileOnHit>();
        entity.GetComponent<ProjectileOnHit>().Attribute = attribute;
    }
    public GameObject Entity 
    {
        // 호출과 동시에 객체를 생성합니다.
        get
        {
            Debug.Log("Entity called: " + entity.GetInstanceID());
            if (entity.GetComponent<ProjectileOnHit>())
            {}
            else
            {
                Debug.LogError("error");
            }
            return entity;
        }
        // 직접 객체를 불러와 만들 수도 있습니다.
        set { entity = value; }
    }
}
