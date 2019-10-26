using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour, IWieldable
{
    public GameObject[] bulletType;
    public projectileAttribute[] bulletTypeManualSetting; // bulletType이 이미 존재한다면 오버라이딩합니다.
    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 aim;
    
    [System.Serializable]
    public struct projectileAttribute
    {
        public int          damage;
        public double       force;
        public int          areaDamage;
        public double       areaForce;
        public float        radius;
        public GameObject   animationExplosion; // 폭발할 때 효과
    }

    public void start()
    {
        for(int i = 0; i < bulletTypeManualSetting.Length; ++i)
        {
            // bulletType의 크기보다 큰 배열은 무시됩니다.
            if (i >= bulletType.Length)
                break;


            // 투사체들의 GameObject 자동 생성 시작
            GameObject ToInstantiate = new GameObject();
            
            // 만약 bulletType 객체가 없다면 만들어줍니다.
            if (bulletType[i] == null)
            {
                bulletType[i] = Instantiate(GameObject.Find("DefaultBullet"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
                
            }
            
        }
    }

    public void OnPush()
    {
        GameObject projectile = Instantiate(bulletType[0], (Vector2)transform.position + aim * 0.5f, owner.transform.rotation) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
    }

    public void OnHold()
    {

    }

    public void OnRelease()
    {

    }

    protected void Update()
    {
        // TODO :: 무기가 해제당했을 경우 소유자를 해제해 주는 것이 만약을 위해 안전할 것입니다.
    }
}
