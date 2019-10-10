using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : Unit
{
    protected double invincible;
    public Wieldable mainHand;

    public GameObject faceArrow; // --for debug--

    void Start()
    {
        // --for debug--
        faceArrow = Instantiate(faceArrow, (Vector2)transform.position, transform.rotation) as GameObject;
    }

    void Update()
    {
        // 방향키를 입력받아 Move메서드를 호출해 이동을 처리합니다.
        int horizontal = (int)Math.Round((Input.GetAxisRaw("Horizontal")));
        int vertical = (int)Math.Round((Input.GetAxisRaw("Vertical")));
        Vector2 keyboardDirection = new Vector2(horizontal, vertical);
        keyboardDirection.Normalize();
        Move(keyboardDirection);

        // 키보드를 입력받아 face의 위치를 조절해줍니다.
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (Vector2)(mousePosition - transform.position);

        // TODO :: 현재 face에 aim을 그냥 대입하도록 되어있습니다, 시간지연을 넣는 편이 부드러울 것입니다.
        direction.Normalize();
        faceDirection = direction;

        // --for debug--
        faceArrow.transform.position = (Vector2)((Vector2) transform.position + faceDirection);

        // 마우스 입력를 Wieldable 객체로 연결
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("left mouse pushed");
            // mainHand.OnPush();
            mainHand.holding = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("left mouse released");
            // mainHand.OnRelease();
            mainHand.holding = false;
        }
    }
}
