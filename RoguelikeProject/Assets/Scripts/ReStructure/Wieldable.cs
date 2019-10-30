using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour, IWieldable
{
    public GameObject[] bulletType;
    public ProjectileAttribute[] bulletTypeManualSetting; // bulletType이 이미 존재한다면 오버라이딩합니다.

    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 rotationVector;
    protected double cooldownWait;

    void Start()
    {
        // Debug.Log("Weapon Start<Default Setting>: "+ bulletTypeManualSetting.Length);
        // Debug.Log("Weapon Start<Manual Setting>: "+ bulletType.Length);
    }

    // public void Init()
    // {
    //     for(int i = 0; i < bulletTypeManualSetting.Length; ++i)
    //     {
    //         // bulletType의 크기보다 큰 배열은 무시됩니다.
    //         if (i >= bulletType.Length)
    //             break;

    //         // Debug.Log("Bullet Setting Manual");
    //         // 만약 bulletType 객체가 없다면 만들어줍니다.
    //         if (bulletType[i] == null)
    //         {
    //             bulletType[i] = Instantiate(GameObject.Find("DefaultBullet"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
    //             Debug.LogError("Default Bullet Created");
    //         }

    //         bulletType[i].GetComponent<ProjectileOnHit>().SetAttribute(bulletTypeManualSetting[i]);
    //     }
    // }

    public void FireRangeDirect(GameObject bulletType)
    {
        GameObject projectile = Instantiate(bulletType, (Vector2)owner.transform.position + rotationVector * 0.5f, Quaternion.identity) as GameObject;
        projectile.GetComponent<ProjectileOnHit>().SetAttacker(owner);
        projectile.GetComponent<Rigidbody2D>().AddForce(rotationVector * 1000.0f);
        projectile.transform.SetParent(owner.transform);
    }   

    public virtual void OnPush()
    {
        
    }

    public virtual void OnHold()
    {

    }

    public virtual void OnRelease()
    {

    }

    protected void Update()
    {
        if (cooldownWait > 0)
        {
            cooldownWait -= Time.deltaTime;
        }

        if (owner)
        {
            rotationVector = owner.GetComponent<Unit>().RotationVector;
        }
    }
}
