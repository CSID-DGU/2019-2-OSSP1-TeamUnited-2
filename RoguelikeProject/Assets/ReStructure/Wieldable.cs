using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable
{
    public GameObject[] bulletType;
    public ProjectileAttribute[] bulletTypeManualSetting; // bulletType이 이미 존재한다면 오버라이딩합니다.
    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 aim;

    // Start() 혹은 Awake()로 동적배열 접근시 오류가 발생합니다. (배열의 생성 시점이 Start() 뒤로 추정됩니다)
    // 일단 첫번째 Update때 Initialize()를 불러오는 방식을 채택합니다.
    // 추후 구조개선이 필요할 수 있습니다.
    private bool init;
    private double currentForce;
    void Init()
    {
        Debug.Log(this.GetInstanceID());
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
                Debug.LogError("Default bullet Created");
            }

            Debug.Log("AreaForce Setting : " + bulletTypeManualSetting[i].areaForce);
            bulletType[i].GetComponent<ProjectileOnHit>().SetAttribute(bulletTypeManualSetting[i]); 
            Debug.Log("AreaForce Set : " + bulletType[i].GetComponent<ProjectileOnHit>().attribute.areaForce);
            Debug.Log("Bullet AreaForce : " + bulletType[0].GetComponent<ProjectileOnHit>().attribute.areaForce);
        }
        currentForce = bulletType[0].GetComponent<ProjectileOnHit>().attribute.areaForce;
        init = true;
    }

    public void OnPush()
    {
        Debug.Log("CurrentForce : " + this.currentForce);
        Debug.Log("Bullet AreaForce : " + bulletType[0].GetComponent<ProjectileOnHit>().attribute.areaForce);
        GameObject projectile = Instantiate(bulletType[0], (Vector2)transform.position + aim * 0.5f, owner.transform.rotation) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(aim * 1000.0f);
        Debug.Log(this.GetInstanceID());
    }

    public void OnHold()
    {

    }

    public void OnRelease()
    {

    }
}
