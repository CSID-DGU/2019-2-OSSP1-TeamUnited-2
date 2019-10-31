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

    // 파라미터에는 발사 가능한 투사체를 넣습니다 
    // 발사 가능한 투사체는 SpriteRenderer, Collider, Rigidbody, ProjectileOnHit 컴포넌트를 모두 가지고 있는 게임 오브젝트여야 합니다.
    // ProjectileManager 인스턴스의 Entity 필드를 넣는 것으로 의도되어 있습니다만, 어떤 형태로든 발사 가능한 투사체를 넣으면 작동은 됩니다.
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
        
        // 피아식별 등의 처리를 위해 공격주체(owner)와 부모를 설정해줍니다.
        projectile.GetComponent<ProjectileOnHit>().Attacker = owner;
        projectile.transform.SetParent(owner.transform);

        // 무기의 방향으로 발사합니다. 탄속은 아직 유동옵션은 아닙니다.
        projectile.GetComponent<Rigidbody2D>().AddForce(rotationVector * 1000.0f);
    }   

    // 무기의 작동방식은 상속받은 클래스에서 커스텀하여 사용합니다.
    public virtual void OnPush() {}
    public virtual void OnHold() {}
    public virtual void OnRelease() {}

    // 모든 무기 공용인 쿨다운 처리 및 조준점 처리를 합니다.
    // 상속받은 객체에서 Update를 재정의 할 경우 반드시 base.Update()를 호출해주어야 합니다.
    // 상속받은 객체에서 Update기능을 사용하지 않을 것을 강력히 권장합니다.
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
