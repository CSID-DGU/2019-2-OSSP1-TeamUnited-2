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
        direction.Normalize(); //방향을 크기가 1인 단위벡터로 정규화
        GetComponent<Rigidbody2D>().AddForce(direction * acceleration, ForceMode2D.Impulse);
    }
}
