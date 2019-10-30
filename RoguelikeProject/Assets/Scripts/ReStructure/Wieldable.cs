using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wieldable : MonoBehaviour, IWieldable
{
    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 rotationVector;
    protected double cooldownWait;


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
        // owner가 없는 무기는 발사할 수 없습니다.
        if (!owner)
        {
            Debug.LogError("Weapon fired without owner");
            return;
        }

        // 소유자가 있는 무기라면 발사를 위해 투사체를 생성합니다.
        GameObject projectile = Instantiate(bulletType, (Vector2)owner.transform.position + rotationVector * 0.5f, Quaternion.identity) as GameObject;
        
        // 피아식별 등의 처리를 위해 투사체의 소유자(공격자)와 부모를 설정해줍니다.
        projectile.GetComponent<ProjectileOnHit>().Attacker = owner;
        projectile.transform.SetParent(owner.transform);

        // 무기의 방향으로 발사합니다. 탄속은 아직 유동옵션은 아닙니다.
        projectile.GetComponent<Rigidbody2D>().AddForce(rotationVector * 1000.0f);
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
