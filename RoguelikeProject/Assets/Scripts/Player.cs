using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    protected double invincible;
    public Wieldable mainHand;

    void Update()
    {
        // 방향키를 입력받아 Move메서드를 호출해 이동을 처리합니다.
        int horizontal = Math.Round((Input.GetAxisRaw("Horizontal")));
        int vertical = Math.Round((Input.GetAxisRaw("Vertical")));
        Vector2 keyboardDirection = new Vector2(horizontal, vertical);
        keyboardDirection.Normalize();
        Move(keyboardDirection);

        // 작동이 확인되는 기존 코드
        // Vector3 mousePosition = Input.mousePosition;
        // mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // Vector2 direction = new Vector2(
        // mousePosition.x - transform.position.x,
        // mousePosition.y - transform.position.y);

        // 키보드를 입력받아 face의 위치를 조절해줍니다.
        // TODO :: 작동 확인 점검 필요
        Vector2 mousePosition = Input.moustPosition;
        Vector2 aim = mousePosition - transform.position;

        // TODO :: 현재 face에 aim을 그냥 대입하도록 되어있습니다, 시간지연을 넣는 편이 부드러울 것입니다.
        face = aim;
    }
}
