using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int HP;
    protected int currentHP;
    public int GetHP() {return currentHP;}
    public double maxSpeed;
    public double acceleration;
    public Vector2 faceDirection;

    protected void Start()
    {
        currentHP = HP;
    }

    protected void Move(Vector2 direction)
    {
        // TODO :: 최대속도 구현 해야함, 현재 무한가속
        // TODO :: acceleration을 deltaTime을 고려하여 값을 조절해야 합니다
        // 호출시 단위속도만큼 이동합니다. Update등의 지속호출과 연계하여야 제대로 된 움직임이 나옵니다.
        direction.Normalize();
        GetComponent<Rigidbody2D>().AddForce(direction * (float)acceleration, ForceMode2D.Impulse);
    }

    public virtual int GetDamage(int damage)
    // 모든 유닛의 HP 조작 처리는 이 메서드를 사용해야만 합니다.
    // 유닛 종류별 재정의 하는것이 *강력히* 권장됩니다.
    {
        // 실제로 받게 될 데미지를 추적연산합니다.
        int actualDamage = damage;
        
        // 실제 연산
        currentHP -= actualDamage;

        // 추적된 값을 반환합니다.        
        return actualDamage;
    }

    public Strike GetStrike (Strike strike)
    // 모든 유닛의 모든 피격은 이 메서드를 사용해야만 합니다. 
    // 직접적인 HP, transform 등의 조작은 나중에 큰 문제를 야기할 수 있습니다.
    {
        // 실제로 받게 되는 물리량을 추적연산합니다.
        Strike actualStrike = new Strike(strike);
        // 호출하는 순간 강한 힘으로 오브젝트를 밀어버립니다.
        Vector2 pushDirection = (Vector2)transform.position - strike.attackPosition;
        pushDirection.Normalize(); 
        if (gameObject.GetComponent<Rigidbody2D>())
        {
           gameObject.GetComponent<Rigidbody2D>().AddForce(pushDirection * (float)strike.force, ForceMode2D.Impulse);
        }

        // 데미지가 있다면 데미지도 받습니다
        actualStrike.damage = GetDamage(strike.damage);

        //Debug.Log(strike.damage);
        // 추적한 값을 반환합니다.
        return actualStrike;
    }

    protected virtual void SelfDestruction()
    // 기본적으로는 단순히 오브젝트를 파괴하는것으로 처리했습니다.
    // 오버라이딩해서 개별 처리를 구현하시길 바랍니다.
    {
        Destroy(gameObject);
    }

    protected void Update()
    {
        if (currentHP < 0)
        {
            // 유닛별로 다른 메서드를 호출합니다.
            SelfDestruction();
        }
    }
}
