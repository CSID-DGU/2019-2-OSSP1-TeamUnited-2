using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour, IWieldable
{
    public GameObject[] bulletType;
    public ProjectileAttribute[] bulletTypeManualSetting; // bulletType이 이미 존재한다면 오버라이딩합니다.
    // public int test;

    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 aim;

    void Start()
    {
        Debug.Log("Weapon Start<Default Setting>: "+ bulletTypeManualSetting.Length);
        Debug.Log("Weapon Start<Manual Setting>: "+ bulletType.Length);
        // Debug.Log("Weapon Start: "+ test);
    }

    public void Init()
    {
        Debug.Log("init weapon: " + bulletTypeManualSetting.Length);
        Debug.Log("AreaForce before : " + bulletType[0].GetComponent<ProjectileOnHit>().attribute.areaForce);
        for(int i = 0; i < bulletTypeManualSetting.Length; ++i)
        {
            // bulletType의 크기보다 큰 배열은 무시됩니다.
            if (i >= bulletType.Length)
                break;

            Debug.Log("Bullet Setting Manual");            
            // 만약 bulletType 객체가 없다면 만들어줍니다.
            if (bulletType[i] == null)
            {
                bulletType[i] = Instantiate(GameObject.Find("DefaultBullet"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
                Debug.LogError("Default Bullet Created");
            }

            Debug.Log("AreaForce Setting : " + bulletTypeManualSetting[i].areaForce);
            bulletType[i].GetComponent<ProjectileOnHit>().SetAttribute(bulletTypeManualSetting[i]); 
            Debug.Log("AreaForce Set : " + bulletType[i].GetComponent<ProjectileOnHit>().attribute.areaForce);
            Debug.Log("Bullet AreaForce : " + bulletType[0].GetComponent<ProjectileOnHit>().attribute.areaForce);
        }
    }

    public void OnPush()
    {
        Debug.Log("Weapon Status: " + bulletTypeManualSetting.Length);
        GameObject projectile = Instantiate(bulletType[0], (Vector2)owner.transform.position + aim * 0.5f, owner.transform.rotation) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
    }

    public void OnHold()
    {

    }

    public void OnRelease()
    {

    }
}
