using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int HP;
    protected int currentHP;
    public double maxSpeed;
    public double acceleration;
    public Vector2 faceDirection;

    protected void Move(Vector2 direction)
    {
        // TODO :: 최대속도 구현 해야함, 현재 무한가속
        // TODO :: acceleration을 deltaTime을 고려하여 값을 조절해야 합니다
        // 호출시 단위속도만큼 이동합니다. Update등의 지속호출과 연계하여야 제대로 된 움직임이 나옵니다.
        direction.Normalize();
        GetComponent<Rigidbody2D>().AddForce(direction * (float)acceleration, ForceMode2D.Impulse);
    }

    public void GetStrike (Strike strike)
    // 모든 유닛의 모든 피격은 이 메서드를 사용해야만 합니다. 
    // 직접적인 HP, transform 등의 조작은 나중에 큰 문제를 야기할 수 있습니다.
    {
        // 호출하는 순간 강한 힘으로 객체를 밀어버립니다.
        Vector2 pushDirection = (Vector2)transform.position - strike.attackPosition;
        pushDirection.Normalize(); 
        GetComponent<Rigidbody2D>().AddForce(pushDirection * (float)strike.attackType.force, ForceMode2D.Impulse);

        // 데미지가 있다면 데미지도 받습니다.
        currentHP -= strike.attackType.damage;
    }

    void Update()
    {
        if (currentHP < 0)
        {
            // 일단은 HP가 움수가 되면 오브젝트를 파괴하도록 했습니다.
            // 플레이어 등의 경우 단순 오브젝트 파괴는 문제가 될 수 있으므로 각 유닛별 다른 처리가 필요할 것입니다.
            Destroy(gameObject);
        }
    }
}
