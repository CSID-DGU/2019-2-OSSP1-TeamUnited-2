using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int HP;
    protected int currentHP;
    public double maxSpeed;
    public double acceleration;
    public Vector2 face;

    protected void Move(Vector2 direction)
    {
        //TODO:: 최대속도 구현 해야함, 현재 무한가속
        //호출시 단위속도만큼 이동합니다. Update등의 지속호출과 연계하여야 제대로 된 움직임이 나옵니다.
        direction.Normalize(); //방향을 단위벡터화
        GetComponent<Rigidbody2D>().AddForce(direction * acceleration, ForceMode2D.Impulse);
    }

    public void GetStrike (Strike strike)
    {
        //호출하는 순간 강한 힘으로 객체를 밀어버립니다.
        Vector2 pushDirection = transform.position - strike.attackPosition; //(공격을 받아) 밀려날 방향
        pushDirection.Normalize(); 
        GetComponent<Rigidbody2D>().AddForce(pushDirection * strike.attackType.force, ForceMode2D.Impulse);

        //데미지가 있다면 데미지도 받습니다.
        currentHP -= strike.attackType.damage;
    }
}
