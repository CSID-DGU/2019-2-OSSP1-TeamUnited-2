using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileManager
{
    public ProjectileAttribute attribute;
    public GameObject shape; // Sprite, Collider, RigidBody로 구성된 형체 오브젝트
    protected GameObject entity = null; // 실제 투사체를 생성하기 위한 샘플. 발사가능한 투사체와 동일한 속성이어야 합니다.
    public GameObject Entity 
    {
        get { return entity; }
        set { entity = value; }
    }
}
