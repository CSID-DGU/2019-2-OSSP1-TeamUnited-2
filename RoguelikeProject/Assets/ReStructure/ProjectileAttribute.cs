using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
    public struct projectileAttribute
    {
        public int          damage;
        public double       force;
        public int          areaDamage;
        public double       areaForce;
        public float        areaRadius;
        public GameObject   animationExplosion; // 폭발할 때 효과
    }
