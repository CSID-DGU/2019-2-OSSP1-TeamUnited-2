using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    protected double invincible;
    public Wieldable mainHand;

    void Update()
    {
        //방향키를 입력받아 Move메서드를 호출해 이동을 처리합니다.
        int horizontal = Math.Round((Input.GetAxisRaw("Horizontal")));
        int vertical = Math.Round((Input.GetAxisRaw("Vertical")));
        Vector2 keyboardDirection = new Vector2(horizontal, vertical);
        keyboardDirection.Normalize();
        Move(keyboardDirection);

        Vector2 mouseDirection = 

    }
}
