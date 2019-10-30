using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileManager
{
    public ProjectileAttribute attribute;
    public GameObject shape; // Sprite, RigidBody2D, Collider로 구성된 형체 오브젝트
    protected GameObject entity = null;
    public GameObject Entity 
    {
        get
        {
            if (entity != null) 
            {
                return entity;
            }
            else
            {
                entity = shape;
                entity.AddComponent<ProjectileOnHit>().SetAttribute(attribute);
                return entity;
            }
        }
        set { entity = value; }
    }
}
