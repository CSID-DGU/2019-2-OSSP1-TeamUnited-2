using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileAttribute
{
    public int          damage;
    public double       force;
    public int          areaDamage;
    public double       areaForce;
    public float        areaRadius;
    public GameObject   animationExplosion = null; // 폭발할 때 효과
}
