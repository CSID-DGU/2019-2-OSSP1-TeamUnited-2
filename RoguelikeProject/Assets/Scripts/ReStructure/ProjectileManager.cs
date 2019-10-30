using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileManager : MonoBehaviour
{
    public ProjectileAttribute attribute;
    protected GameObject entity;
    public GameObject Entity 
    {
        get { return entity; }
        set { entity = value; }
    }
}
