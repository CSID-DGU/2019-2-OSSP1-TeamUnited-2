using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wieldable : MonoBehaviour
{
    [System.Serializable]
    public class ProjectileManager
    {
        public ProjectileAttribute attribute;
        public GameObject shape; // Sprite, Collider, RigidBody로 구성된 형체 오브젝트
        protected GameObject entity = null; // 실제 투사체를 생성하기 위한 샘플. 발사가능한 투사체와 동일한 속성이어야 합니다.
        public GameObject Entity 
        {
            get { return entity; }
            set { entity = value; }
        }
    }
    protected GameObject owner;
    public GameObject Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    public Vector2 rotationVector;
    protected double cooldownWait;
    protected GameObject projectileHolder = null;

    // 파라미터에는 발사 가능한 투사체를 넣습니다 
    // 발사 가능한 투사체는 SpriteRenderer, Collider, Rigidbody, ProjectileOnHit 컴포넌트를 모두 가지고 있는 게임 오브젝트여야 합니다.
    // ProjectileManager 인스턴스의 Entity 필드를 넣는 것으로 의도되어 있습니다만, 어떤 형태로든 발사 가능한 투사체를 넣으면 작동은 됩니다.
    protected void FireRangeDirect(GameObject bulletType)
    {
        // owner가 없는 무기는 발사할 수 없습니다.
        if (owner == null)
        {
            Debug.LogError("Weapon fired without owner");
            return;
        }

        // 소유자가 있는 무기라면 발사를 위해 투사체를 생성합니다.
        GameObject projectile = Instantiate(bulletType, (Vector2)owner.transform.position + rotationVector * 0.5f, Quaternion.identity) as GameObject;
        
        // 피아식별 등의 처리를 위해 공격주체(owner)와 부모를 설정해줍니다.
        projectile.GetComponent<ProjectileOnHit>().Attacker = owner;
        projectile.transform.SetParent(GameObject.Find("GameManager").transform);

        // 투사체는 반드시 공격자의 정보를 담고 있어야 합니다.
        // Debug.Log("Projectile fired, Attacker: " + projectile.GetComponent<ProjectileOnHit>().Attacker.GetInstanceID());


        //현재 무기의 위치 
        Vector2 positionOnScreen = (Vector2)Camera.main.WorldToViewportPoint(owner.transform.position);
        // Vector2 positionOnScreen = owner.transform.position;

        //현재 마우스의 위치 
        // Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(owner.transform.position) + rotationVector;
        // Vector2 mouseOnScreen = (Vector2)owner.transform.position + rotationVector;
        
        Vector3 mouseOnScreen = Input.mousePosition;
        mouseOnScreen = Camera.main.ScreenToWorldPoint(mouseOnScreen);

        // 무기와 마우스의 위치의 각도 
        // float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        // Debug.Log("Player Position : " + positionOnScreen +  ", Mouse Position : " + mouseOnScreen + ", Angle : " +  angle);
        float angle = GameObject.Find("Player").GetComponent<Player>().angle;

        // 각도만큼 로테이션값 주기 
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        

        // 무기의 방향으로 발사합니다. 탄속은 아직 유동옵션은 아닙니다.
        projectile.GetComponent<Rigidbody2D>().AddForce(rotationVector * 1000.0f);
    }

    protected void ProjectileInstantiation(ProjectileManager pManager)
    {
        // 기존의 정보는 삭제합니다.
        pManager.Entity = null;

        // 새로운 샘플을 생성합니다. 투사체 발사시 해당 샘플을 복제해서 발사하게 됩니다.
        pManager.Entity = Instantiate(pManager.shape);
        pManager.Entity.AddComponent<ProjectileOnHit>();
        pManager.Entity.GetComponent<ProjectileOnHit>().Attribute = pManager.attribute;

        // 생성된 샘플의 부모를 설정하고 격리된 공간에 보관합니다.
        // TODO :: 공간이 완벽하게 격리되어있지 않습니다.
        pManager.Entity.transform.SetParent(transform);
        pManager.Entity.transform.position = new Vector3(-1000,-1000,-1000);
        Debug.Log("Projectile sample entity initialized");        
    }

    // 무기의 작동방식은 상속받은 클래스에서 커스텀하여 사용합니다.
    public abstract void OnPush();
    public abstract void OnHold();
    public abstract void OnRelease();

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

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
