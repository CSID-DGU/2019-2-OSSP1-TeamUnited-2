using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileManager
{
    public ProjectileAttribute attribute;
    public GameObject shape; // Sprite, RigidBody2D, Collider로 구성된 형체 오브젝트
    protected GameObject entity = null; // 실제 투사체를 생성하기 위한 복제본. 발사가능한 투사체와 동일한 객체여야 합니다.
    public GameObject Entity 
    {
        // 호출과 동시에 객체를 생성합니다.
        get
        {
            // 객체가 생성된 적이 있다면 그대로 반환합니다.
            if (entity != null) 
            {
                return entity;
            }
            // 객체를 그냥 호출하였을 경우 생성해야합니다. Shape의 물리적 속성에 투사체의 속성(ProjectileOnHit)을 더합니다.
            else
            {
                entity = shape;
                entity.AddComponent<ProjectileOnHit>().SetAttribute(attribute);
                return entity;
            }
        }
        // 직접 객체를 불러와 만들 수도 있습니다.
        set { entity = value; }
    }
}
